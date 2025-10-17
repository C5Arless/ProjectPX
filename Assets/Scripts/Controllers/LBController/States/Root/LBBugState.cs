using UnityEngine;

public class LBBugState : LBBaseState, IContextInit {
    public LBBugState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
    }

    public override void EnterState() {
        //Enter logic

        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic
        
        //if Running && !Idle scout
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic

    }

    public override void CheckSwitchStates() {
        //Switch logic
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
