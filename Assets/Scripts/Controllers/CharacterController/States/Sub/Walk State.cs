using UnityEngine;

public class WalkState : BaseState, IContextInit, IWalk {
    public WalkState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();

        if (Ctx.IsGrounded) {       
            Ctx.AnimHandler.Play(AnimHandler.Walk());
        }
    }
    public override void UpdateState() {
        //Update logic

        /*
        if (Ctx.MoveInput != Vector2.zero) {
            Ctx.Player.transform.forward = Ctx.PlayerForward.transform.forward;
        }
        */

        HandleWalk();

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic

    }
    public override void CheckSwitchStates() {
        //Switch logic

        if (Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        } 
        else if (Ctx.IsAttacking) {
            SwitchState(StateHandler.Attack());
        } 
        else if (Ctx.IsDashing) {
            SwitchState(StateHandler.Dash());
        } 
        else if (Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        } 
        else if (Ctx.IsFalling && !Ctx.IsGrounded) {
            Ctx.JumpCount--;
            SwitchState(StateHandler.Fall());
        } 
        else if (Ctx.IsDamaged) {
            SwitchState(StateHandler.Damage());
        }
    }
    public void InitializeContext() {
        //Set Context Vars
    }
    public void HandleWalk() {
        if (Direction() == Vector3.zero) { return; }
        Ctx.Asset.transform.forward = Direction();
        Ctx.PlayerRb.AddForce(Direction() * Ctx.MoveSpeed * Time.deltaTime, ForceMode.Force);
        SpeedControl();
    }
    private Vector3 Direction() {
        if (!Ctx.OnSlope) {
            Vector3 direction = Ctx.PlayerForward.transform.forward * Ctx.MoveInput.y + Ctx.PlayerForward.transform.right * Ctx.MoveInput.x;
            //Vector3 direction = Ctx.Player.transform.forward * Ctx.MoveInput.y + Ctx.Player.transform.right * Ctx.MoveInput.x;
            return direction;
        } else {
            Vector3 direction = Ctx.PlayerForward.transform.forward * Ctx.MoveInput.y + Ctx.PlayerForward.transform.right * Ctx.MoveInput.x;
            //Vector3 direction = Ctx.Player.transform.forward * Ctx.MoveInput.y + Ctx.Player.transform.right * Ctx.MoveInput.x;
            Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, Ctx.SurfaceNormal);
            return slopeDirection;
        }
    }
    private void SpeedControl() {
        Vector3 flatvelocity = new Vector3(Ctx.PlayerRb.velocity.x, 0f, Ctx.PlayerRb.velocity.z);
        if (flatvelocity.magnitude > Ctx.MoveSpeed) {
            Vector3 limvelocity = flatvelocity.normalized * Ctx.MoveSpeed;
            Ctx.PlayerRb.velocity = new Vector3(limvelocity.x, Ctx.PlayerRb.velocity.y, limvelocity.z);
        }
    }
}
