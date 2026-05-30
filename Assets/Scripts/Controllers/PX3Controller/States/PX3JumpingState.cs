using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3JumpingState : PX3BaseState {
    private int phase;
    private float gravity;
    private int minHeight = 6;
    private int maxHeight = 12;
    private float holdMeter = 0f;
    
    public PX3JumpingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        InputHandler._onJump += OnJump;
    }

    public override void EnterState() {
        phase = 0;
        gravity = Context.MaxGravity / 5;
        holdMeter = 0f;
        
        AnimHandler.PlayClip(PX3A_AirSet.Jump1);
    }

    public override void UpdateState() {
        if (InputHandler.JumpInput && phase != 0 && phase < 3) {
            if (holdMeter < 1) holdMeter += .05f;
            else holdMeter = 1f;
        }
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (InputHandler.JumpInput && phase != 0 && phase < 3) {
            float diff = maxHeight - minHeight;
            float height = (1 - holdMeter) * diff / maxHeight;
            
            PhysicsHandler.AddVelocityChange(Vector3.up * height);
        }

        HandleGravity();
    }

    public override void ExitState() {
        gravity = Context.MaxGravity / 5;
        holdMeter = 0f;
        phase = 0;
    }

    public override void HandleSignal(int sig) {
        switch (sig) {
            case 0: {
                phase = 1;
                PhysicsHandler.Unfreeze(true);
                PhysicsHandler.AddVelocityChange(Vector3.up * minHeight);
                break;
            }
            case 1: {
                phase = 2;
                break;
            }
            case 2: {
                phase = 3;
                break;
            }
            case 3: {
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

    private void OnJump() {
        if (StateHandler.CurrentSubState == this) return;
        
        StateHandler.SetSubState(PX3SubStates.Jumping);
        PhysicsHandler.Freeze();
    }

    private void ExitJump() {
        if (StateHandler.RootStates[PX3RootStates.Airborne]) StateHandler.SetSubState(PX3SubStates.Falling);
    }

    private void HandleGravity() {
        switch (phase) {
            case 1: {
                if (gravity < Context.MaxGravity) {
                    gravity += Context.MaxGravity / 5;
                    PhysicsHandler.ApplyCustomGravity(gravity);
                } else PhysicsHandler.ApplyCustomGravity(Context.MaxGravity);

                break;
            }
            case 2: {
                PhysicsHandler.ApplyCustomGravity(Context.MaxGravity * 2);
                break;
            }
            case 3: {
                PhysicsHandler.ApplyCustomGravity(Context.MaxGravity / 2);
                break;
            }
        }
    }
}