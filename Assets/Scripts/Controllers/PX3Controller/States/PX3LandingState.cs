using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3LandingState : PX3BaseState {
    private PhysicsInfo physicsData;
    private PX3Sensor groundSensor;

    public PX3LandingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.Landing);
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
        if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
    }
}