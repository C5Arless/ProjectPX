using UnityEngine;

public class LBBallState : LBBaseState, IContextInit {
    public LBBallState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true; 
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
        if (Ctx.IsBug) {
            SwitchState(StateHandler.Bug());
        }
    }

    public void InitializeContext() {
        //
    }
}
