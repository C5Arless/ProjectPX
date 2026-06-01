using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PX3Controller : MonoBehaviour {
    [SerializeField] Rigidbody _rb;
    [SerializeField] GameObject _asset;
    [SerializeField] private GameObject _forward;
    [SerializeField] GameObject _camera;
    [SerializeField] PX3AnimHandler _animHandler;
    [SerializeField] PX3SensorHandler _sensorHandler;
    [SerializeField] PlayerInput _playerInput;
    [Space]
    [SerializeField]
    [Range(2f, 30f)] float maxGravity;
    [SerializeField]
    [Range(2f, 10f)] float maxSpeed;
    
    PX3StateHandler _stateHandler;
    PX3InputHandler _inputHandler;
    PX3PhysicsHandler _physicsHandler;
    
    #region GetSet

    public PX3StateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    public PX3AnimHandler AnimHandler { get => _animHandler; set => _animHandler = value; }
    public PX3InputHandler InputHandler { get => _inputHandler; set => _inputHandler = value; }
    public PX3SensorHandler SensorHandler { get => _sensorHandler; set => _sensorHandler = value; }
    public PX3PhysicsHandler PhysicsHandler { get => _physicsHandler; set => _physicsHandler = value; }
    
    public GameObject Camera { get => _camera; }
    public GameObject Asset { get => _asset; set => _asset = value; }
    public GameObject Forward { get => _forward; set => _forward = value; }

    public float MaxGravity { get => maxGravity; }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }

    #endregion

    private void Awake() {
        _inputHandler = new PX3InputHandler(_playerInput);
        _physicsHandler = new PX3PhysicsHandler(_rb);
        _stateHandler = new PX3StateHandler(this, _animHandler, _inputHandler, _sensorHandler, _physicsHandler);
        
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

        _forward.transform.forward = GetForwardDirection();
    }

    private void FixedUpdate() {
        if (_stateHandler.RootStates[PX3RootStates.Dead]) return;
        
        _physicsHandler.FixedUpdate();
        _stateHandler.CurrentRootState.FixedUpdateState();
        _stateHandler.CurrentSubState.FixedUpdateState();
    }

    public void OnDestroy() {
        _inputHandler.UnsubscribeCallbacks();
    }

    public void HandleSignal(int sig) {
        
    }

    public Vector3 GetForwardDirection() {
        Vector2 head = new Vector2(transform.position.x, transform.position.z);
        Vector2 tail = new Vector2(_camera.transform.position.x, _camera.transform.position.z);
        
        Vector2 target = (head - tail).normalized;
        Vector3 direction = new Vector3(target.x, 0f, target.y);

        return direction;
    }
}
