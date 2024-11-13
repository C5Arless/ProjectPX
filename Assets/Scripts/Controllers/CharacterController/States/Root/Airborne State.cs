using System.Collections;
using UnityEngine;

public class AirborneState : BaseState, IContextInit, IPhysics {
    public AirborneState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        IsRootState = true; //SOLO SU GROUNDED, AIRBORNE E DEAD (ROOT STATES)
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic
        if (Ctx.PlayerRb.velocity.y < -1f) {
            Ctx.IsFalling = true;
        } 

        if (!Ctx.IsDashing || !Ctx.IsAttacking) {
            HandleGravity(Ctx.PlayerRb);
        }

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        Ctx.Gravity = 9.81f;
    }
    public override void CheckSwitchStates() {
        //Switch logic

        if (Ctx.IsGrounded) {
            SwitchState(StateHandler.Grounded());
        }
        else if (Ctx.IsDead) {
            SwitchState(StateHandler.Dead());
        }
    }
    public void InitializeContext() {
        if (Ctx.MoveSpeed > 600) {
            Ctx.StartCoroutine("InitializeMoveSpeed");
        }

        if (Ctx.IsFalling) {
            InitializeJumpCount();
        }
    }
    public void HandleGravity(Rigidbody rb) {
        //Gravity logic
        if (Ctx.Gravity < (9.81f * Ctx.GravityMultiplier)) {
            Ctx.Gravity = Ctx.Gravity + Ctx.GravitySpeed;
        }
        rb.AddForce(Vector3.up * -Ctx.Gravity * Time.deltaTime, ForceMode.VelocityChange);
    }    
    private void InitializeJumpCount() {
        if (Ctx.JumpCount > 0) {
            Ctx.JumpCount--;
        }
    }
}
