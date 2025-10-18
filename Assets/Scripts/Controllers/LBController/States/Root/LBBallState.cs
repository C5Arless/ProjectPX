using UnityEngine;

public class LBBallState : LBBaseState, IContextInit {
    public LBBallState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true; 
    }

    public override void EnterState() {
        //Enter logic
        
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Morph());
        
        InitializeContext();        
    }

    public override void UpdateState() {
        //Update logic
        
        //if pursue spin
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic        
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.RootStates[LBRootStates.Bug]) {
            SwitchState(StateHandler.Bug());
        }
        else if (Ctx.RootStates[LBRootStates.Dead]) {
            SwitchState(StateHandler.Dead());
        }
    }

    public void InitializeContext() {
        //
    }
}
