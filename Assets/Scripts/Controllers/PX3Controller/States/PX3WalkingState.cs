using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3WalkingState : PX3BaseState {
    private Vector3 direction;
    public PX3WalkingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        InputHandler._onMove += OnMove;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {

    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {

    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Thumbling]) SwitchState(StateHandler.GetState(PX3SubStates.Thumbling));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
    }
    
    private void OnMove() {
        if (InputHandler.MoveInput.magnitude >= 0.5f) StateHandler.SetSubState(PX3SubStates.Running);
    }
}