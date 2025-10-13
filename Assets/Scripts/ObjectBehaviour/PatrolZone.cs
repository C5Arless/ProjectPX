using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class PatrolZone : MonoBehaviour{
    [SerializeField] public int areaIndex;
    [SerializeField] public NavMeshSurface patrolArea;
    [SerializeField] public List<Transform> waypoints;

    private void Start() {
        GameBucket.Instance.SpawnHandler.RegisterPatrolZone(this);
    }

    private void OnDestroy() {
        if (GameBucket.Instance != null) { 
            GameBucket.Instance.SpawnHandler.UnregisterPatrolZone(this);   
        }
    }
}
