using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3DivingState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3DivingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
        InputHandler.OnCrouchAction += OnCrouch;
        SensorHandler.OnSensorsTrigger += OnLanding;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_MixedSet.Diving);
        StartCoroutine(DiveRoutine());
    }

    public override void UpdateState() {
        
        if (!Context.IsBusy) CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (Phase is > 0 and < 3) {
            Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
            Vector3 targetVelocity = physicsData.MaxSpeed * 1.25f * direction;
            
            if (direction != Vector3.zero) {
                Context.Asset.transform.forward = direction;
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);
            } else {
                targetVelocity = physicsData.MaxSpeed * 1.25f * Context.Asset.transform.forward;
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);
            }
        }
    }
    
    public override void ExitState() {
        Phase = 0;
        StopCoroutine(DiveRoutine());
        StopCoroutine(LandRoutine());
    }

    public override void HandleSignal(int sig) {
        switch (sig) {
            case 0: {
                //
                break;
            }
            case 1: {
                Phase = 1;
                break;
            }
            case 2: {
                Phase = 2;
                //AnimHandler.Stop();
                break;
            }
            case 3: {
                //Phase = 3;
                Context.IsBusy = false;
                //StateHandler.SetSubState(PX3SubStates.Idle);
                break;
            }
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
    }
    
    private void OnCrouch(PX3InputPhase phase) {
        if (phase != PX3InputPhase.started) return;
        if (StateHandler.CurrentSubState == this) return;
        
        if (StateHandler.RootStates[PX3RootStates.Airborne]) {
            StateHandler.SetSubState(PX3SubStates.Diving);
        }
    }

    private void OnLanding(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (type != PX3SensorType.Ground) return;
        if (stage != PX3SensorStage.Enter) return;
        if (StateHandler.CurrentSubState != this) return;
        
        StartCoroutine(LandRoutine());
    }

    private IEnumerator DiveRoutine() {
        //PhysicsHandler.ApplyStop();
        //PhysicsHandler.Freeze();
        Context.IsBusy = true;
        yield return null;
        
        //PhysicsHandler.Unfreeze(false);
        PhysicsHandler.ApplyVerticalImpulse(physicsData.JumpHeight * .85f);
        physicsData.Acceleration = physicsData.MaxAcceleration;

        yield return new WaitForSeconds(.25f);
        AnimHandler.Stop();
    }

    private IEnumerator LandRoutine() {
        PhysicsHandler.ApplyStop();
        PhysicsHandler.Freeze();
        AnimHandler.Resume();
        Phase = 3;

        while (Context.IsBusy) yield return null;
        PhysicsHandler.Unfreeze(false);
        StateHandler.SetSubState(PX3SubStates.Idle);
    }
    
}