using UnityEngine;

public class LBPursueState : LBBaseState, IContextInit {
    public LBPursueState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        EnterBugPursue();
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Pursue());
    }

    public override void UpdateState() {
        UpdateBugPursue();
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
    }

    public override void CheckSwitchStates() {
        if (Ctx.SubStates[LBSubStates.Attack]) {
            SwitchState(StateHandler.Attack());
        }
        else if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.SubStates[LBSubStates.Damaged]) {
            SwitchState(StateHandler.Damaged());
        }
    }

    public void InitializeContext() {
        //
    }
    
    public void EnterBugPursue() {
        Ctx.Agent.speed = Ctx.PursueSpeed;
        Ctx.Agent.isStopped = false;
    }
    
    public void UpdateBugPursue() {
        if (Ctx.Agent.remainingDistance > 3f) {
            Ctx.Agent.SetDestination(Ctx.Player.transform.position);
        } else {
            Ctx.Agent.isStopped = true;
            Ctx.AttackPoint = Ctx.Player.transform.position;
            Ctx.SetSubState(LBSubStates.Attack);
        }
    }
}
