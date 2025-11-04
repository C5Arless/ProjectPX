using UnityEngine;
using System.Collections;

public class LBPursueState : LBBaseState, IContextInit {
    private float timer;
    
    public LBPursueState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        if (Ctx.CurrentHealth < Ctx.MaxHealth) {
            Ctx.StartCoroutine(EnterBallRoutine());
        }
        else {
            Ctx.StartCoroutine(EnterBugRoutine());
        }
    }

    public override void UpdateState() {
        
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

    private IEnumerator EnterBugRoutine() {
        Ctx.Agent.speed = Ctx.PursueSpeed;
        Ctx.Agent.isStopped = false;
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Pursue());
        yield return null;
        
        while (Ctx.Agent.remainingDistance > 3.5f) {
            Ctx.Agent.SetDestination(Ctx.Player.transform.position);
            yield return null;
        }
        
        Ctx.Agent.isStopped = true;
        Ctx.AttackPoint = Ctx.Player.transform.position;
        Ctx.SetSubState(LBSubStates.Attack);
        yield break;
    }
    
    private IEnumerator EnterBallRoutine() {
        Ctx.Agent.speed = Ctx.PursueSpeed;
        Ctx.Agent.isStopped = false;
        Ctx.AnimHandler.PlayDirect(Ctx.AnimHandler.Pursue());
        yield return null;
        
        while (Ctx.Agent.remainingDistance > Ctx.VisionRange / 2) {
            Ctx.Agent.SetDestination(Ctx.Player.transform.position);
            yield return null;
        }
        
        Ctx.SetRootState(LBRootStates.Ball);
        Ctx.Agent.enabled = true;
        yield return null;
        
        Ctx.Agent.isStopped = true;
        
        yield return new WaitUntil(() => Ctx.IsMorphing);
        yield return new WaitWhile(() => Ctx.IsMorphing);
        
        Ctx.AnimHandler.PlaySpin();
        yield return null;
        
        Vector3 targetPos = Vector3.zero;
        Vector3 selfPos = Vector3.zero;
        Vector3 direction = Vector3.zero;
        timer =  Ctx.IdleTime * .8f;
        
        while (timer > 0f) {
            timer -= Time.deltaTime;
            targetPos = Ctx.Player.transform.position;
            selfPos = Ctx.transform.position;
            
            direction = new Vector3(targetPos.x - selfPos.x, 0f, targetPos.z - selfPos.z);

            if (direction.sqrMagnitude > 0.01f) {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                
                Ctx.transform.rotation = Quaternion.RotateTowards(
                    Ctx.transform.rotation,
                    targetRotation,
                    720f * Time.deltaTime
                );
            }
            
            yield return null;
        }
        
        timer = Ctx.IdleTime * .8f;
        
        yield break;
    }
}
