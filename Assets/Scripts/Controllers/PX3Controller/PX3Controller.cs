using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PX3Controller : MonoBehaviour {
    [SerializeField] Rigidbody rb;
    [SerializeField] PX3AnimHandler _animHandler;
    [SerializeField] PX3SensorHandler _sensorHandler;
    [SerializeField] PlayerInput _playerInput;
    
    PX3StateHandler _stateHandler;
    PX3InputHandler _inputHandler;
    
    #region GetSet

    public PX3StateHandler StateHandler { get => _stateHandler; set => _stateHandler = value; }
    public PX3AnimHandler AnimHandler { get => _animHandler; set => _animHandler = value; }
    public PX3InputHandler InputHandler { get => _inputHandler; set => _inputHandler = value; }
    public PX3SensorHandler SensorHandler { get => _sensorHandler; set => _sensorHandler = value; }

    #endregion

    private void Awake() {
        _inputHandler = new PX3InputHandler(_playerInput);
        _stateHandler = new PX3StateHandler(this, _animHandler, _inputHandler, _sensorHandler);
        
        _animHandler.Initialize(this, _stateHandler);
        //_sensorHandler.Initialize();
        _stateHandler.Initialize();
    }

    private void Start() {
        _inputHandler.SubscribeCallbacks();

        _inputHandler._onAttack += CallbackTest;
        _inputHandler._onDash += CallbackTest;
        _inputHandler._onJump += CallbackTest;
        _inputHandler._onCrouch += CallbackTest;
        _inputHandler._onSprint += CallbackTest;
    }

    private void Update() {
        //Debug.Log("CamInput: " + _inputHandler.CamInput + "; MoveInput: " + _inputHandler.MoveInput);
    }

    public void OnDestroy() {
        _inputHandler._onAttack -= CallbackTest;
        _inputHandler._onDash -= CallbackTest;
        _inputHandler._onJump -= CallbackTest;
        _inputHandler._onCrouch -= CallbackTest;
        _inputHandler._onSprint -= CallbackTest;
        
        _inputHandler.UnsubscribeCallbacks();
    }

    public void CallbackTest() {
        Debug.Log("Input Received! A: " + _inputHandler.AttackInput + 
                  "; D: " + _inputHandler.DashInput + "; J: " + _inputHandler.JumpInput +
                  "; C: " + _inputHandler.CrouchInput + "; S: " + _inputHandler.SprintInput);
    }
    
 }
