using UnityEngine;

public class LBBallState : LBBaseState, IContextInit {
    public LBBallState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext(); 
    }

    public override void EnterState() {
        //Enter logic

        Ctx.IsMorphing = true;
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Morph());
    }

    public override void UpdateState() {
        //Update logic
        
        //if pursue spin
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        
        //Ctx.IsMorphing = true;
        //Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Unmorph());
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
