using UnityEngine;
using System.Collections;

public class LBAttackState : LBBaseState, IContextInit {
    public LBAttackState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) { 
        InitializeContext();
    }

    public override void EnterState() {
        Ctx.IsReady = false;
        if (Ctx.RootStates[LBRootStates.Bug]) {
            Ctx.AnimHandler.PlayAttack();
            Ctx.StartCoroutine(EnterAttackRoutine());
        }
        else {
            Ctx.StartCoroutine(EnterBallAttackRoutine());
        }
    }

    public override void UpdateState() {
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        ExitAttack();
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

        if (Ctx.RootStates[LBRootStates.Bug]) {
            Ctx.AnimHandler.StopAttack();
            Ctx.SetSubState(LBSubStates.Idle);
        }
        else {
            Ctx.AnimHandler.StopSpin();
            Ctx.SetSubState(LBSubStates.Idle);
        }
    }
    
    private IEnumerator EnterBallAttackRoutine() {
        Ctx.Agent.enabled = false;
        yield return null;
        
        Ctx.RigidBody.isKinematic = false;
        yield return new WaitForSeconds(.1f);
        
        Ctx.IsReady = true;
        yield break;
    }
    
    private IEnumerator EnterAttackRoutine() {
        Ctx.Agent.isStopped = true;
        Ctx.Agent.speed = 0f;
        yield return new WaitForSeconds(.25f);
        
        Ctx.IsReady = true;
        yield break;
    }
}
