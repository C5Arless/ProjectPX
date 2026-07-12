using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3IdleState : PX3BaseState {
    private PhysicsInfo physicsData;
    
    public PX3IdleState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        physicsData = Context.PhysicsData;
        SensorHandler.OnSensorsTrigger += OnGroundTrigger;
    }

    public override void EnterState() {
        AnimHandler.PlayClip(PX3A_GroundSet.IRC);
        AnimHandler.SetIRCBlend(0);

        PhysicsHandler.ApplyStop();
    }

    public override void UpdateState() {
        if (physicsData.Acceleration > 0f) {
            physicsData.Acceleration -= physicsData.MaxAcceleration * Time.deltaTime;
        } else physicsData.Acceleration = 0f;
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        
    }
    
    public override void ExitState() {
        
    }

    public override void HandleSignal(int sig) {
        
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
        else if (StateHandler.SubStates[PX3SubStates.Crouching]) SwitchState(StateHandler.GetState(PX3SubStates.Crouching));
        else if (StateHandler.SubStates[PX3SubStates.Walking]) SwitchState(StateHandler.GetState(PX3SubStates.Walking));
        else if (StateHandler.SubStates[PX3SubStates.Running]) SwitchState(StateHandler.GetState(PX3SubStates.Running));
        else if (StateHandler.SubStates[PX3SubStates.Sprinting]) SwitchState(StateHandler.GetState(PX3SubStates.Sprinting));
    }
    
    private void OnGroundTrigger(Collider other, PX3SensorType type, PX3SensorStage stage) {
        if (!StateHandler.RootStates[PX3RootStates.Grounded]) return;
        if (InputHandler.MoveInput != Vector2.zero) return;
        
        if (StateHandler.SubStates[PX3SubStates.Jumping] ||
            StateHandler.SubStates[PX3SubStates.Crouching]) return;
        
        if (!other.CompareTag("Ground") && type != PX3SensorType.Ground) return;
        
        if (stage == PX3SensorStage.Stay) {
            StateHandler.SetSubState(PX3SubStates.Idle);
        }
    }
}