using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = System.Random;

public class PatrolZone : MonoBehaviour{
    [SerializeField] public int areaIndex;
    [SerializeField] public NavMeshSurface patrolArea;
    [SerializeField] public List<Transform> waypoints;

    public List<bool> visitedWaypoints;

    private void Awake() {
        visitedWaypoints = new List<bool>(waypoints.Count);

        foreach (var visitedWaypoint in waypoints) {
            visitedWaypoints.Add(false);
        }
    }

    private void Start() {
        GameBucket.Instance.SpawnHandler?.RegisterPatrolZone(this);
    }

    private void OnDestroy() {
        if (GameBucket.Instance != null) { 
            GameBucket.Instance.SpawnHandler?.UnregisterPatrolZone(this);   
        }
    }

    public Vector3 RetrieveWaypoint() {
        List<Transform> targetWaypoints = new List<Transform>();
        List<int> targetIdxs = new List<int>();
        
        for (int i = 0; i < visitedWaypoints.Count; i++) {
            if (!visitedWaypoints[i]) {
                targetWaypoints.Add(waypoints[i]);
                targetIdxs.Add(i);
            }
        }

        if (targetWaypoints.Count <= 0) {
            visitedWaypoints = new List<bool>(waypoints.Count);
            targetIdxs = new List<int>(waypoints.Count);
            
            foreach (var visitedWaypoint in waypoints) {
                targetIdxs.Add(waypoints.IndexOf(visitedWaypoint));
                visitedWaypoints.Add(false);
            }
            
            targetWaypoints = waypoints;
        }
        
        int tempIndex = UnityEngine.Random.Range(0, targetWaypoints.Count);
        
        int targetIndex = targetIdxs[tempIndex];
        
        visitedWaypoints[targetIndex] = true;
        
        return waypoints[targetIndex].position;
    }
}
