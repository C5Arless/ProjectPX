using UnityEngine;

public class LBPursueState : LBBaseState, IContextInit {
    public LBPursueState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic
        
        Ctx.EnterPursue();
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Pursue());
        
        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic
        
        Ctx.UpdatePursue();
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic

    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.SubStates[LBSubStates.Attack]) {
            SwitchState(StateHandler.Attack());
        }
        else if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.SubStates[LBSubStates.Damaged]) {
            SwitchState(StateHandler.Damaged());
        }
    }

    public void InitializeContext() {
        //
    }
}
