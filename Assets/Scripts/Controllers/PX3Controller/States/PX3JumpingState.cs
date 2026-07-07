using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3JumpingState : PX3BaseState {
    public PX3JumpingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        InputHandler._onJump += OnJump;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_AirSet.Jump1);
    }

    public override void UpdateState() {
        
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }

    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        switch (sig) {
            case 0: {
                PhysicsHandler.ApplyImpulse(Vector3.up, Context.PhysicsData.JumpHeightCap);
                break;
            }
            case 1: {
                break;
            }
            case 2: {
                break;
            }
            case 4: {
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

    private void OnJump() {
        if (StateHandler.CurrentSubState == this) return;
        
        
        StateHandler.SetSubState(PX3SubStates.Jumping);
        //PhysicsHandler.Freeze();
    }
}