using UnityEngine;

public class LBIdleState : LBBaseState, IContextInit {
    public LBIdleState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic
        if (Ctx.RootStates[LBRootStates.Bug]) {
            //Ctx.IdleStart();
            Ctx.EnterIdle();
            Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Idle());
        }
        
        //Debug.Log("LB - Entered IdleState.");
        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic
        
        Ctx.UpdateIdle();
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        
        Ctx.ExitIdle();
        
        //Debug.Log("LB - Exited IdleState.");
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.SubStates[LBSubStates.Pursue]) {
            SwitchState(StateHandler.Pursue());
        }
        else if (Ctx.SubStates[LBSubStates.Patrol]) {
            SwitchState(StateHandler.Patrol());
        }
        else if (Ctx.SubStates[LBSubStates.Damaged]) {
            SwitchState(StateHandler.Damaged());
        }
    }

    public void InitializeContext() {
        //
    }
}
