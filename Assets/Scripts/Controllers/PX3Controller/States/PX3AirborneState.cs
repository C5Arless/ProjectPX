using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PX3AirborneState : PX3BaseState {
    private PhysicsInfo physicsData;
    public PX3AirborneState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        physicsData = Context.PhysicsData;
        IsRootState = true;
        SensorHandler.OnSensorsTrigger += OnGroundTrigger;
    }

    public override void EnterState() {
        //SensorHandler.EnableCollisions(PX3SensorType.Ledge);
    }

    public override void UpdateState() {
        if (!StateHandler.SubStates[PX3SubStates.Falling] && StateHandler.RootStates[PX3RootStates.Airborne]) {
            if (StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Jumping) &&
                StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Attacking) &&
                StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Dashing) &&
                StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Damaged) &&
                StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Diving) &&
                StateHandler.CurrentSubState != StateHandler.GetState(PX3SubStates.Grabbing)) 
            {
                
                StateHandler.SetSubState(PX3SubStates.Falling);
            }
        }
        
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (!StateHandler.SubStates[PX3SubStates.Jumping]) {
            PhysicsHandler.UpdateGravity(physicsData.Gravity);
        }
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {
        if (StateHandler.RootStates[PX3RootStates.Grounded]) SwitchState(StateHandler.GetState(PX3RootStates.Grounded));
        else if (StateHandler.RootStates[PX3RootStates.Mixed]) SwitchState(StateHandler.GetState(PX3RootStates.Mixed));
        else if (StateHandler.RootStates[PX3RootStates.Dead]) SwitchState(StateHandler.GetState(PX3RootStates.Dead));
    }
    
    private void OnGroundTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (StateHandler.RootStates[PX3RootStates.Mixed]) return;
        if (!other.CompareTag("Ground") && type != PX3SensorType.Ground) return;
        
        if (stage == PX3SensorStage.Enter) {
            StateHandler.SetRootState(PX3RootStates.Grounded);
        }

        if (!StateHandler.SubStates[PX3SubStates.Jumping]) {
            if (stage == PX3SensorStage.Stay) {
                StateHandler.SetRootState(PX3RootStates.Grounded);
            }
        }
    }
}