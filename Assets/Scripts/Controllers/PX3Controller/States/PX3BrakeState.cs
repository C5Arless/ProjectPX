using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3BrakeState : PX3BaseState {
    private Vector3 previousVelocity;
    private float dampFactor;
    private PhysicsInfo physicsData;
    
    public PX3BrakeState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.Brake);
        previousVelocity = PhysicsHandler.PreviousHorizontalVelocity;
        physicsData.Acceleration = .01f;
    }

    public override void UpdateState() {
        dampFactor = physicsData.Acceleration / physicsData.MaxAcceleration;
        
        if (dampFactor < 1f) {
            physicsData.Acceleration += physicsData.MaxAcceleration * (Time.deltaTime * Mathf.PI) ;
        } else physicsData.Acceleration = physicsData.MaxAcceleration;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 dampVelocity = previousVelocity * (1 - dampFactor);
        PhysicsHandler.UpdateMovement(dampVelocity, physicsData.Acceleration);
        
        if (physicsData.Acceleration >= physicsData.MaxAcceleration) StateHandler.SetSubState(PX3SubStates.Idle);
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