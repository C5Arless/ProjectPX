using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3SprintingState : PX3BaseState {
    private PhysicsInfo physicsData;
    private bool windUp;
    private Vector3 direction;
    
    public PX3SprintingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {

        physicsData = Context.PhysicsData;
        InputHandler._onSprint += OnSprint;
    }

    public override void EnterState() {
        if (windUp) WindUpEnter();
        else LoopEnter();
    }

    public override void UpdateState() {
        if (!InputHandler.SprintInput) StateHandler.SetSubState(PX3SubStates.Running);
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (windUp) return;
        
        direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = physicsData.MaxSpeed * 1.5f * direction;
        float dot = Vector3.Dot(PhysicsHandler.HorizontalVelocity, targetVelocity);

        if (direction != Vector3.zero) {
            Context.Asset.transform.forward = direction;
            
            if (dot < -.5f) {
                StateHandler.SetSubState(PX3SubStates.Brake);
            } else {
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);  
            }
        }
    }
    
    public override void ExitState() {
        windUp = false;
    }

    public override void HandleSignal(int sig) {
        if (sig == 0) {
            windUp = false;
            AnimHandler.PlayClip(PX3A_GroundSet.Sprint_Loop);
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
    }

    private void OnSprint() {
        if (!StateHandler.RootStates[PX3RootStates.Grounded]) return;
        if (InputHandler.MoveInput == Vector2.zero) return;

        windUp = !StateHandler.SubStates[PX3SubStates.Running];
        
        StateHandler.SetSubState(PX3SubStates.Sprinting);
    }

    private void WindUpEnter() {
        AnimHandler.PlayClip(PX3A_GroundSet.Sprint_S);
    }

    private void LoopEnter() {
        AnimHandler.PlayClip(PX3A_GroundSet.Sprint_Loop);
    }
}