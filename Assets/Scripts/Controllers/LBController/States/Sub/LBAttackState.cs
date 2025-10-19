using UnityEngine;

public class LBAttackState : LBBaseState, IContextInit {
    public LBAttackState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) { 
        InitializeContext();
    }

    public override void EnterState() {
        EnterAttack();
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
    }

    public override void CheckSwitchStates() {
        if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.SubStates[LBSubStates.Damaged]) {
            SwitchState(StateHandler.Damaged());
        }
    }

    public void InitializeContext() {
        //
    }
    
    public void EnterAttack() {
        Ctx.AttackPoint = Vector3.zero;
        Ctx.SetSubState(LBSubStates.Idle);
    }
}
