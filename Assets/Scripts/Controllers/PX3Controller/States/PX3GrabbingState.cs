using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GrabbingState : PX3BaseState {
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

    }

    private void OnLedgeTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (stage == PX3SensorStage.Exit) return;
        if (type != PX3SensorType.Ledge) return;
        if (!other.CompareTag("Ground") && !other.CompareTag("Wall")) return;
        if (StateHandler.RootStates[PX3RootStates.Mixed]) return;
   
        if (stage == PX3SensorStage.Enter) {
            bodySensor ??= SensorHandler.GetSensor(PX3SensorType.Body);
            ledgeSensor ??= SensorHandler.GetSensor(PX3SensorType.Ledge);

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
        
        if (Physics.Raycast(bodySensor.transform.position, Context.Asset.transform.forward, out RaycastHit wallHit, 5f, LayerMask.GetMask("Walls"))) {
            Vector3 ledgePoint = wallHit.point - (wallHit.normal * 0.2f);
            ledgePoint.y += bodySensor.Collider.bounds.extents.y;

            if (Physics.Raycast(ledgePoint, Vector3.down, out RaycastHit groundHit, 5f, LayerMask.GetMask("Ground"))) {
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
}