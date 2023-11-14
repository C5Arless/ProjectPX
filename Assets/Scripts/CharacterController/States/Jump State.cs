using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BaseState, IContextInit, IWalk {
    public JumpState(TPCharacterController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();
        
        Ctx.AnimHandler.Play(AnimHandler.Jump());

        HandleJump(Ctx.PlayerRb);
        Ctx.StartCoroutine("ResetJump");
    }
    public override void UpdateState() {
        //Update logic
        Ctx.IsGrounded = false;

        if (Ctx.MoveInput != Vector2.zero) {
            Ctx.Player.transform.forward = Ctx.PlayerForward.transform.forward;
        }
        
        HandleWalk();
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
    }
    public override void CheckSwitchStates() {
        //Switch logic
        
        if (Ctx.IsFalling) {
            SwitchState(StateHandler.Fall());
        }
        else if (Ctx.IsDamaged) {
            SwitchState(StateHandler.Damage());
        }        
        else if (Ctx.JumpInput && Ctx.CanJump && Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        }        
        else if (Ctx.IsDashing) {
            SwitchState(StateHandler.Dash());
        }
        else if (Ctx.IsAttacking) {
            SwitchState(StateHandler.Attack());
        }
        else if (Ctx.IsGrounded && Ctx.IsWalking) {
            SwitchState(StateHandler.Walk());
        }
        else if (Ctx.IsGrounded && Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }

    }
    public void InitializeContext() {
        Ctx.MoveSpeed = 1760;
        Ctx.Gravity = 9.81f;

        Ctx.JumpCount--;
        Ctx.JumpInput = false;
        Ctx.CanJump = false;
        Ctx.IsFalling = false;

        Ctx.IsDashing = false;
        Ctx.IsAttacking = false;
        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
    }
    private void HandleJump(Rigidbody rb) {
        //Jump Logic
        rb.velocity.Set(rb.velocity.x, -1f, rb.velocity.z);        
        rb.AddForce(Vector3.up * Ctx.JumpHeight * 3.14f, ForceMode.VelocityChange);
    }
    public void HandleWalk() {
        if (Direction() == Vector3.zero) { return; }
        Ctx.Asset.transform.forward = Direction();
        Ctx.PlayerRb.AddForce(Direction() * Ctx.MoveSpeed * Time.deltaTime, ForceMode.Force);
        SpeedControl();
    }
    private Vector3 Direction() {
        Vector3 direction = Ctx.Player.transform.forward * Ctx.MoveInput.y + Ctx.Player.transform.right * Ctx.MoveInput.x;
        return direction;
    }
    private void SpeedControl() {
        Vector3 flatvelocity = new Vector3(Ctx.PlayerRb.velocity.x, 0f, Ctx.PlayerRb.velocity.z);
        if (flatvelocity.magnitude > Ctx.MoveSpeed) {
            Vector3 limvelocity = flatvelocity.normalized * Ctx.MoveSpeed;
            Ctx.PlayerRb.velocity = new Vector3(limvelocity.x, Ctx.PlayerRb.velocity.y, limvelocity.z);
        }
    }
}
