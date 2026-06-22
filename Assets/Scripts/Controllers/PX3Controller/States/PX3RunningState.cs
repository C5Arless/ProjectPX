using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3RunningState : PX3BaseState {
    private float acceleration;
    
    public PX3RunningState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        InputHandler._onMove += OnMove;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        acceleration = 0;
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);

        if (acceleration < Context.PhysicsData.MaxAcceleration) {
            acceleration += Context.PhysicsData.MaxAcceleration * Time.deltaTime;
        } else acceleration = Context.PhysicsData.MaxAcceleration;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = direction * Context.PhysicsData.MaxSpeed;
        float dot = Vector3.Dot(targetVelocity, PhysicsHandler.CurrentVelocity);
        
        Context.Asset.transform.forward = direction;
        
        if (dot < -.5f) {
            SwitchState(StateHandler.GetState(PX3SubStates.Brake));
        } else if (dot < .5f) {
            //curve
        } else {
            PhysicsHandler.UpdateMovement(targetVelocity, acceleration);  
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
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
    }

    private void OnMove() {
        if (InputHandler.RawMoveInput.magnitude < 0.5f) StateHandler.SetSubState(PX3SubStates.Walking);
    }
}