using UnityEngine;

public class AttackState : BaseState, IContextInit, IVFXInit {
    public AttackState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }

    public override void EnterState() {
        //Enter logic
        InitializeContext();

        ColliderOn(Ctx.AttackCollider);

        HandleAttack();

        GravityOff();

        InitializeParticles();
        
        Ctx.AnimHandler.PlayDirect(AnimHandler.Attack1());
        Ctx.StartCoroutine("ResetAttack");
    }

    public override void UpdateState() {        

        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
        Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);
        ColliderOff(Ctx.AttackCollider);
        Ctx.PlayerRb.isKinematic = false;
        GravityOn();        
    }

    public override void CheckSwitchStates() {
        //Switch logic        
        if (Ctx.IsGrounded && Ctx.CanAttack && Ctx.IsWalking) {          
            SwitchState(StateHandler.Walk());
        }
        else if (Ctx.CanAttack && Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.IsDamaged) {
            SwitchState(StateHandler.Damage());
        } 
        else if (Ctx.IsDashing) {
            SwitchState(StateHandler.Dash());
        }
        else if (Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        }
        else if (!Ctx.IsGrounded && Ctx.IsFalling) {
            SwitchState(StateHandler.Fall());
        }
    }

    public void InitializeContext() {
        Ctx.AttackInput = false;

        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
        Ctx.IsDashing = false;
        Ctx.IsJumping = false;
        Ctx.IsFalling = false;
    }

    public void InitializeParticles() {
        VFXManager.Instance.SpawnFollowVFX(PlayerVFX.AttackBurst, Ctx.AttackPoint.transform.position, Ctx.AttackPoint.transform.rotation, Ctx.AttackPoint);
    }

    private void HandleAttack() {
        Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);
        Ctx.PlayerRb.AddForce(DashDirection() * 3f, ForceMode.Impulse);

        if (!Ctx.IsGrounded) {
            Ctx.SetKinematic();
        }
    }

    private Vector3 DashDirection() {
        if (Ctx.OnSlope) {
            Vector3 direction = Vector3.ProjectOnPlane(Ctx.Asset.transform.forward, Ctx.SurfaceNormal);
            return direction;
        }
        else return Ctx.Asset.transform.forward;
    }
}
