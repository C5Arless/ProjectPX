using UnityEngine;

public class DashState : BaseState, IContextInit, IVFXInit {
    public DashState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }

    public override void EnterState() {
        //Enter logic
        Ctx.StartCoroutine("ResetDash");
        InitializeContext();

        ColliderOn(Ctx.DashCollider);
        GravityOff();

        InitializeParticles();

        Ctx.AnimHandler.PlayDirect(AnimHandler.Dash());

        HandleDash(Ctx.PlayerRb);        
    }

    public override void UpdateState() {
        //Update logic
        //Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        ColliderOff(Ctx.DashCollider);

        Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);
        Ctx.OnKinematic = false;
        Ctx.PlayerRb.isKinematic = false;

        GravityOn();
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (!Ctx.IsDashing && Ctx.IsFalling) {
            SwitchState(StateHandler.Fall());
        } 
        else if (!Ctx.IsDashing && Ctx.IsAttacking) {
            SwitchState(StateHandler.Attack());
        } 
        else if (!Ctx.IsDashing && Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        }       
        else if (!Ctx.IsDashing && Ctx.IsGrounded && Ctx.IsWalking) {
            SwitchState(StateHandler.Walk());
        }
        else if (!Ctx.IsDashing && Ctx.IsGrounded && Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
    }

    public void InitializeContext() {
        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
        Ctx.IsJumping = false;
        Ctx.IsAttacking = false;
        Ctx.IsDamaged = false;

        if (!Ctx.IsAttacking) {
            Ctx.OnKinematic = true;
            Ctx.PlayerRb.isKinematic = true;
            Ctx.SetKinematic();
        }

    }

    public void InitializeParticles() {
        //VFXManager.Instance.SpawnFixedVFX(PlayerVFX.AirRing, Ctx.RingPoint.transform.position, Ctx.RingPoint.transform.rotation);
        //VFXManager.Instance.SpawnFollowVFX(PlayerVFX.DashTrail, Ctx.RingPoint.transform.position, Ctx.RingPoint.transform.rotation, Ctx.DashPoint);
    }

    public void HandleDash(Rigidbody rb) {
        Ctx.DashInput = false;

        rb.velocity.Set(0f, 0f, 0f);
        rb.AddForce(DashDirection() * 25f, ForceMode.Impulse);
    }

    private Vector3 DashDirection() {
        if (Ctx.OnSlope) {
            Vector3 direction = Vector3.ProjectOnPlane(Ctx.Asset.transform.forward, Ctx.SurfaceNormal);
            return direction;
        }
        else return Ctx.Asset.transform.forward;
    }
}
