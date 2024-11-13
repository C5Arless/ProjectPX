using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState, IContextInit {
    public GroundedState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base (currentContext, stateHandler, animHandler){
        IsRootState = true; //SOLO SU GROUNDED, AIRBORNE E DEAD (ROOT STATES)
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic
        if (Ctx.OnSlope) {
            GravityOff();
        } else {
            GravityOn();
        }

        if (!Ctx.IsAttacking && !Ctx.IsDashing && !Ctx.IsJumping && !Ctx.IsDamaged && !(Ctx.MoveInput == Vector2.zero)) {
            Ctx.IsWalking = true;
            Ctx.IsIdle = false;            
        } else if (!Ctx.IsAttacking && !Ctx.IsDashing && !Ctx.IsJumping && !Ctx.IsDamaged && (Ctx.MoveInput == Vector2.zero)) {
            Ctx.IsWalking = false;
            Ctx.IsIdle = true;
        }

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
    }
    public override void CheckSwitchStates() {
        //Switch logic
        if (!Ctx.IsGrounded) {
            SwitchState(StateHandler.Airborne());
        }
        else if (Ctx.IsDead) {
            SwitchState(StateHandler.Dead());
        }
    }   
    public void InitializeContext() {
        Ctx.IsFalling = false;
        Ctx.IsJumping = false;

        Ctx.StopCoroutine("InitializeMoveSpeed");
        Ctx.MoveSpeed = 1760;

        Ctx.AttackCount = 1;

        if (Ctx.PlayerInfo.PowerUps >= 1) {
            Ctx.JumpCount = 2;
        } else if (Ctx.PlayerInfo.PowerUps <= 0) {
            Ctx.JumpCount = 1;
        }
    }
}
