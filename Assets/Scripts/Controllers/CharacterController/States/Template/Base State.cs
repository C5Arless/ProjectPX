using UnityEngine;

public abstract class BaseState {
    private bool _isRootState = false;

    private PXController _ctx;
    private StateHandler _stateHandler;
    private AnimHandler _animHandler;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PXController Ctx { get { return _ctx; } }
    protected StateHandler StateHandler { get { return _stateHandler; } }
    protected AnimHandler AnimHandler { get { return _animHandler; } }

    public BaseState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
        _animHandler = animHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    protected void SwitchState(BaseState newState) {          
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
    protected void GravityOff() {
        _ctx.PlayerRb.useGravity = false;
        if (!_ctx.IsGrounded) {
            _ctx.Gravity = 0f;
        }
    }
    protected void GravityOn() {
        _ctx.PlayerRb.useGravity = true;
        if (!_ctx.IsGrounded) {
            _ctx.Gravity = 9.81f;
        }
    }
    protected void ColliderOff(Collider collider) {
        collider.enabled = false;
    }
    protected void ColliderOn(Collider collider) {
        collider.enabled = true;
    }
}
