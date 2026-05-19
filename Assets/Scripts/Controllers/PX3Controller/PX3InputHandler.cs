using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PX3InputHandler {
    PlayerInput _playerInput;
    
    InputAction _lookAction;
    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _attackAction;
    InputAction _dashAction;
    InputAction _crouchAction;
    InputAction _sprintAction;
    
    private Vector2 camInput;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;
    private bool crouchInput;
    private bool sprintInput;
    
    public delegate void OnJumpAction();
    public delegate void OnAttackAction();
    public delegate void OnDashAction();
    public delegate void OnCrouchAction();
    public delegate void OnSprintAction();
    
    public OnJumpAction _onJump = () => { };
    public OnAttackAction _onAttack = () => { };
    public OnDashAction _onDash = () => { };
    public OnCrouchAction _onCrouch = () => { };
    public OnSprintAction _onSprint = () => { };
    
    public Vector2 CamInput { get => camInput; }
    public Vector2 MoveInput { get => moveInput; }
    public bool JumpInput { get => jumpInput; }
    public bool AttackInput { get => attackInput; }
    public bool DashInput { get => dashInput; }
    public bool CrouchInput { get => crouchInput; }
    public bool SprintInput { get => sprintInput; }

    public PX3InputHandler(PlayerInput playerInput) {
        _playerInput = playerInput;
        
        InitializeActions();
    }

    public void SubscribeCallbacks() {
        _lookAction.started += OnLook;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        
        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        
        _jumpAction.started += OnJump;
        _jumpAction.performed += OnJump;
        _jumpAction.canceled += OnJump;

        _attackAction.started += OnAttack;
        _attackAction.performed += OnAttack;
        _attackAction.canceled += OnAttack;
        
        _dashAction.started += OnDash;
        _dashAction.performed += OnDash;
        _dashAction.canceled += OnDash;
        
        _crouchAction.started += OnCrouch;
        _crouchAction.performed += OnCrouch;
        _crouchAction.canceled += OnCrouch;
        
        _sprintAction.started += OnSprint;
        _sprintAction.performed += OnSprint;
        _sprintAction.canceled += OnSprint;
    }

    public void UnsubscribeCallbacks() {
        _lookAction.started -= OnLook;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
        
        _moveAction.started -= OnMove;
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        
        _jumpAction.started -= OnJump;
        _jumpAction.performed -= OnJump;
        _jumpAction.canceled -= OnJump;

        _attackAction.started -= OnAttack;
        _attackAction.performed -= OnAttack;
        _attackAction.canceled -= OnAttack;
        
        _dashAction.started -= OnDash;
        _dashAction.performed -= OnDash;
        _dashAction.canceled -= OnDash;
        
        _crouchAction.started -= OnCrouch;
        _crouchAction.performed -= OnCrouch;
        _crouchAction.canceled -= OnCrouch;
        
        _sprintAction.started -= OnSprint;
        _sprintAction.performed -= OnSprint;
        _sprintAction.canceled -= OnSprint;
    }
    
    private void InitializeActions() {
        _lookAction = _playerInput.actions.FindAction("Look");
        _moveAction = _playerInput.actions.FindAction("Move");
        _jumpAction = _playerInput.actions.FindAction("Jump");
        _attackAction = _playerInput.actions.FindAction("Attack");
        _dashAction = _playerInput.actions.FindAction("Dash");
        _crouchAction = _playerInput.actions.FindAction("Crouch");
        _sprintAction = _playerInput.actions.FindAction("Sprint");
    }
    
    public void OnLook(InputAction.CallbackContext input) {
        if (input.ReadValue<Vector2>() != Vector2.zero) {
            camInput = input.ReadValue<Vector2>();            
        } else {
            camInput = Vector2.zero;
        }
    }
    
    public void OnMove(InputAction.CallbackContext input) {
        moveInput = input.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext input) {
        if (input.started) {
            jumpInput = true;
        } else if (input.canceled) {
            jumpInput = false;
        }
        
        if (input.performed) {
            _onJump?.Invoke();
        }
    }
    
    public void OnAttack(InputAction.CallbackContext input) {
        if (input.started) {
            attackInput = true;
        } else if (input.canceled) {
            attackInput = false;
        }
        
        if (input.performed) {
            _onAttack?.Invoke();
        }
    }
    
    public void OnDash(InputAction.CallbackContext input) {
        if (input.started) {
            dashInput = true;
        } else if (input.canceled) {
            dashInput = false;
        }
        
        if (input.performed) {
            _onDash?.Invoke();
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext input) {
        if (input.started) {
            crouchInput = true;
        } else if (input.canceled) {
            crouchInput = false;
        }
        
        if (input.performed) {
            _onCrouch?.Invoke();
        }
    }
    
    public void OnSprint(InputAction.CallbackContext input) {
        if (input.started) {
            sprintInput = true;
        } else if (input.canceled) {
            sprintInput = false;
        }
        
        if (input.performed) {
            _onSprint?.Invoke();
        }
    }
}
