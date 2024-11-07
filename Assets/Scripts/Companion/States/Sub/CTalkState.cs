public class CTalkState : CBaseState {
    public CTalkState(CompanionController currentContext, CompanionStateHandler stateHandler) : base(currentContext, stateHandler) { }
    public override void EnterState() {
        //Enter logic
        Ctx.VisionEnterTalkBehaviour();
        Ctx.TravelEnterTalkBehaviour();

        InitializeContext();
    }
    public override void UpdateState() {
        //Update logic        

        //CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }
    public override void ExitState() {
        //Exit logic
        Ctx.VisionExitBehaviour();
        Ctx.TravelExitBehaviour();
    }
    public override void CheckSwitchStates() {
        //Switch logic
        
        if (Ctx.IsMoving) {
            SwitchState(StateHandler.Move());
        } 
        else if (Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.IsStuck && Ctx.IsUnstucking) {
            SwitchState(StateHandler.Unstuck());
        }
    }
    public void InitializeContext() {
        Ctx.IsMoving = false;
        Ctx.IsIdle = false;
        Ctx.IsUnstucking = false;

        //Ctx.CurrentVelocity = 0f;
    }    
}
