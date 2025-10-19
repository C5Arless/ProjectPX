using UnityEngine;

public class LBPatrolState : LBBaseState, IContextInit {
    public LBPatrolState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        EnterPatrol();
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Patrol());
    }

    public override void UpdateState() {
        UpdatePatrol();

        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
    }

    public override void CheckSwitchStates() {
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
        //
    }
    
    public void EnterPatrol() {
        Ctx.Agent.speed = Ctx.PatrolSpeed;
        Ctx.Agent.isStopped = false;
        
        Vector3 patrolPosition = Ctx.PatrolZone.RetrieveWaypoint();
        Ctx.Agent.SetDestination(patrolPosition);
    }
    
    public void UpdatePatrol() {
        if (Ctx.Agent.remainingDistance > Ctx.Agent.radius * 2f) { return; }

        if (!Ctx.SubStates[LBSubStates.Pursue]) {
            Ctx.Agent.isStopped = true;
            Ctx.SetSubState(LBSubStates.Idle);
        }
    }
}
