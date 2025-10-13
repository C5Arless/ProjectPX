using UnityEngine;

public class LBPatrolState : LBBaseState, IContextInit {
    public LBPatrolState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        //
    }

    public override void EnterState() {
        //Enter logic
        Debug.Log("LB - Entered PatrolState.");
        
        InitializeContext();
    }

    public override void UpdateState() {
        //Update logic
        
        Ctx.UpdatePatrol();
        
        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic
        Debug.Log("LB - Exited PatrolState.");
    }

    public override void CheckSwitchStates() {
        //Switch logic
        if (Ctx.SubStates[LBSubStates.Pursue]) {
            SwitchState(StateHandler.Pursue());
        }
        else if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.SubStates[LBSubStates.Damaged]) {
            SwitchState(StateHandler.Damaged());
        }
    }

    public void InitializeContext() {
        Ctx.EnterPatrol();
        Ctx.SetPatrolPosition();
    }
}
