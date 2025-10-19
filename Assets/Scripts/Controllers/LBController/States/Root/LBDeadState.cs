using UnityEngine;

public class LBDeadState : LBBaseState, IContextInit {
    public LBDeadState(LBController currentContext, LBStateHandler stateHandler) : base(currentContext, stateHandler) {
        IsRootState = true;
        
        InitializeContext();
    }

    public override void EnterState() {
        //Enter logic
        
    }

    public override void UpdateState() {
        //Update logic

        //CheckSwitchStates(); //MUST BE LAST INSTRUCTION
    }

    public override void ExitState() {
        //Exit logic        
    }

    public override void CheckSwitchStates() {
        //Switch logic        
    }

    public void InitializeContext() {
        //
    }
}
