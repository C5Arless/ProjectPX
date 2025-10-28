using UnityEngine;
using System.Collections;

public class LBAttackState : LBBaseState, IContextInit {
    public LBAttackState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) { 
        InitializeContext();
    }

    public override void EnterState() {
        if (Ctx.RootStates[LBRootStates.Bug]) {
            Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Attack());
            Ctx.StartCoroutine(EnterAttackRoutine());
        }
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        if (Ctx.RootStates[LBRootStates.Bug]) {
            ExitAttack();
        }
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
    
    public void ExitAttack() {
        Ctx.AttackPoint = Vector3.zero;
        
        Ctx.RigidBody.isKinematic = true;
        
        Ctx.Agent.enabled = true;
        Ctx.Agent.isStopped = true;
        
        Ctx.SetSubState(LBSubStates.Idle);
    }

    private IEnumerator EnterAttackRoutine() {
        Ctx.Agent.isStopped = true;
        Ctx.Agent.speed = 0f;
        yield break;
    }
}
