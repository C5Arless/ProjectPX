using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GrabbingState : PX3BaseState {
    private PX3Sensor groundSensor;
    private PX3Sensor bodySensor;
    private PX3Sensor ledgeSensor;
    
    public PX3GrabbingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        SensorHandler.OnSensorsTrigger += OnLedgeTrigger;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_MixedSet.Ledge_Grab);
        // PhysicsHandler.Freeze();
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.WallSliding]) SwitchState(StateHandler.GetState(PX3SubStates.WallSliding));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
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
            
            if (RetrieveSnapPoints(out Vector3 snapPosition, out Vector3 snapForward)) {
                SensorHandler.DisableCollisions(PX3SensorType.Body);
                PhysicsHandler.Freeze();
                
                Context.transform.position = snapPosition;
                Context.Asset.transform.forward = snapForward;
                
                StateHandler.SetRootState(PX3RootStates.Mixed);
                StateHandler.SetSubState(PX3SubStates.Grabbing);
            }
        }
    }

    private bool RetrieveSnapPoints(out Vector3 snapPosition, out Vector3 snapForward) {
        snapPosition = Vector3.zero;
        snapForward = Vector3.zero;
        
        if (Physics.Raycast(bodySensor.transform.position, Context.Asset.transform.forward, out RaycastHit wallHit, 1f, LayerMask.GetMask("Walls"))) {
            Vector3 ledgePoint = wallHit.point - (wallHit.normal * 0.2f);
            ledgePoint.y += bodySensor.Collider.bounds.extents.y;

            if (Physics.Raycast(ledgePoint, Vector3.down, out RaycastHit groundHit, 1f, LayerMask.GetMask("Ground"))) {
                float wallOffset = ((new Vector2(bodySensor.Collider.bounds.center.x, bodySensor.Collider.bounds.center.z)) -
                                   (new Vector2(ledgeSensor.Collider.bounds.center.x, ledgeSensor.Collider.bounds.center.z))).magnitude;
                snapPosition = wallHit.point + (wallHit.normal * wallOffset);
                snapPosition.y = groundHit.point.y - bodySensor.Collider.bounds.extents.y;
                snapForward = -wallHit.normal;

                return true;
            }
        }
        
        return false;
    }

    private bool ValidateLedgeGrab() {
        if (Physics.Raycast(groundSensor.transform.position, Vector3.down, out RaycastHit hit, 1f, LayerMask.GetMask("Ground"))) {
            return true;
        } return false;
    }
}