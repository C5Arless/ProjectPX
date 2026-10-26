using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GrabbingState : PX3BaseState {
    private PX3Sensor groundSensor;
    
    public PX3GrabbingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //InputHandler._onMove += OnMove;
        InputHandler.OnMoveAction += OnMove;
    }

    public override void EnterState() {
        Context.IsBusy = true;
        AnimHandler.PlayClip(PX3A_MixedSet.Ledge_Grab);
    }

    public override void UpdateState() {
        
        if (!Context.IsBusy) CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        if (sig == 0) Context.IsBusy = false;

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.WallSliding]) SwitchState(StateHandler.GetState(PX3SubStates.WallSliding));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
    }

    private void OnMove(PX3InputPhase phase) {
        if (phase != PX3InputPhase.started) return;
        
        if (!StateHandler.SubStates[PX3SubStates.Grabbing]) return;
        if (Context.IsBusy) return;

        if (InputHandler.MoveInput.y < -.9f) {
            groundSensor ??= SensorHandler.GetSensor(PX3SensorType.Ground);
            
            if (EvaluateWallHit()) {
                PhysicsHandler.Unfreeze(false);
                StateHandler.GetState(PX3SubStates.WallSliding).Mode = 1;
                StateHandler.SetSubState(PX3SubStates.WallSliding);
            }
            else {
                SensorHandler.EnableCollisions(PX3SensorType.Body);
                PhysicsHandler.Unfreeze(false);
                StateHandler.SetRootState(PX3RootStates.Airborne);
                StateHandler.SetSubState(PX3SubStates.Falling);
            }
        }
    }

    private bool EvaluateWallHit() {
        if (Physics.Raycast(groundSensor.transform.position, Context.Asset.transform.forward, 1f, LayerMask.GetMask("Walls"))) {
            return true;
        }

        return false;
    }
}