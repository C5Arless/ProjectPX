using System;
using UnityEngine;

public class PX3SensorHandler : MonoBehaviour {
    private PX3Sensor[] sensors;
    
    public event Action<Collider, PX3SensorType, PX3SensorStage> OnSensorsTrigger;
    public event Action<Collision, PX3SensorType, PX3SensorStage> OnSensorsCollision;
    
    void Awake() {
        sensors = GetComponentsInChildren<PX3Sensor>();
    }

    private void Start() {
        foreach (var sensor in sensors) {
            sensor.OnSensorTrigger += HandleTrigger;
            sensor.OnSensorCollision += HandleCollision;
        }
    }
    
    private void HandleTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        OnSensorsTrigger?.Invoke(other, type, stage);
    }

    private void HandleCollision(Collision collision, PX3SensorType type, PX3SensorStage stage) {
        OnSensorsCollision?.Invoke(collision, type, stage);
    }

    private void OnDestroy() {
        /*
        foreach (var sensor in sensors) {
            sensor.OnSensorTrigger -= HandleTrigger;
            sensor.OnSensorCollision -= HandleCollision;
        }
        */
        
        OnSensorsCollision = null;
        OnSensorsTrigger = null;
    }
}
