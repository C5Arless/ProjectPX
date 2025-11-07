using UnityEngine;
using System.Collections;

public class LBIdleState : LBBaseState, IContextInit {
    private float timer;
    private float currentTime;
    private bool isOperative;
    
    public LBIdleState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        isOperative = false;

        if (Ctx.RootStates[LBRootStates.Bug]) {
            EnterBugIdle();
            Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Idle());
        }
        else if (Ctx.RootStates[LBRootStates.Ball] && Ctx.CurrentHealth < Ctx.MaxHealth) {
            Ctx.StartCoroutine(EnterBallIdleRoutine());
        }
}

    public override void UpdateState() {
        if (Ctx.RootStates[LBRootStates.Bug] && !Ctx.IsMorphing) {
            UpdateBugIdle();
        }
        
        if (isOperative) {
            CheckSwitchStates();
        }
    }

    public override void ExitState() {
        ExitIdle();
        Ctx.StopCoroutine(EnterBallIdleRoutine());
    }

    public override void CheckSwitchStates() {
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
        timer = Ctx.IdleTime;
    }
    
    public void EnterBugIdle() {
        currentTime = Time.time;
        timer = Ctx.IdleTime;

        Ctx.Agent.speed = 0f;
    }
    
    public void UpdateBugIdle() {
        float elapsed = Time.time - currentTime;
        if (timer > 0) {
            timer -= elapsed * Time.deltaTime;
            
            if (timer > Ctx.IdleTime / 2f) {
                isOperative = true;
            }
        } else {
            Ctx.SetSubState(LBSubStates.Patrol);
            timer = Ctx.IdleTime;
        }
    }

    public void ExitIdle() {
        currentTime = 0f;
        timer = Ctx.IdleTime;
        
        //Ctx.Scout();
    }

    private IEnumerator EnterBallIdleRoutine() {
        Ctx.SetRootState(LBRootStates.Bug);

        yield return new WaitUntil(() => Ctx.IsMorphing);
        yield return new WaitWhile(() => Ctx.IsMorphing);
        
        EnterBugIdle();
        yield break;
    }
}
