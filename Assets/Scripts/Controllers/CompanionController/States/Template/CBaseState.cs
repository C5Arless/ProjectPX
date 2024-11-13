using UnityEngine;

public abstract class CBaseState {
    private bool _isRootState = false;

    private CompanionController _ctx;
    private CompanionStateHandler _stateHandler;

    protected bool IsRootState { set { _isRootState = value; } }
    protected CompanionController Ctx { get { return _ctx; } }
    protected CompanionStateHandler StateHandler { get { return _stateHandler; } }

    public CBaseState(CompanionController currentContext, CompanionStateHandler stateHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    protected void SwitchState(CBaseState newState) {          
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
    protected void ColliderOff(Collider collider) {
        collider.enabled = false;
    }
    protected void ColliderOn(Collider collider) {
        collider.enabled = true;
    }
}
