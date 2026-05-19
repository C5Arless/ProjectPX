using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3CrouchingState : PX3BaseState {
    public PX3CrouchingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler) {
        
        //
    }

    public override void EnterState() {
        
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        //if (StateHandler.SubStates[PX3SubStates.Attacking]) {
        //    StateHandler.SetSubState(PX3SubStates.Attacking);
        //} //syntax test
    }
}