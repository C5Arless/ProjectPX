using System;
using UnityEngine;

public class PX3Sensor : MonoBehaviour {
    [SerializeField] public PX3SensorType type;
    private new MeshCollider collider;
    public MeshCollider Collider => collider;
    public event Action<Collider, PX3SensorType, PX3SensorStage> OnSensorTrigger;

    private void Awake() {
        collider = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other) => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Enter);
    private void OnTriggerStay(Collider other)  => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Stay);
    private void OnTriggerExit(Collider other)  => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Exit);
    
}