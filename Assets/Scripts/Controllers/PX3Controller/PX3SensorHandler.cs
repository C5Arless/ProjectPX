using System;
using UnityEngine;

public class PX3SensorHandler : MonoBehaviour {
    private PX3Sensor[] sensors;
    private PX3SensorForwarder sensorForwarder;
    
    public event Action<Collider, PX3SensorType, PX3SensorStage> OnSensorsTrigger;
    public event Action<Collision, PX3SensorType, PX3SensorStage> OnSensorsCollision;

    public void DisableCollisions(PX3SensorType sensorType) {
        foreach (var sensor in sensors) {
            if (sensor.type == sensorType) {
                sensor.Collider.isTrigger = true;
            }
        }
    }
    
    public void EnableCollisions(PX3SensorType sensorType) {
        foreach (var sensor in sensors) {
            if (sensor.type == sensorType) {
                sensor.Collider.isTrigger = false;
            }
        }
    }
    
    public void Initialize(PX3SensorForwarder forwarder) {
        sensorForwarder = forwarder;
        sensors = GetComponentsInChildren<PX3Sensor>();
    }

    public PX3Sensor GetSensor(PX3SensorType sensorType) {
        PX3Sensor _sensor = null;
        
        foreach (var sensor in sensors) {
            if (sensor.type == sensorType) _sensor = sensor;
        }
        
        return _sensor;
    }
    
    private void Start() {
        foreach (var sensor in sensors) {
            sensor.OnSensorTrigger += HandleTrigger;
        }
        
        sensorForwarder.OnSensorCollision += HandleCollision;
    }
    
    private void HandleTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        OnSensorsTrigger?.Invoke(other, type, stage);
    }

    private void HandleCollision(Collision collision, PX3SensorType type, PX3SensorStage stage) {
        OnSensorsCollision?.Invoke(collision, type, stage);
    }

    private void OnDestroy() {
        OnSensorsCollision = null;
        OnSensorsTrigger = null;
    }
}
