using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3CrouchingState : PX3BaseState {
    private PhysicsInfo physicsData;
    private Vector3 previousVelocity;
    private float dampFactor;
    
    public PX3CrouchingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        physicsData = Context.PhysicsData;
        InputHandler._onCrouch += OnCrouch;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        StartCoroutine(LerpCrouchAnimation(-1f));
        
        previousVelocity = PhysicsHandler.PreviousHorizontalVelocity;
        physicsData.Acceleration = .01f;
    }

    public override void UpdateState() {
        if (!InputHandler.CrouchInput) StartCoroutine(ExitRoutine());
        
        dampFactor = physicsData.Acceleration / physicsData.MaxAcceleration;
        
        if (dampFactor < 1f) {
            physicsData.Acceleration += physicsData.MaxAcceleration * (Time.deltaTime * Mathf.PI) ;
        } else physicsData.Acceleration = physicsData.MaxAcceleration;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 dampVelocity = previousVelocity * (1 - dampFactor);
        PhysicsHandler.UpdateMovement(dampVelocity, physicsData.Acceleration);
        
        //if (physicsData.Acceleration >= physicsData.MaxAcceleration) StateHandler.SetSubState(PX3SubStates.Idle);
    }
    
    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        //else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        //else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
    }

    private void OnCrouch() {
        if (StateHandler.CurrentSubState == this) return;
        
        if (StateHandler.RootStates[PX3RootStates.Grounded]) {
            StateHandler.SetSubState(PX3SubStates.Crouching);
        }
    }
    
    private IEnumerator LerpCrouchAnimation(float target) {
        float value;
        
        if (target < 0) {
            value = 0f;
            
            while (value > -1f) {
                AnimHandler.SetIRCBlend(value);
                value -= .3f;
                yield return null;
            }

            value = -1f;
        }
        else {
            value = -1f;
            
            while (value < 0) {
                AnimHandler.SetIRCBlend(value);
                value += .3f;
                yield return null;
            }

            value = 0;
        }
        
        AnimHandler.SetIRCBlend(value);
    }

    private IEnumerator ExitRoutine() {
        yield return Context.StartCoroutine(LerpCrouchAnimation(0f));
        
        StateHandler.SetSubState(PX3SubStates.Idle);
    }
}