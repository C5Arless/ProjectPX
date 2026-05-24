using System;
using UnityEngine;

public class PX3Sensor : MonoBehaviour {
    [SerializeField] PX3SensorType type;
    
    public event Action<Collider, PX3SensorType, PX3SensorStage> OnSensorTrigger;
    public event Action<Collision, PX3SensorType, PX3SensorStage> OnSensorCollision;
    
    private void OnTriggerEnter(Collider other) => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Enter);
    private void OnTriggerStay(Collider other)  => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Stay);
    private void OnTriggerExit(Collider other)  => OnSensorTrigger?.Invoke(other, type, PX3SensorStage.Exit);
    
    private void OnCollisionEnter(Collision collision) => OnSensorCollision?.Invoke(collision, type, PX3SensorStage.Enter);
    private void OnCollisionStay(Collision collision)  => OnSensorCollision?.Invoke(collision, type, PX3SensorStage.Stay);
    private void OnCollisionExit(Collision collision)  => OnSensorCollision?.Invoke(collision, type, PX3SensorStage.Exit);
}