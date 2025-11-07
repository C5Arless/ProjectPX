using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class PatrolZone : MonoBehaviour{
    [SerializeField] public NavMeshSurface patrolArea;
    [SerializeField] public List<Transform> waypoints;

    public List<bool> visitedWaypoints;
    private NavMeshQuery navQuery;
    private NavMeshWorld navWorld;
    private Bounds areaBounds;

    private void Awake() {
        navWorld = NavMeshWorld.GetDefaultWorld();
        navQuery = new NavMeshQuery(navWorld, Allocator.Persistent);
        areaBounds = patrolArea.navMeshData.sourceBounds;
        areaBounds.center = transform.position;
        
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

        navQuery.Dispose();
    }
    
    public bool ValidateTarget(GameObject target) {
        Vector3 targetPos = target.transform.position;
        
        if (!areaBounds.Contains(targetPos)) {
            return false;
        }
        
        NavMeshLocation location = navQuery.MapLocation(
            targetPos,
            Vector3.one * 3f,
            NavMesh.AllAreas
        );
        
        if (location.polygon.IsNull()) return false;

        return true;

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
        
        int tempIndex = Random.Range(0, targetWaypoints.Count);
        
        int targetIndex = targetIdxs[tempIndex];
        
        visitedWaypoints[targetIndex] = true;
        
        return waypoints[targetIndex].position;
    }
}
