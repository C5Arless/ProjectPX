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

        if (Context.PhysicsData.Acceleration <= .5f) {
            Context.PhysicsData.Acceleration = Context.PhysicsData.MaxAcceleration * .3f;
        }
    }

    public override void UpdateState() {
        if (InputHandler.MoveInput.magnitude > .2f) {
            if (Context.PhysicsData.Acceleration > 0f) {
                Context.PhysicsData.Acceleration -= Context.PhysicsData.MaxAcceleration * Time.deltaTime;
            } else Context.PhysicsData.Acceleration = 0f;
        }
        
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = Context.PhysicsData.MaxSpeed * .85f * direction;
        
        if (direction != Vector3.zero) {
            Context.Asset.transform.forward = direction;
            PhysicsHandler.UpdateMovement(targetVelocity, Context.PhysicsData.Acceleration);  
        }
        
        switch (phase) {
            case 1: {
                PhysicsHandler.UpdateGravity(.2f);
                break;
            }
            case 2: {
                PhysicsHandler.UpdateGravity(Context.PhysicsData.Gravity / 2);
                break;
            }
            case 3: {
                PhysicsHandler.UpdateGravity(Context.PhysicsData.Gravity * 2);
                break;
            }
        }
    }

    public override void ExitState() {
        phase = 0;
    }

    public override void HandleSignal(int sig) {
        switch (sig) {
            case 0: {
                phase = 1;
                StateHandler.SetRootState(PX3RootStates.Airborne);
                PhysicsHandler.ApplyVerticalImpulse(Context.PhysicsData.JumpHeightCap);
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
                StateHandler.SetSubState(PX3SubStates.Falling);
                break;
            }
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
    }

    private void OnJump() {
        if (StateHandler.CurrentSubState == this) return;
        
        StateHandler.SetSubState(PX3SubStates.Jumping);
    }

}