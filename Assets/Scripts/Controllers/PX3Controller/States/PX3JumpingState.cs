using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3JumpingState : PX3BaseState {
    private int phase;
    private int mode;
    private PhysicsInfo physicsData;
    
    public PX3JumpingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {

        physicsData = Context.PhysicsData;
        InputHandler._onJump += OnJump;
    }

    public override void EnterState() {
        physicsData.JumpCount--;
        phase = 0;
        
        switch (mode) {
            case 0: {
                AnimHandler.PlayClip(PX3A_AirSet.Jump1);
                break;
            }
            case 1: {
                AnimHandler.PlayClip(PX3A_AirSet.Jump2);
                break;
            }
            case 2: {
                AnimHandler.PlayClip(PX3A_AirSet.Jump3);
                break;
            }
        }
        
        if (physicsData.Acceleration <= .5f) {
            physicsData.Acceleration = physicsData.MaxAcceleration * .3f;
        }
    }

    public override void UpdateState() {
        if (InputHandler.MoveInput.magnitude > .2f) {
            if (physicsData.Acceleration > 0f) {
                physicsData.Acceleration -= physicsData.MaxAcceleration * Time.deltaTime;
            } else physicsData.Acceleration = 0f;
        }
        
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = physicsData.MaxSpeed * .85f * direction;
        
        if (direction != Vector3.zero) {
            Context.Asset.transform.forward = direction;
            PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);  
        }
        
        switch (phase) {
            case 1: {
                PhysicsHandler.UpdateGravity(1f);
                break;
            }
            case 2: {
                PhysicsHandler.UpdateGravity(physicsData.Gravity / 2);
                break;
            }
            case 3: {
                PhysicsHandler.UpdateGravity(physicsData.Gravity * 2);
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
                HandleJump();
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
        if (physicsData.JumpCount <= 0) return;
        
        if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Thumbling)) mode = 1;
        else if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Crouching)) mode = 2;
        else if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Grabbing)) mode = 3;
        else mode = 0;
        
        StateHandler.SetSubState(PX3SubStates.Jumping);
    }

    private void HandleJump() {
        StateHandler.SetRootState(PX3RootStates.Airborne);

        switch (mode) {
            case 0: {
                PhysicsHandler.ApplyVerticalImpulse(physicsData.JumpHeight);
                break;
            }
            case 1: {
                PhysicsHandler.ApplyVerticalImpulse(physicsData.JumpHeight * 1.1f);
                break;
            }
            case 2: {
                PhysicsHandler.ApplyVerticalImpulse(physicsData.JumpHeight * 1.25f);
                break;
            }
        }
    }
}