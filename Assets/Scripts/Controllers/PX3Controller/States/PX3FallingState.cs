using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3FallingState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3FallingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_AirSet.Falling);
    }

    public override void UpdateState() {
        if (InputHandler.MoveInput.magnitude > .2f) {
            if (physicsData.Acceleration > 0f) {
                physicsData.Acceleration -= physicsData.MaxAcceleration * Time.deltaTime;
            } else physicsData.Acceleration = 0f;
        }
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        Vector3 direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Vector3 targetVelocity = physicsData.MaxSpeed * .85f * direction;
        
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
        if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
        else if (StateHandler.SubStates[PX3SubStates.Grabbing]) SwitchState(StateHandler.GetState(PX3SubStates.Grabbing));
        else if (StateHandler.SubStates[PX3SubStates.WallSliding]) SwitchState(StateHandler.GetState(PX3SubStates.WallSliding));
    }
    
}