using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3AirborneState : PX3BaseState {
    public PX3AirborneState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        IsRootState = true;
    }

    public override void EnterState() {

    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        //PhysicsHandler.ApplyCustomGravity(9.81f);
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {
        if (StateHandler.RootStates[PX3RootStates.Grounded]) SwitchState(StateHandler.GetState(PX3RootStates.Grounded));
        else if (StateHandler.RootStates[PX3RootStates.Holding]) SwitchState(StateHandler.GetState(PX3RootStates.Holding));
        else if (StateHandler.RootStates[PX3RootStates.Dead]) SwitchState(StateHandler.GetState(PX3RootStates.Dead));
    }
}