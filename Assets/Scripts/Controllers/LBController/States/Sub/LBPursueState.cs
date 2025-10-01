using UnityEngine;

public class LBPursueState : LBBaseState, IContextInit {
    public LBPursueState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
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
        if (Ctx.IsAttacking) {
            SwitchState(StateHandler.Attack());
        }
        else if (Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
    }

    public void InitializeContext() {
        //
    }
}
