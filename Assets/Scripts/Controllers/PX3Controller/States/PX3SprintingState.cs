using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3SprintingState : PX3BaseState {
    private PhysicsInfo physicsData;
    private bool windUp;
    
    public PX3SprintingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {

        physicsData = Context.PhysicsData;
        InputHandler._onSprint += OnSprint;
    }

    public override void EnterState() {
        if (windUp) WindUpEnter();
        else LoopEnter();
    }

    public override void UpdateState() {
        if (!InputHandler.SprintInput) StateHandler.SetSubState(PX3SubStates.Running);
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {
        windUp = false;
    }

    public override void HandleSignal(int sig) {
        if (sig == 0) AnimHandler.PlayClip(PX3A_GroundSet.Sprint_Loop);
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
    }

    private void OnSprint() {
        if (!StateHandler.RootStates[PX3RootStates.Grounded]) return;
        
        if (!StateHandler.SubStates[PX3SubStates.Running]) {
            windUp = true;
        }
        
        StateHandler.SetSubState(PX3SubStates.Sprinting);
    }

    private void WindUpEnter() {
        AnimHandler.PlayClip(PX3A_GroundSet.Sprint_S);
    }

    private void LoopEnter() {
        AnimHandler.PlayClip(PX3A_GroundSet.Sprint_Loop);
    }
}