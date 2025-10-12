using UnityEngine;

public class LBDamageState : LBBaseState, IContextInit {
    public LBDamageState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic

        InitializeContext();
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
        if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }          
    }

    public void InitializeContext() {
        //
    }
}