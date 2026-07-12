using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3CrouchingState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3CrouchingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        physicsData = Context.PhysicsData;
        InputHandler._onCrouch += OnCrouch;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        StartCoroutine(LerpCrouchAnimation(-1f));
    }

    public override void UpdateState() {
        if (!InputHandler.CrouchInput) StartCoroutine(ExitRoutine());
        
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {
        AnimHandler.SetIRCBlend(0f);
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
    }

    private void OnCrouch() {
        if (StateHandler.CurrentSubState == this) return;
        
        if (StateHandler.RootStates[PX3RootStates.Grounded]) {
            if (StateHandler.SubStates[PX3SubStates.Running]) {
                StateHandler.SetSubState(PX3SubStates.Thumbling);
            } else StateHandler.SetSubState(PX3SubStates.Crouching);
        }
    }
    
    private IEnumerator LerpCrouchAnimation(float target) {
        float value;
        
        if (target < 0) {
            value = 0f;
            
            while (value > -1f) {
                AnimHandler.SetIRCBlend(value);
                value -= .2f;
                yield return null;
            }

            value = -1f;
        }
        else {
            value = -1f;
            
            while (value < 0) {
                AnimHandler.SetIRCBlend(value);
                value += .2f;
                yield return null;
            }

            value = 0;
        }
        
        AnimHandler.SetIRCBlend(value);
    }

    private IEnumerator ExitRoutine() {
        yield return Context.StartCoroutine(LerpCrouchAnimation(0f));
        
        StateHandler.SetSubState(PX3SubStates.Idle);
    }
}