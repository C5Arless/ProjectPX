using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3GroundedState : PX3BaseState {
    private PhysicsInfo physicsData;
    public PX3GroundedState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        IsRootState = true;
        physicsData = Context.PhysicsData;
        SensorHandler.OnSensorsTrigger += OnGroundTrigger;
    }

    public override void EnterState() {
        physicsData.JumpCount = 2;
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
        //CheckMove();
        //CheckSprint();
    }

    public override void FixedUpdateState() {
        PhysicsHandler.UpdateGravity(physicsData.Gravity * .05f);
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
        
        if (stage == PX3SensorStage.Exit) {
            StateHandler.SetRootState(PX3RootStates.Airborne);
        }
    }

    private void CheckSprint() {
        if (InputHandler.SprintInput && InputHandler.MoveInput != Vector2.zero) {
            if (StateHandler.SubStates[PX3SubStates.Idle] || StateHandler.SubStates[PX3SubStates.Running]) {
                StateHandler.SetSubState(PX3SubStates.Sprinting);
            }
        }
    }

    private void CheckMove() {
        if (InputHandler.JumpInput || StateHandler.SubStates[PX3SubStates.Jumping]) return;
        
        if (!InputHandler.CrouchInput || StateHandler.SubStates[PX3SubStates.Falling]) {
            if (InputHandler.MoveInput == Vector2.zero) {
                StateHandler.SetSubState(PX3SubStates.Idle);
            } else {
                //if (InputHandler.MoveInput.magnitude < 0.5f) StateHandler.SetSubState(PX3SubStates.Walking);
                //else if (InputHandler.MoveInput.magnitude >= 0.5f) StateHandler.SetSubState(PX3SubStates.Running);
            }
        }
    }
}
