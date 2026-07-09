using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PX3BrakeState : PX3BaseState {
    private Vector3 previousVelocity;
    private float dampFactor;
    public PX3BrakeState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.Brake);
        previousVelocity = PhysicsHandler.PreviousHorizontalVelocity;
        Context.PhysicsData.Acceleration = .01f;
    }

    public override void UpdateState() {
        dampFactor = Context.PhysicsData.Acceleration / Context.PhysicsData.MaxAcceleration;
        
        if (dampFactor < 1f) {
            Context.PhysicsData.Acceleration += Context.PhysicsData.MaxAcceleration * (Time.deltaTime * Mathf.PI) ;
        } else Context.PhysicsData.Acceleration = Context.PhysicsData.MaxAcceleration;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 dampVelocity = previousVelocity * (1 - dampFactor);
        PhysicsHandler.UpdateMovement(dampVelocity, Context.PhysicsData.Acceleration);
        
        if (Context.PhysicsData.Acceleration >= Context.PhysicsData.MaxAcceleration) StateHandler.SetSubState(PX3SubStates.Idle);
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