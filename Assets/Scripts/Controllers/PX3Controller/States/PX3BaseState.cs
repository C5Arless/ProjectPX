using UnityEngine;

public abstract class PX3BaseState {
    private bool _isRootState = false;

    private PX3Controller _ctx;
    private PX3StateHandler _stateHandler;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PX3Controller Ctx { get { return _ctx; } }
    protected PX3StateHandler StateHandler { get { return _stateHandler; } }

    public PX3BaseState(PX3Controller currentContext, PX3StateHandler stateHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    protected void SwitchState(PX3BaseState newState) {
        if (newState._isRootState) {
            _ctx.CurrentRootState.ExitState();
            _ctx.CurrentRootState = newState;
            _ctx.CurrentRootState.EnterState();
        } else {
            _ctx.CurrentSubState.ExitState();
            _ctx.CurrentSubState = newState;
            _ctx.CurrentSubState.EnterState();
        }
    }
}