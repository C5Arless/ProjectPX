public class LBBugState : LBBaseState, IContextInit {
    public LBBugState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        if (Ctx.CurrentHealth < Ctx.MaxHealth) {
            Ctx.AnimHandler.PlayUnmorph();
        }
    }

    public override void UpdateState() {
        //Update logic
        if (!Ctx.IsRunning || Ctx.IsMorphing) { return; }
            
        if (Ctx.SubStates[LBSubStates.Patrol]) {
            Ctx.Scout();
        }

        if (!Ctx.PatrolZone.ValidateTarget(Ctx.gameObject)) {
            Ctx.SetRootState(LBRootStates.Dead);
        }
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic

    }

    public override void CheckSwitchStates() {
        if (Ctx.RootStates[LBRootStates.Ball]) {
            SwitchState(StateHandler.Ball());
        }
        else if (Ctx.RootStates[LBRootStates.Dead]) {
            SwitchState(StateHandler.Dead());
        }
    }

    public void InitializeContext() {
        //
    }
}
