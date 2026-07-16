using System;
using System.Collections.Generic;
using UnityEngine;

public class PX3SensorForwarder : MonoBehaviour {
    private Dictionary<Collider, PX3Sensor> collisionBuffer = new Dictionary<Collider, PX3Sensor>();
    public event Action<Collision, PX3SensorType, PX3SensorStage> OnSensorCollision;

    private void OnCollisionEnter(Collision collision) {
        for (int i = 0; i <= collision.contactCount; i++) {
            Collider other = collision.collider;
            PX3Sensor sensor = collision.contacts[0].thisCollider.gameObject.GetComponent<PX3Sensor>();
            
            collisionBuffer[other] = sensor;
            OnSensorCollision?.Invoke(collision, sensor.type, PX3SensorStage.Enter);
        }
    }
    
    private void OnCollisionStay(Collision collision) {
        if (collisionBuffer.TryGetValue(collision.collider, out PX3Sensor sensor)) {
            OnSensorCollision?.Invoke(collision, sensor.type, PX3SensorStage.Stay);
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collisionBuffer.TryGetValue(collision.collider, out PX3Sensor sensor)) {
            OnSensorCollision?.Invoke(collision, sensor.type, PX3SensorStage.Exit);
            collisionBuffer.Remove(collision.collider);
        }
    }
}
