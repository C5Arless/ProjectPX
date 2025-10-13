using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour {
    [SerializeField] private Transform player; //DEBUG
    
    [SerializeField] private float timeToWait = 2f;
    [SerializeField] private float minDistance = 20f;

    private readonly List<ISpawnable> spawnables = new List<ISpawnable>();
    private readonly List<PatrolZone> patrolZones = new List<PatrolZone>();

    private void Awake() {
        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler = this;
        }
    }

    private void Start() {
        /*
        if (GameBucket.Instance != null) {
            player = GameBucket.Instance.PXController.transform;
        } 
        */ //DEBUG

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
                target = patrolZone;
            }
        }

        return target;
    }
    
    private IEnumerator SpawnLoop() {
        while (true) {
            if (spawnables.Count > 0) {
                CheckAndSpawn();
                yield return new WaitForSeconds(timeToWait);
            }
            else {
                yield return null;
            }
            
        }
    }

    private void CheckAndSpawn() {
        if (player == null) return;
        
        foreach (var spawnable in spawnables) {            
            if (spawnable.IsRunning || spawnable.IsReady || spawnable.IsSpawning) continue;

            float distance = Vector3.Distance(player.position, spawnable.Position);
            if (distance <= minDistance) {
                spawnable.IsReady = true;
            }
        }
        
        foreach (var spawnable in spawnables) {
            if (spawnable.IsReady && !spawnable.IsRunning && !spawnable.IsSpawning) {
                spawnable.Spawn();                
            }
        }
    }
}
