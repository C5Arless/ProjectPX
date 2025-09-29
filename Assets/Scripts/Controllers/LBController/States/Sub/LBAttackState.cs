using UnityEngine;

public class LBAttackState : LBBaseState, IContextInit {
    public LBAttackState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
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

    }

    public void InitializeContext() {
        //
    }
}
