using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GroundedState : PX3BaseState {
    public PX3GroundedState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler) {
        
        IsRootState = true;

        SensorHandler.OnSensorsTrigger += OnGroundTrigger;
    }

    public override void EnterState() {
        
    }

    public override void UpdateState() {
        if (!InputHandler.CrouchInput) {
            if (InputHandler.MoveInput == Vector2.zero) {
                StateHandler.SetSubState(PX3SubStates.Idle);
            } else {
                StateHandler.SetSubState(PX3SubStates.Running);
            }
        }
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.RootStates[PX3RootStates.Airborne]) SwitchState(StateHandler.GetState(PX3RootStates.Airborne));
        else if (StateHandler.RootStates[PX3RootStates.Holding]) SwitchState(StateHandler.GetState(PX3RootStates.Holding));
        else if (StateHandler.RootStates[PX3RootStates.Dead]) SwitchState(StateHandler.GetState(PX3RootStates.Dead));
    }

    private void OnGroundTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (!other.CompareTag("Ground") && type != PX3SensorType.Ground) return;
        
        if (stage == PX3SensorStage.Enter) {
            StateHandler.SetRootState(PX3RootStates.Grounded);
        }
        else if (stage == PX3SensorStage.Exit) {
            StateHandler.SetRootState(PX3RootStates.Airborne);
        }
    }
}
