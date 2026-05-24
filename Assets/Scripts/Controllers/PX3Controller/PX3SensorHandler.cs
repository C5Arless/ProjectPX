using System;
using UnityEngine;

public class PX3SensorHandler : MonoBehaviour {
    private PX3Controller _ctx;
    private PX3Sensor[] sensors;
    
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
        switch (type) {
            case PX3SensorType.Ground:
                if (other.CompareTag("Ground")) GroundTrigger(stage);
                break;
            case PX3SensorType.Ledge:
                LedgeTrigger(other, stage);
                break;
        }
    }

    private void HandleCollision(Collision collision, PX3SensorType type, PX3SensorStage stage) {
        if (type == PX3SensorType.Body && stage == PX3SensorStage.Enter) {
            Debug.Log($"Urto fisico contro {collision.gameObject.name}!");
        }
    }

    private void OnDestroy() {
        foreach (var sensor in sensors) {
            sensor.OnSensorTrigger -= HandleTrigger;
            sensor.OnSensorCollision -= HandleCollision;
        }
    }

    private void GroundTrigger(PX3SensorStage stage) {
        if (stage == PX3SensorStage.Enter) {
            _ctx.StateHandler.SetRootState(PX3RootStates.Grounded);
        }
        else if (stage == PX3SensorStage.Exit) {
            _ctx.StateHandler.SetRootState(PX3RootStates.Airborne);
        }
    }

    private void LedgeTrigger(Collider collider, PX3SensorStage stage) {
        if (stage == PX3SensorStage.Enter) {
            _ctx.StateHandler.SetRootState(PX3RootStates.Holding);
            
            if (collider.CompareTag("Wall")) _ctx.StateHandler.SetSubState(PX3SubStates.WallSliding);
            else if (collider.CompareTag("Ground")) _ctx.StateHandler.SetSubState(PX3SubStates.Grabbing);
        }
        else if (stage == PX3SensorStage.Exit) {
            _ctx.StateHandler.SetRootState(PX3RootStates.Airborne);
        }
    }
}
