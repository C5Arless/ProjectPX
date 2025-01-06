using UnityEngine;

public class DeadState : BaseState, IContextInit {
    public DeadState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base (currentContext, stateHandler, animHandler){
        IsRootState = true; //SOLO SU GROUNDED, AIRBORNE E DEAD (ROOT STATES)
    }

    public override void EnterState() {
        //Enter logic
        Debug.Log("Lmao u ded");

        //Ctx.AnimHandler.SetAlt(true);
        Ctx.AnimHandler.PlayDirect(AnimHandler.Dead());
        VFXManager.Instance.SpawnFollowVFX(EnvVFX.Shock, Ctx.Asset.transform.position, Ctx.Asset.transform.rotation, Ctx.Player);
        VFXManager.Instance.SpawnFollowVFX(EnvVFX.Smoke, Ctx.Asset.transform.position, Ctx.Asset.transform.rotation, Ctx.Player);

        GravityOff();
        InitializeContext();
        ScenesManager.Instance.ReloadOnDeath();
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
        
    }

    public void InitializeContext() {
        //
    }
}
