using UnityEngine;
using System.Collections;

public class LBPursueState : LBBaseState, IContextInit {
    private float timer;
    private float currentTime;
    
    public LBPursueState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        if (Ctx.MaxHealth == 2 && Ctx.CurrentHealth < Ctx.MaxHealth) {
            Ctx.StartCoroutine(EnterBallRoutine());
        }
        else {
            EnterBugPursue();
            Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Pursue());
        }
    }

    public override void UpdateState() {
        if (Ctx.RootStates[LBRootStates.Bug]) {
            UpdateBugPursue();
        }
        
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
        timer = Ctx.IdleTime;
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

    private IEnumerator EnterBallRoutine() {
        Ctx.SetRootState(LBRootStates.Ball);
        Ctx.Agent.enabled = true;
        yield return null;
        
        Ctx.Agent.speed = 0.01f;
        
        yield return new WaitUntil(() => Ctx.IsMorphing);
        yield return new WaitWhile(() => Ctx.IsMorphing);
        
        Ctx.AnimHandler.PlaySpin();
        yield return null;
        
        while (Ctx.SubStates[LBSubStates.Pursue]) {
            Ctx.Agent.SetDestination(Ctx.Player.transform.position);
            yield return null;
        }
        
        yield break;
    }
}
