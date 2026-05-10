using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3IdleState : PX3BaseState {
    public PX3IdleState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler) {
        
        //
    }

    public override void EnterState() {
        throw new System.NotImplementedException();
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        throw new System.NotImplementedException();
    }

    public override void HandleSignal(int sig) {
        throw new System.NotImplementedException();
    }

    public override void CheckSwitchStates() {
        throw new System.NotImplementedException();
    }
}