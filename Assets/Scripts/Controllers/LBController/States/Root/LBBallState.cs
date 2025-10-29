using UnityEngine;

public class LBBallState : LBBaseState, IContextInit {
    public LBBallState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext(); 
    }

    public override void EnterState() {
        //Enter logic

        Ctx.AnimHandler.PlayMorph();
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
