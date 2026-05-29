using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3JumpingState : PX3BaseState {
    private int phase;
    
    public PX3JumpingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        InputHandler._onJump += OnJump;
    }

    public override void EnterState() {
        phase = 0;
        AnimHandler.PlayClip(PX3A_AirSet.Jump1);
        
    }

    public override void UpdateState() {
        if (phase == 2) PhysicsHandler.ApplyCustomGravity(10f);
        
        CheckSwitchStates();
    }
    
    public override void LateUpdateState() {
        
    }

    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        switch (sig) {
            case 0: {
                PhysicsHandler.SetVelocity(Vector3.up * 10f);
                phase = 1;
                break;
            }
            case 1: {
                phase = 2;
                break;
            }
            case 2: {
                ExitJump();
                break;
            }
        }

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
    }

    public void OnJump() {
        if (StateHandler.CurrentSubState == this) return;
        
        StateHandler.SetSubState(PX3SubStates.Jumping);
    }

    public void ExitJump() {
        if (StateHandler.RootStates[PX3RootStates.Airborne]) StateHandler.SetSubState(PX3SubStates.Falling);
    }
}