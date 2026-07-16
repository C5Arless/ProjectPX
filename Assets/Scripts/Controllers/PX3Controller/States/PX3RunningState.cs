using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3RunningState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3RunningState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {

        physicsData = Context.PhysicsData;
        InputHandler._onMove += OnMove;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        physicsData.Acceleration = physicsData.MaxAcceleration * .25f;
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);

        if (physicsData.Acceleration < physicsData.MaxAcceleration) {
            physicsData.Acceleration += physicsData.MaxAcceleration * Time.deltaTime;
        } else physicsData.Acceleration = physicsData.MaxAcceleration;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = direction * physicsData.MaxSpeed;
        float dot = Vector3.Dot(PhysicsHandler.HorizontalVelocity, targetVelocity);

        if (direction != Vector3.zero) {
            Context.Asset.transform.forward = direction;
            
            if (dot < -.5f) {
                EvaluateBrake(targetVelocity);
            } else {
                PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);  
            }
        }
    }
    
    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        if (sig == 0) {
            
        }
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Thumbling]) SwitchState(StateHandler.GetState(PX3SubStates.Thumbling));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
        else if (StateHandler.SubStates[PX3SubStates.Brake]) SwitchState(StateHandler.GetState(PX3SubStates.Brake));
        else if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
    }

    private void OnMove() {
        if (!StateHandler.RootStates[PX3RootStates.Grounded]) return;
        
        if (!StateHandler.SubStates[PX3SubStates.Jumping] || !StateHandler.SubStates[PX3SubStates.Crouching]) {
            if (InputHandler.RawMoveInput.magnitude < 0.5f) StateHandler.SetSubState(PX3SubStates.Walking);
        }
    }

    private void EvaluateBrake(Vector3 fallbackVelocity) {
        if (physicsData.Acceleration >= physicsData.MaxAcceleration * .8f) {
            StateHandler.SetSubState(PX3SubStates.Brake);
        }
        else {
            physicsData.Acceleration = physicsData.MaxAcceleration * .25f;
            PhysicsHandler.UpdateMovement(fallbackVelocity, physicsData.Acceleration);
        }
    }
}