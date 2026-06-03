using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3RunningState : PX3BaseState {
    private Vector3 direction;
    private float acceleration;
    
    public PX3RunningState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        acceleration = Context.MaxSpeed / (InputHandler.MoveInput.magnitude * 2);
        
        direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Context.Asset.transform.forward = direction;
        //PhysicsHandler.SetVelocity(InputHandler.MoveInput.magnitude * Context.MaxSpeed * direction);
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
        direction = Context.Forward.transform.forward * InputHandler.MoveInput.y + Context.Forward.transform.right * InputHandler.MoveInput.x;
        Context.Asset.transform.forward = direction;
        
        PhysicsHandler.ApplyLinearMovement(direction, Context.MaxSpeed, acceleration);
    }
    
    public override void ExitState() {
        
        //Context.Asset.transform.forward = direction;
        Vector3 exitVelocity = new Vector3(0f, PhysicsHandler.PreviousVelocity.y, 0f);
        PhysicsHandler.SetVelocity(exitVelocity);
    }

    public override void HandleSignal(int sig) {
        if (sig == 0) {
            PhysicsHandler.AddVelocityChange(direction * Context.MaxSpeed * 0.1f);
        }
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Thumbling]) SwitchState(StateHandler.GetState(PX3SubStates.Thumbling));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
    }
}