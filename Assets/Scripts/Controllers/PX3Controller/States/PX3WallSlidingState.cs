using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3WallSlidingState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3WallSlidingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_MixedSet.WallSlide);
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        PhysicsHandler.UpdateGravity(physicsData.Gravity * .25f);
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
    }
}