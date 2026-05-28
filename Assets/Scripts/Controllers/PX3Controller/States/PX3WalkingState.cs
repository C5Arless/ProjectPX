using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3WalkingState : PX3BaseState {
    public PX3WalkingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
    }

    public override void EnterState() {

    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void LateUpdateState() {
        
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {

    }
}