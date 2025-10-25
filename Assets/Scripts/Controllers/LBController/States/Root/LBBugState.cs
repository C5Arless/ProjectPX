using UnityEngine;

public class LBBugState : LBBaseState, IContextInit {
    public LBBugState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic

    }

    public override void UpdateState() {
        //Update logic
        if (!Ctx.IsRunning) { return; }

        if (!Ctx.RootStates[LBRootStates.Dead]) {
            if (Ctx.SubStates[LBSubStates.Patrol] || Ctx.SubStates[LBSubStates.Idle]) Ctx.Scout();
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
