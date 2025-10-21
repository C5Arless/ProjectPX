using UnityEngine;

public class LBDamageState : LBBaseState, IContextInit {
    private float timer;
    private float currentTime;
    
    public LBDamageState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        EnterDamage();
    }

    public override void UpdateState() {
        UpdateDamage();
        
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

    private void EnterDamage() {
        currentTime = Time.time;
        timer = Ctx.IdleTime;
        
        Ctx.CurrentHealth--;
        Ctx.Agent.isStopped = true;
        Ctx.Agent.enabled = false;
        
        Ctx.RigidBody.isKinematic = false;
        Ctx.RigidBody.AddForce(Ctx.RigidBody.transform.forward * -10f, ForceMode.Impulse);
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
}