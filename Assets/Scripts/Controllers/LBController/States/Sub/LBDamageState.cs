using UnityEngine;

public class LBDamageState : LBBaseState, IContextInit {
    public LBDamageState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
    }

    public override void UpdateState() {

        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
    }

    public override void CheckSwitchStates() {
        if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }          
    }

    public void InitializeContext() {
        //
    }
}