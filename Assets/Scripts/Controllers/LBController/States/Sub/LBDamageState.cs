using UnityEngine;
using System.Collections;

public class LBDamageState : LBBaseState, IContextInit {
    public LBDamageState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        Ctx.StartCoroutine(DamageEnterRoutine());
    }

    public override void UpdateState() {
        
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
        //
    }
    
    private void ExitDamage() {
        Ctx.RigidBody.isKinematic = true;
        
        Ctx.Agent.enabled = true;
        Ctx.Agent.isStopped = true;

        Ctx.IsDamaged = false;
    }

    private IEnumerator DamageEnterRoutine() {
        if (Ctx.RootStates[LBRootStates.Bug]) {
            Ctx.AnimHandler.StopAttack();
        }

        yield return null;
        
        Ctx.CurrentHealth--;
        Ctx.Agent.speed = 0f;
        Ctx.Agent.isStopped = true;
        Ctx.Agent.enabled = false;
        yield return null;
        
        Ctx.RigidBody.isKinematic = false;
        Ctx.RigidBody.ResetInertiaTensor();
        yield return null;
        
        Ctx.RigidBody.AddForce(Ctx.RigidBody.transform.forward * -5f, ForceMode.Impulse);
        Ctx.RigidBody.AddForce(Ctx.RigidBody.transform.up * 4f, ForceMode.Impulse);
        yield return new WaitForSeconds(2f);

        if (Ctx.CurrentHealth <= 0) {
            Ctx.SetRootState(LBRootStates.Dead);    
        } else {
            Ctx.SetSubState(LBSubStates.Idle);  
        }
        
        yield break;
    }
}