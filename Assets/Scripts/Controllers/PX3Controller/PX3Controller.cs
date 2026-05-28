using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PX3Controller : MonoBehaviour {
    [SerializeField] Rigidbody _rb;
    [SerializeField] PX3AnimHandler _animHandler;
    [SerializeField] PX3SensorHandler _sensorHandler;
    [SerializeField] PlayerInput _playerInput;
    
    PX3StateHandler _stateHandler;
    PX3InputHandler _inputHandler;
    PX3PhysicsHandler _physicsHandler;
    
    #region GetSet

    public PX3StateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    public PX3AnimHandler AnimHandler { get => _animHandler; set => _animHandler = value; }
    public PX3InputHandler InputHandler { get => _inputHandler; set => _inputHandler = value; }
    public PX3SensorHandler SensorHandler { get => _sensorHandler; set => _sensorHandler = value; }
    public PX3PhysicsHandler PhysicsHandler { get => _physicsHandler; set => _physicsHandler = value; }

    #endregion

    private void Awake() {
        _inputHandler = new PX3InputHandler(_playerInput);
        _stateHandler = new PX3StateHandler(this, _animHandler, _inputHandler, _sensorHandler);
        _physicsHandler = new PX3PhysicsHandler(_rb, _sensorHandler);
        
        _animHandler.Initialize(this, _stateHandler);
        _stateHandler.Initialize();
    }

    private void Start() {
        _inputHandler.SubscribeCallbacks();
    }
    
    private void Update() {
        if (_stateHandler.RootStates[PX3RootStates.Dead]) return;
        
        _stateHandler.CurrentRootState.UpdateState();
        _stateHandler.CurrentSubState.UpdateState();
    }
    
    private void LateUpdate() {
        if (_stateHandler.RootStates[PX3RootStates.Dead]) return;
        
        _stateHandler.CurrentRootState.LateUpdateState();
        _stateHandler.CurrentSubState.LateUpdateState();
    }

    private void FixedUpdate() {
        if (_stateHandler.RootStates[PX3RootStates.Dead]) return;
        
        _physicsHandler.FixedUpdate();
    }

    public void OnDestroy() {
        _inputHandler.UnsubscribeCallbacks();
    }

    public void HandleSignal(int sig) {
        
    }

}
