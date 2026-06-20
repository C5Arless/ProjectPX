using UnityEngine;
using System.Collections;

public abstract class PX3BaseState {
    bool _isRootState = false;

    PX3Controller _ctx;
    PX3StateHandler _stateHandler;
    PX3AnimHandler _animHandler;
    PX3InputHandler _inputHandler;
    PX3SensorHandler _sensorHandler;
    PX3PhysicsHandler _physicsHandler;

    protected bool IsRootState { get => _isRootState; set => _isRootState = value; }
    protected PX3Controller Context { get => _ctx; set => _ctx = value; }
    protected PX3StateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    protected PX3AnimHandler AnimHandler { get => _animHandler; set => _animHandler = value; }
    protected PX3InputHandler InputHandler { get => _inputHandler; set => _inputHandler = value; }
    protected PX3SensorHandler SensorHandler { get => _sensorHandler; set => _sensorHandler = value; }
    protected PX3PhysicsHandler PhysicsHandler { get => _physicsHandler; set => _physicsHandler = value; }
    
    public PX3BaseState(PX3Controller currentContext, PX3StateHandler stateHandler, 
        PX3AnimHandler animHandler, PX3InputHandler inputHandler, PX3SensorHandler sensorHandler, 
        PX3PhysicsHandler physicsHandler) {
        _ctx = currentContext;
        _stateHandler = stateHandler;
        _animHandler = animHandler;
        _inputHandler = inputHandler;
        _sensorHandler = sensorHandler;
        _physicsHandler = physicsHandler;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void HandleSignal(int sig);
    public abstract void CheckSwitchStates();
    protected void SwitchState(PX3BaseState newState) {
        if (newState == this) return;
        
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
    protected void StartCoroutine(IEnumerator routine) {
        _ctx.StartCoroutine(routine);
    }
    protected void StopCoroutine(IEnumerator routine) {
        _ctx.StopCoroutine(routine);
    }
}