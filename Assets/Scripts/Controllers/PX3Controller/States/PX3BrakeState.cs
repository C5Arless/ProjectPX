using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3BrakeState : PX3BaseState {
    public PX3BrakeState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.Brake);

    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        PhysicsHandler.UpdateMovement(Vector3.zero, Context.PhysicsData.MaxAcceleration * Time.deltaTime);
        
        if (PhysicsHandler.HorizontalVelocity == Vector3.zero) StateHandler.SetSubState(PX3SubStates.Idle);
    }
    
    public override void ExitState() {

    }

    public override void HandleSignal(int sig) {
        if (sig == 0) {
            //StateHandler.SetSubState(PX3SubStates.Idle);
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));

    }
}