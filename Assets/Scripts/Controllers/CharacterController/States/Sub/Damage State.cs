using UnityEngine;

public class DamageState : BaseState, IContextInit {
    public DamageState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }
    public override void EnterState() {
        //Enter logic
        InitializeContext();

        GravityOff();
        
        Ctx.AnimHandler.PlayDirect(AnimHandler.Damage());

        VFXManager.Instance.SpawnFollowVFX(EnvVFX.Shock, Ctx.Player.transform.position, Ctx.Player.transform.rotation, Ctx.Player);
        
        HandleDMG();
        
        Ctx.StartCoroutine("ResetDMG");
    }
    public override void UpdateState() {
        //Update logic

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        GravityOn();
        Ctx.PlayerRb.velocity.Set(Ctx.PlayerRb.velocity.x, Ctx.PlayerRb.velocity.y * 3.14f * 2, Ctx.PlayerRb.velocity.z);
    }
    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.IsWalking) {
            SwitchState(StateHandler.Walk());
        }
        else if (Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
        else if (!Ctx.IsDamaged && Ctx.IsFalling) {
            SwitchState(StateHandler.Fall());
        }
    }
    public void InitializeContext() {
        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
        Ctx.IsJumping = false;
        Ctx.IsAttacking = false;
        Ctx.IsDashing = false;
        Ctx.IsFalling = false;

        Ctx.PlayerInfo.CurrentHp--;
    }
    public void HandleDMG() {
        Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);

        Ctx.PlayerRb.AddForce(Ctx.Asset.transform.forward * -10f, ForceMode.Impulse);
    }
}
