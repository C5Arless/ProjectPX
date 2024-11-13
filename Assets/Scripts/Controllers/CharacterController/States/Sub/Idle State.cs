using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState, IContextInit {
    public IdleState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();
        Ctx.AnimHandler.SetAlt(false);
        Ctx.AnimHandler.PlayDirect(AnimHandler.Idle());
    }
    public override void UpdateState() {
        //Update logic
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic

    }
    public override void CheckSwitchStates() {
        //Switch logic

        if (Ctx.IsWalking) {
            SwitchState(StateHandler.Walk());
        }
        else if (Ctx.IsAttacking) {
            SwitchState(StateHandler.Attack());
        }
        else if (Ctx.IsDashing) {
            SwitchState(StateHandler.Dash());
        }
        else if (Ctx.CanJump && Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        }
        else if (Ctx.IsDamaged) {
            SwitchState(StateHandler.Damage());
        }
        else if (Ctx.IsFalling && !Ctx.IsGrounded) {
            //Ctx.JumpCount--;
            SwitchState(StateHandler.Fall());
        }
    }
    public void InitializeContext() {
        //Set Context Vars

    }
}
