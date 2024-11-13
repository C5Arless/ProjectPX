using UnityEngine;
public class CStuckState : CBaseState {
    public CStuckState(CompanionController currentContext, CompanionStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true; //SOLO SU STUCK E OPERATIVE (ROOT STATES)
    }
    public override void EnterState() {
        //Enter logic

        //Ctx.InvokeRepeating("CheckOperativeBehaviour", 3f, 3f);

        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic              

        /*
        if (!Ctx.IsUnstucking) {
            Ctx.IsUnstucking = true;
        }
        */

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        //Ctx.CancelInvoke("CheckOperativeBehaviour");

    }
    public override void CheckSwitchStates() {
        //Switch logic
        
        if (Ctx.IsOperative) {
            SwitchState(StateHandler.Operative());
        }        
        
    }
    public void InitializeContext() {
        Ctx.IsOperative = false;

        Ctx.IsUnstucking = true;
    }    
}
