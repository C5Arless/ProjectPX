using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour {
    [Header("Riferimenti")]
    [SerializeField] private Transform player; //DEBUG

    [Header("Parametri Spawn")]
    [SerializeField] private float timeToWait = 2f;   // ogni quanto fare il check
    [SerializeField] private float minDistance = 20f;       // distanza minima dal player

    private readonly List<ISpawnable> spawnables = new List<ISpawnable>();

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
        *///DEBUG

        StartCoroutine(SpawnLoop());
    }

    public void Register(ISpawnable spawnable) {
        if (!spawnables.Contains(spawnable)) {
            spawnables.Add(spawnable);
        }
    }

    public void Unregister(ISpawnable spawnable) {
        if (spawnables.Contains(spawnable)) {
            spawnables.Remove(spawnable);
        }
    }

    private IEnumerator SpawnLoop() {
        while (true) {
            CheckAndSpawn();
            yield return new WaitForSeconds(timeToWait);
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
