using UnityEngine;
using System.Collections;

public class LBDamageState : LBBaseState, IContextInit {
    private float timer;
    private float currentTime;
    
    public LBDamageState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        Ctx.StartCoroutine(DamageEnterRoutine());
        //Ctx.StartCoroutine(EvaluateDeath());
    }

    public override void UpdateState() {
        //UpdateDamage();
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
        ExitDamage();
    }

    public override void CheckSwitchStates() {
        if (Ctx.SubStates[LBSubStates.Idle]) {
            SwitchState(StateHandler.Idle());
        } 
    }

    public void InitializeContext() {
        timer = Ctx.IdleTime;
    }
    
    public void UpdateDamage() {
        float elapsed = Time.time - currentTime;
        if (timer > 0) {
            timer -= elapsed * Time.deltaTime;
        } else {

            if (Ctx.CurrentHealth > 0) {
                Ctx.SetSubState(LBSubStates.Idle);
                timer = Ctx.IdleTime;
            } else {
                Ctx.SetRootState(LBRootStates.Dead);
            }
        }
    }
    
    private void ExitDamage() {
        Ctx.RigidBody.isKinematic = true;
        
        Ctx.Agent.enabled = true;
        Ctx.Agent.isStopped = true;

        Ctx.IsDamaged = false;
        
        currentTime = 0f;
        timer = Ctx.IdleTime;
    }

    private IEnumerator DamageEnterRoutine() {
        currentTime = Time.time;
        timer = Ctx.IdleTime;
        
        Ctx.CurrentHealth--;
        Ctx.Agent.speed = 0f;
        Ctx.Agent.isStopped = true;
        Ctx.Agent.enabled = false;
        yield return null;
        
        Ctx.RigidBody.isKinematic = false;
        Ctx.RigidBody.ResetInertiaTensor();
        yield return null;
        
        //To be moved to a collisionSolver
        Ctx.RigidBody.AddForce(Ctx.RigidBody.transform.forward * -5f, ForceMode.Impulse);
        Ctx.RigidBody.AddForce(Ctx.RigidBody.transform.up * 4f, ForceMode.Impulse);
        yield return new  WaitForSeconds(.5f);

        if (Ctx.CurrentHealth <= 0) {
            Ctx.SetRootState(LBRootStates.Dead);    
        } else {
            Ctx.SetSubState(LBSubStates.Idle);  
        }
        
        yield break;
    }

    private IEnumerator EvaluateDeath() {
        while (Ctx.SubStates[LBSubStates.Damaged]) {
            if (Ctx.RootStates[LBRootStates.Dead]) {
                ExitDamage();
                yield break;
            } else yield return null;
        }

        yield break;
    }
}