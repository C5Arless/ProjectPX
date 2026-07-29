using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3JumpingState : PX3BaseState {
    private int phase;
    private int mode;
    private bool isBusy;
    private PhysicsInfo physicsData;
    private PX3Sensor bodySensor;
    private PX3Sensor groundSensor;
    
    public PX3JumpingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {

        physicsData = Context.PhysicsData;
        InputHandler._onJump += OnJump;
    }

    public override void EnterState() {
        phase = 0;

        if (mode < 3) {
            physicsData.JumpCount--;
            
            if (physicsData.Acceleration <= .5f) {
                physicsData.Acceleration = physicsData.MaxAcceleration * .3f;
            }
        }
        
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
            case 3: {
                AnimHandler.PlayClip(PX3A_MixedSet.Ledge_Jump);
                break;
            }
        }
    }

    public override void UpdateState() {
        if (mode < 3) {
            if (InputHandler.MoveInput.magnitude > .2f) {
                if (physicsData.Acceleration > 0f) {
                    physicsData.Acceleration -= physicsData.MaxAcceleration * Time.deltaTime;
                } else physicsData.Acceleration = 0f;
            }
        }
        
        if (!isBusy) CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (mode < 3) {
            Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
            Vector3 targetVelocity = physicsData.MaxSpeed * .85f * direction;
            
            if (direction != Vector3.zero) {
                Context.Asset.transform.forward = direction;
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);  
            }
            
            switch (phase) {
                case 1: {
                    PhysicsHandler.UpdateGravity(physicsData.Gravity * .15f);
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
        else {
            if (phase == 1) {
                Vector3 targetVelocity = physicsData.MaxSpeed * .4f * Context.Asset.transform.forward;
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.MaxAcceleration); 
                PhysicsHandler.UpdateGravity(physicsData.Gravity);
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
            case 4: {
                PhysicsHandler.Unfreeze(false);
                break;
            }
            case 5: {
                SensorHandler.EnableCollisions(PX3SensorType.Body);
                break;
            }
            case 6: {
                phase = 1;
                PhysicsHandler.ApplyVerticalImpulse(physicsData.JumpHeight * .85f);
                break;
            }
            case 7: {
                isBusy = false;
                break;
            }
            case 8: {
                phase = 2;
                PhysicsHandler.ApplyStop();
                break;
            }
            case 9: {
                StateHandler.SetSubState(PX3SubStates.Idle);
                break;
            }
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Grabbing]) SwitchState(StateHandler.GetState(PX3SubStates.Grabbing));
        else if (StateHandler.SubStates[PX3SubStates.WallSliding]) SwitchState(StateHandler.GetState(PX3SubStates.WallSliding));
    }

    private void OnJump() {
        if (physicsData.JumpCount <= 0) return;

        bodySensor ??= SensorHandler.GetSensor(PX3SensorType.Body);
        groundSensor ??= SensorHandler.GetSensor(PX3SensorType.Ground);
        
        if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Thumbling)) mode = 1;
        else if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Crouching)) mode = 2;
        else if (StateHandler.CurrentSubState == StateHandler.GetState(PX3SubStates.Grabbing)) {
            isBusy = true;
            mode = 3;
        }
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