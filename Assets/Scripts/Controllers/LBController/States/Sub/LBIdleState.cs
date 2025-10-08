using UnityEngine;

public class LBIdleState : LBBaseState, IContextInit {
    public LBIdleState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic
        Ctx.IdleStart();
        
        Debug.Log("LB - Entered IdleState.");
        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        
        Debug.Log("LB - Exited IdleState.");
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.IsPursuing) {
            SwitchState(StateHandler.Pursue());
        }
        else if (Ctx.IsPatrolling) {
            SwitchState(StateHandler.Patrol());
        }
    }

    public void InitializeContext() {
        //
    }
}
