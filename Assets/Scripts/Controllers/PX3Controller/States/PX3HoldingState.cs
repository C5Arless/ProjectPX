using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3HoldingState : PX3BaseState {
    public PX3HoldingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler) {
        
        IsRootState = true;
        SensorHandler.OnSensorsTrigger += OnLedgeTrigger;
    }

    public override void EnterState() {

    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.RootStates[PX3RootStates.Airborne]) SwitchState(StateHandler.GetState(PX3RootStates.Airborne));
        else if (StateHandler.RootStates[PX3RootStates.Grounded]) SwitchState(StateHandler.GetState(PX3RootStates.Grounded));
        else if (StateHandler.RootStates[PX3RootStates.Dead]) SwitchState(StateHandler.GetState(PX3RootStates.Dead));
    }

    public void OnLedgeTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (StateHandler.RootStates[PX3RootStates.Grounded]) return;
        
        if (stage == PX3SensorStage.Enter) {
            StateHandler.SetRootState(PX3RootStates.Holding);
            
            if (other.CompareTag("Wall")) StateHandler.SetSubState(PX3SubStates.WallSliding);
            else if (other.CompareTag("Ground")) StateHandler.SetSubState(PX3SubStates.Grabbing);
        }
        else if (stage == PX3SensorStage.Exit) {
            StateHandler.SetRootState(PX3RootStates.Airborne);
        }
    }
}