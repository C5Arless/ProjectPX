using UnityEngine;

public class LBPatrolState : LBBaseState, IContextInit {
    public LBPatrolState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic
        
        Ctx.SetPatrolPosition();
        Debug.Log("LB - Entered PatrolState.");
        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        Debug.Log("LB - Exited PatrolState.");
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.IsPursuing) {
            SwitchState(StateHandler.Pursue());
        }
        else if (Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
    }

    public void InitializeContext() {
        //
    }
}
