using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3RunningState : PX3BaseState {
    private Vector3 direction;
    private Vector3 input;
    
    public PX3RunningState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
        input = new Vector3(InputHandler.MoveInput.x, 0f, InputHandler.MoveInput.y);
        direction = new Vector3(Context.transform.forward.x * input.x, 0f, Context.transform.forward.z * input.z);
        PhysicsHandler.SetVelocity(Context.MaxSpeed * direction);
    }

    public override void UpdateState() {
        AnimHandler.SetIRCBlend(InputHandler.MoveInput.magnitude);
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
        input = new Vector3(InputHandler.MoveInput.x, 0f, InputHandler.MoveInput.y);
        direction = new Vector3(Context.transform.forward.x * input.x, 0f, Context.transform.forward.z * input.z);
        PhysicsHandler.SetVelocity(Context.MaxSpeed * direction);
    }
    
    public override void ExitState() {
        
        input = new Vector3(InputHandler.MoveInput.x, 0f, InputHandler.MoveInput.y);
        direction = new Vector3(Context.transform.forward.x * input.x, 0f, Context.transform.forward.z * input.z);
        PhysicsHandler.SetVelocity(Context.MaxSpeed * direction);
    }

    public override void HandleSignal(int sig) {
        
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