using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3WalkingState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3WalkingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
        InputHandler._onMove += OnMove;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        physicsData.Acceleration = 0;
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
        if (physicsData.Acceleration < physicsData.MaxAcceleration * .25f) {
            physicsData.Acceleration += physicsData.MaxAcceleration * Time.deltaTime;
        } else physicsData.Acceleration = physicsData.MaxAcceleration * .25f;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = direction * physicsData.MaxSpeed;
        
        if (direction != Vector3.zero) {
            Context.Asset.transform.forward = direction;
            PhysicsHandler.UpdateMovement(targetVelocity, physicsData.Acceleration);  
        }
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
        if (StateHandler.RootStates[PX3RootStates.Airborne]) return;
        
        if (!StateHandler.SubStates[PX3SubStates.Jumping] || !StateHandler.SubStates[PX3SubStates.Crouching]) {
            if (InputHandler.MoveInput.magnitude >= 0.5f) StateHandler.SetSubState(PX3SubStates.Running);
        }
    }
}