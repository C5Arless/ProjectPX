using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3HoldingState : PX3BaseState {
    public PX3HoldingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        IsRootState = true;
        SensorHandler.OnSensorsCollision += OnLedgeCollision;
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

    private void OnLedgeCollision(Collision collision, PX3SensorType type, PX3SensorStage stage) {
        if (stage == PX3SensorStage.Exit) return;
        if (type != PX3SensorType.Ledge) return;
        if (StateHandler.RootStates[PX3RootStates.Holding]) return;
        
        
        //if (StateHandler.RootStates[PX3RootStates.Grounded]) return;
        //if (StateHandler.SubStates[PX3SubStates.WallSliding] ||
        //    StateHandler.SubStates[PX3SubStates.Grabbing]) return;
        
        if (stage == PX3SensorStage.Enter) {
            SensorHandler.DisableCollisions(PX3SensorType.Body);
            SensorHandler.DisableCollisions(PX3SensorType.Ledge);
            PhysicsHandler.Freeze();
            Vector3 contactPoint = collision.contacts[0].point;
            Bounds bodyBounds = SensorHandler.GetSensor(PX3SensorType.Body).Collider.bounds;
            Bounds ledgeBounds = SensorHandler.GetSensor(PX3SensorType.Ledge).Collider.bounds;
            float targetY = contactPoint.y - (bodyBounds.size.y / 2) - (ledgeBounds.size.y / 2);
            Vector3 freezePosition = new Vector3(Context.transform.position.x, targetY, Context.transform.position.z);
            Context.transform.position = freezePosition;
            StateHandler.SetRootState(PX3RootStates.Holding);
            StateHandler.SetSubState(PX3SubStates.Grabbing);
            
            //Debug.Log(other.tag);
            
            //if (other.CompareTag("Ground")) StateHandler.SetSubState(PX3SubStates.Grabbing);
            //else if (other.CompareTag("Wall")) StateHandler.SetSubState(PX3SubStates.WallSliding);
        }
    }
}