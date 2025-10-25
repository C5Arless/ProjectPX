using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour {
    [SerializeField] private Transform player; //DEBUG
    
    [SerializeField] private float timeToWait = 1f;
    [SerializeField] private float minDistance = 20f;

    private readonly List<ISpawnable> spawnables = new List<ISpawnable>();
    private readonly List<PatrolZone> patrolZones = new List<PatrolZone>();

    private void Awake() {
        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler = this;
        }
    }

    private void Start() {
        if (GameBucket.Instance.PXController != null) {
            player = GameBucket.Instance.PXController.transform;
        }

        StartCoroutine(SpawnLoop());
    }

    public void RegisterSpawn(ISpawnable spawnable) {
        if (!spawnables.Contains(spawnable)) {
            spawnables.Add(spawnable);
        }
    }

    public void UnregisterSpawn(ISpawnable spawnable) {
        if (spawnables.Contains(spawnable)) {
            spawnables.Remove(spawnable);
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
            if (spawnables.Count > 0) {
                yield return CheckAndSpawn();
            }
            else {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator CheckAndSpawn() {
        if (player == null) yield break;
        
        List<ISpawnable> spawns = new List<ISpawnable>();
        
        foreach (var spawnable in spawnables) {            
            if (spawnable.IsRunning || spawnable.IsReady || spawnable.IsSpawning) continue;

            float distance = Vector3.Distance(player.position, spawnable.Position);
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
        
        yield break;
    }
}
