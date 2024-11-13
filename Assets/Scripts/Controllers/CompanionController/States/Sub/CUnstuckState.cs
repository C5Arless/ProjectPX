public class CUnstuckState : CBaseState {
    public CUnstuckState(CompanionController currentContext, CompanionStateHandler stateHandler) : base(currentContext, stateHandler) { }
    public override void EnterState() {
        //Enter logic
        
        Ctx.InvokeRepeating("UnstuckEnterBehaviour", 0f, 3f);
        //Ctx.Invoke("UnstuckEnterBehaviour", 0f);
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

        Ctx.CancelInvoke("UnstuckEnterBehaviour");
        Ctx.UnstuckExitBehaviour();
        Ctx.VisionExitBehaviour();
    }

    public override void CheckSwitchStates() {
        //Switch logic
        
        if (!Ctx.IsStuck && Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
        
    }
    public void InitializeContext() {
        Ctx.IsMoving = false;
        Ctx.IsIdle = false;
        Ctx.IsTalking = false;
    }    
}
