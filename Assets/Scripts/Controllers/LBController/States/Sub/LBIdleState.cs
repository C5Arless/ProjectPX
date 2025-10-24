using UnityEngine;

public class LBIdleState : LBBaseState, IContextInit {
    private float timer;
    private float currentTime;
    
    public LBIdleState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        if (Ctx.RootStates[LBRootStates.Bug]) {
            EnterIdle();
            
            Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Idle());
        }
    }

    public override void UpdateState() {
        UpdateIdle();
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        ExitIdle();
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
    
    public void EnterIdle() {
        currentTime = Time.time;
        timer = Ctx.IdleTime;

        Ctx.Agent.speed = 0f;
    }
    
    public void UpdateIdle() {
        float elapsed = Time.time - currentTime;
        if (timer > 0) {
            timer -= elapsed * Time.deltaTime;
        } else {
            Ctx.SetSubState(LBSubStates.Patrol);
            timer = Ctx.IdleTime;
        }
    }

    public void ExitIdle() {
        currentTime = 0f;
        timer = Ctx.IdleTime;
    }
}
