using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour {
    [SerializeField] private float timeToWait = 1f;
    [SerializeField] private float minDistance = 20f;

    private readonly List<PatrolZone> patrolZones = new List<PatrolZone>();
    
    private List<ISpawnable> spawnables = new List<ISpawnable>();
    private List<ISpawnable> spawnBuffer = new List<ISpawnable>();

    //private bool isBusy;

    private void Awake() {
        if (GameBucket.Instance is not null) {
            GameBucket.Instance.SpawnHandler = this;
        }
    }

    private void Start() {

        StartCoroutine(SpawnLoop());
    }

    public void RegisterSpawn(ISpawnable spawnable) {
        if (!spawnBuffer.Contains(spawnable)) {
            spawnBuffer.Add(spawnable);
        }
    }

    public void UnregisterSpawn(ISpawnable spawnable) {
        if (spawnBuffer.Contains(spawnable)) {
            spawnBuffer.Remove(spawnable);
        }
    }

    public void RegisterPatrolZone(PatrolZone patrolZone) {
        if (!patrolZones.Contains(patrolZone)) {
            patrolZones.Add(patrolZone);
        }
    }

    public void UnregisterPatrolZone(PatrolZone patrolZone) {
        if (patrolZones.Contains(patrolZone)) {
            patrolZones.Remove(patrolZone);
        }
    }

    public PatrolZone GetPatrolZone(Transform entity) {
        PatrolZone target = null;
        float distance = Mathf.Infinity;
        
        foreach (PatrolZone patrolZone in patrolZones) {
            float tempDistance = Vector3.Distance(entity.position, patrolZone.transform.position);

            if (tempDistance < distance) {

                distance = tempDistance;
                target = patrolZone;
            }
        }

        return target;
    }
    
    private IEnumerator SpawnLoop() {
        while (true) {
            yield return Sync();
            
            if (spawnables.Count > 0) {
                yield return CheckAndSpawn();
                //yield return DequeueSpawns()
            }
            else {
                yield return new WaitForSeconds(.5f);
            }
        }
    }

    private IEnumerator Sync() {
        //isBusy = true;
        List<ISpawnable> targets = new List<ISpawnable>(spawnBuffer);
        yield return null;

        //targets = spawnBuffer;
        spawnables = new List<ISpawnable>(targets);
        yield return new WaitForSeconds(.5f);

        //isBusy = false;
        yield break;
    }
    
    private IEnumerator CheckAndSpawn() {
        //isBusy = true;
        yield return null;
        
        if (GameBucket.Instance.PXController is null) {
            yield break;
        }

        List<ISpawnable> spawns = new List<ISpawnable>();
        
        foreach (var spawnable in spawnables) {            
            if (spawnable.IsRunning || spawnable.IsReady || spawnable.IsSpawning) continue;
            
            float distance = Vector3.Distance(GameBucket.Instance.PXController.Asset.transform.position, spawnable.Position);
            if (distance <= minDistance) {
                spawnable.IsReady = true;
            }
            
            yield return null;
        }
        
        foreach (var spawnable in spawnables) {
            if (spawnable.IsReady && !spawnable.IsRunning && !spawnable.IsSpawning) {
                spawnable.Spawn();
                spawns.Add(spawnable);
                yield return new WaitForSeconds(timeToWait);
            }
        }

        foreach (var spawn in spawns) {
            UnregisterSpawn(spawn);
            yield return null;
        }
        
        //isBusy = false;
        yield break;
    }
    
    
}
