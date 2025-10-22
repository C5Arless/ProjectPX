using UnityEngine;
using System.Collections;

public abstract class LBBaseState {
    private bool _isRootState = false;

    private LBController _ctx;
    private LBStateHandler _stateHandler;

    protected bool IsRootState { set { _isRootState = value; } }
    protected LBController Ctx { get { return _ctx; } }
    protected LBStateHandler StateHandler { get { return _stateHandler; } }

    public LBBaseState(LBController currentContext, LBStateHandler stateHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    protected void SwitchState(LBBaseState newState) {
        if (newState._isRootState) {
            _ctx.CurrentRootState.ExitState();
            _ctx.CurrentRootState = newState;
            _ctx.CurrentRootState.EnterState();
        }
        else {
            _ctx.CurrentSubState.ExitState();
            _ctx.CurrentSubState = newState;
            _ctx.CurrentSubState.EnterState();
        }
    }
}
