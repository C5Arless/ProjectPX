using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3CrouchingState : PX3BaseState {
    public PX3CrouchingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler) {
        
        InputHandler._onCrouch += OnCrouch;
    }

    public override void EnterState() {
        StartCoroutine(LerpCrouchAnimation());
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        AnimHandler.SetIRCBlend(0f);
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        //if (StateHandler.SubStates[PX3SubStates.Attacking]) {
        //    SwitchState(StateHandler.GetState(PX3SubStates.Attacking));
        //} //syntax test
    }

    private void OnCrouch() {
        if (StateHandler.CurrentSubState == this) return;
        
        if (StateHandler.RootStates[PX3RootStates.Grounded]) {
            StateHandler.SetSubState(PX3SubStates.Crouching);
        }
    }
    
    private IEnumerator LerpCrouchAnimation() {
        float value = 0f;

        while (value > -1f) {
            AnimHandler.SetIRCBlend(value);
            value -= .1f;
            yield return null;
        }

        value = -1f;
        AnimHandler.SetIRCBlend(value);
    }
}