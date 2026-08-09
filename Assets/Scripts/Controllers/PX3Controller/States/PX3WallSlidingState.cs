using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3WallSlidingState : PX3BaseState {
    private PhysicsInfo physicsData;
    private PX3Sensor groundSensor;

    public PX3WallSlidingState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, PX3PhysicsHandler physicsHandler) : 
        base (currentContext, stateHandler, animHandler, inputHandler, sensorHandler, physicsHandler) {
        
        //
        physicsData = Context.PhysicsData;
    }

    public override void EnterState() {
        groundSensor ??= SensorHandler.GetSensor(PX3SensorType.Ground);
        
        Phase = 0;
        PhysicsHandler.Unfreeze(false);

        if (Mode == 1) {
            AnimHandler.PlayClip(PX3A_MixedSet.WallSlide_Loop);
        } else {
            PhysicsHandler.ApplyStop();
            AnimHandler.PlayClip(PX3A_MixedSet.WallSlide);
        }
    }

    public override void UpdateState() {
        if (EvaluateWallSlide(out PX3SubStates state)) {
            if (state == PX3SubStates.Idle) {
                SensorHandler.EnableCollisions(PX3SensorType.Body);
                PhysicsHandler.ApplyStop();
                StateHandler.SetRootState(PX3RootStates.Grounded);
                StateHandler.GetState(PX3SubStates.Idle).Mode = 1;
                StateHandler.SetSubState(PX3SubStates.Idle);
            } else {
                SensorHandler.EnableCollisions(PX3SensorType.Body);
                StateHandler.SetRootState(PX3RootStates.Airborne);
                StateHandler.SetSubState(PX3SubStates.Falling);
            }
        }
        
        CheckSwitchStates();
    }
    
    public override void FixedUpdateState() {
        if (Mode == 1) {
            PhysicsHandler.UpdateGravity(physicsData.Gravity * .15f);
        }
        else {
            if (Phase < 1) PhysicsHandler.UpdateGravity(physicsData.Gravity * .85f);
            else PhysicsHandler.UpdateGravity(physicsData.Gravity * .15f);
        }
    }
    
    public override void ExitState() {
        Phase = 0;
        Mode = 0;
    }

    public override void HandleSignal(int sig) {
        if (sig == 0) {
            PhysicsHandler.ApplyStop();
            Phase = 1;
        }
        if (sig == 1) {
            AnimHandler.PlayClip(PX3A_MixedSet.WallSlide_Loop);
            Mode = 0;
        }
    }

    public override void CheckSwitchStates() {
        if (StateHandler.SubStates[PX3SubStates.Falling]) SwitchState(StateHandler.GetState(PX3SubStates.Falling));
        else if (StateHandler.SubStates[PX3SubStates.Idle]) SwitchState(StateHandler.GetState(PX3SubStates.Idle));
        else if (StateHandler.SubStates[PX3SubStates.Jumping]) SwitchState(StateHandler.GetState(PX3SubStates.Jumping));
    }

    private bool EvaluateWallSlide(out PX3SubStates state) {
        state = PX3SubStates.Idle;

        if (Physics.Raycast(groundSensor.transform.position, Context.Asset.transform.forward, 1f,
                LayerMask.GetMask("Walls"))) {

            if (Physics.Raycast(groundSensor.transform.position, Vector3.down, 1f,
                    LayerMask.GetMask("Ground"))) {
                state = PX3SubStates.Idle;
                return true;
            }
            
            return false;
        }
        
        state = PX3SubStates.Falling;
        return true;
    }
}