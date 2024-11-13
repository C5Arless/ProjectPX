public class COperativeState : CBaseState {
    public COperativeState(CompanionController currentContext, CompanionStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true; //SOLO SU STUCK E OPERATIVE (ROOT STATES)
    }
    public override void EnterState() {
        //Enter logic

        Ctx.InvokeRepeating("CheckStuckBehaviour", 3f, 3f);

        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic        

        Ctx.StorePlayerDistance();
        
        if (!Ctx.IsTalking && Ctx.IsMoving && !Ctx.IsStuck) {            
            Ctx.IsIdle = false;
        }
        else if (!Ctx.IsTalking && !Ctx.IsMoving && !Ctx.IsStuck) {
            Ctx.IsIdle = true;            
        }

        if (Ctx.PlayerDistance > Ctx.LimitDistanceMax && !Ctx.IsStuck && !Ctx.IsTalking) {
            Ctx.IsMoving = true;
        }

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        Ctx.CancelInvoke("CheckStuckBehaviour");
    }
    public override void CheckSwitchStates() {
        //Switch logic
        
        if (Ctx.IsStuck) {
            SwitchState(StateHandler.Stuck());
        }        
        
    }
    public void InitializeContext() {
        Ctx.IsStuck = false;

        Ctx.IsUnstucking = false;
    }
    
}
