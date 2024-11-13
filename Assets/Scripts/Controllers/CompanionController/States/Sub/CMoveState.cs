public class CMoveState : CBaseState {
    public CMoveState(CompanionController currentContext, CompanionStateHandler stateHandler) : base(currentContext, stateHandler) { }
    public override void EnterState() {
        //Enter logic
        
        Ctx.TravelEnterBehaviour();
        Ctx.VisionEnterMoveBehaviour();

        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic        

        Ctx.VisionUpdateMoveBehaviour();

        CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic

        Ctx.VisionExitBehaviour();
        Ctx.TravelExitBehaviour();

    }
    public override void CheckSwitchStates() {
        //Switch logic
        
        if (Ctx.IsOperative && Ctx.IsTalking) {
            SwitchState(StateHandler.Talk());
        } 
        else if (Ctx.IsIdle && !Ctx.TravelLocked) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.IsStuck && Ctx.IsUnstucking) {
            SwitchState(StateHandler.Unstuck());
        }
        
    }
    public void InitializeContext() {
        Ctx.IsTalking = false;
        Ctx.IsIdle = false;
        Ctx.IsUnstucking = false;
    }
   
}
