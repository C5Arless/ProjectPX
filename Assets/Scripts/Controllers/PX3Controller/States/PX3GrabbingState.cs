using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GrabbingState : PX3BaseState {
    private bool isBusy;
    
    public PX3GrabbingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        InputHandler._onMove += OnMove;
    }

    public override void EnterState() {
        isBusy = true;
        AnimHandler.PlayClip(PX3A_MixedSet.Ledge_Grab);
    }

    public override void UpdateState() {
        
        if (!isBusy) CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        if (sig == 0) isBusy = false;

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.WallSliding]) SwitchState(StateHandler.GetState(PX3SubStates.WallSliding));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
    }

    private void OnMove() {
        if (!StateHandler.SubStates[PX3SubStates.Grabbing]) return;
        if (isBusy) return;

        if (InputHandler.MoveInput.y < -.9f) {
            SensorHandler.EnableCollisions(PX3SensorType.Body);
            PhysicsHandler.Unfreeze(false);
            StateHandler.SetRootState(PX3RootStates.Airborne);
            StateHandler.SetSubState(PX3SubStates.Falling);
        }
    }
}