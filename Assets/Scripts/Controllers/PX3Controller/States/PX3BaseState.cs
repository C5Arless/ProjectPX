using UnityEngine;

public abstract class PX3BaseState {
    private bool _isRootState = false;

    PX3Controller _ctx;
    PX3StateHandler _stateHandler;
    PX3AnimHandler _animHandler;
    PX3InputHandler _inputHandler;
    PX3SensorHandler _sensorHandler;

    protected bool IsRootState { set { _isRootState = value; } }

    public PX3BaseState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
        _animHandler = animHandler;
        _inputHandler = inputHandler;
        _sensorHandler = sensorHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void HandleSignal(int sig);
    public abstract void CheckSwitchStates();
    protected void SwitchState(PX3BaseState newState) {
        if (newState._isRootState) {
            _stateHandler.CurrentRootState.ExitState();
            _stateHandler.CurrentRootState = newState;
            _stateHandler.CurrentRootState.EnterState();
        } else {
            _stateHandler.CurrentSubState.ExitState();
            _stateHandler.CurrentSubState = newState;
            _stateHandler.CurrentSubState.EnterState();
        }
    }
}