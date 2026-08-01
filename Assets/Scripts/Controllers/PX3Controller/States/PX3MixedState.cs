using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3MixedState : PX3BaseState {
    private PX3Sensor groundSensor;
    private PX3Sensor bodySensor;
    private PX3Sensor ledgeSensor;
    
    public PX3MixedState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        IsRootState = true;
        SensorHandler.OnSensorsTrigger += OnLedgeTrigger;
    }

    public override void EnterState() {
        
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        PhysicsHandler.UpdateGravity(0f);
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.RootStates[PX3RootStates.Airborne]) SwitchState(StateHandler.GetState(PX3RootStates.Airborne));
        else if (StateHandler.RootStates[PX3RootStates.Grounded]) SwitchState(StateHandler.GetState(PX3RootStates.Grounded));
        else if (StateHandler.RootStates[PX3RootStates.Dead]) SwitchState(StateHandler.GetState(PX3RootStates.Dead));
    }

    private void OnLedgeTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (stage == PX3SensorStage.Exit) return;
        if (type != PX3SensorType.Ledge) return;
        if (!other.CompareTag("Ground") && !other.CompareTag("Wall")) return;
        if (StateHandler.RootStates[PX3RootStates.Mixed]) return;
        if (StateHandler.RootStates[PX3RootStates.Grounded]) return;
   
        if (stage == PX3SensorStage.Enter) {
            groundSensor ??= SensorHandler.GetSensor(PX3SensorType.Ground);
            bodySensor ??= SensorHandler.GetSensor(PX3SensorType.Body);
            ledgeSensor ??= SensorHandler.GetSensor(PX3SensorType.Ledge);

            if (ValidateLedgeGrab()) return;
            
            if (RetrieveSnapPoints(out Vector3 snapPosition, out Vector3 snapForward, out bool isGrab)) {
                SensorHandler.DisableCollisions(PX3SensorType.Body);
                StateHandler.SetRootState(PX3RootStates.Mixed);

                if (isGrab) {
                    PhysicsHandler.Freeze();
                    StateHandler.SetSubState(PX3SubStates.Grabbing);
                } else {
                    PhysicsHandler.Freeze();
                    StateHandler.SetSubState(PX3SubStates.WallSliding);
                }
                
                Context.transform.position = snapPosition;
                Context.Asset.transform.forward = snapForward;
                
            }
        }
    }

    private bool RetrieveSnapPoints(out Vector3 snapPosition, out Vector3 snapForward, out bool isGrab) {
        snapPosition = Vector3.zero;
        snapForward = Vector3.zero;
        isGrab = false;
        
        float wallOffset = ((new Vector2(bodySensor.Collider.bounds.center.x, bodySensor.Collider.bounds.center.z)) -
                           (new Vector2(ledgeSensor.Collider.bounds.center.x, ledgeSensor.Collider.bounds.center.z))).magnitude;
        
        if (Physics.Raycast(bodySensor.transform.position, Context.Asset.transform.forward, out RaycastHit wallHit, 1f, LayerMask.GetMask("Walls"))) {
            //if (Vector3.Dot(Context.Asset.transform.forward, -wallHit.normal) < 1) return false; 
            
            Vector3 ledgePoint = wallHit.point - (wallHit.normal * wallOffset);
            ledgePoint.y += bodySensor.Collider.bounds.extents.y;

            if (Physics.Raycast(ledgePoint, Vector3.down, out RaycastHit groundHit, 1f, LayerMask.GetMask("Ground"))) {
                snapPosition = wallHit.point + (wallHit.normal * wallOffset);
                snapPosition.y = groundHit.point.y - bodySensor.Collider.bounds.extents.y;
                snapForward = -wallHit.normal;
                isGrab = true;

                return true;
            }

            snapPosition = wallHit.point + (wallHit.normal * wallOffset);
            snapForward = -wallHit.normal;
            isGrab = false;
            return true;
        }
        
        return false;
    }

    private bool ValidateLedgeGrab() {
        Vector3 topPosition = groundSensor.transform.position;
        topPosition.y += groundSensor.Collider.bounds.extents.y;
        Ray sphereRay = new Ray(topPosition, Vector3.up);
        
        if (!Physics.Raycast(groundSensor.transform.position, Vector3.down, 1f, LayerMask.GetMask("Ground"))) {
            if (!Physics.SphereCast(sphereRay, bodySensor.Collider.bounds.extents.x / 2, 2f, LayerMask.GetMask("Walls"))) {
                return false;
            }
        }

        return true;
    }
}