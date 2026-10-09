using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PX3InputHandler : MonoBehaviour {
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
    private Vector2 rawMoveInput;
    
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;
    private bool crouchInput;
    private bool sprintInput;
    
    private bool onMoveInput;

    public event Action<PX3InputPhase> OnLookAction;
    public event Action<PX3InputPhase> OnMoveAction;
    public event Action<PX3InputPhase> OnJumpAction;
    public event Action<PX3InputPhase> OnAttackAction;
    public event Action<PX3InputPhase> OnDashAction;
    public event Action<PX3InputPhase> OnCrouchAction;
    public event Action<PX3InputPhase> OnSprintAction;
    
    /*
    public delegate void OnCamAction();
    public delegate void OnMoveAction();
    public delegate void OnJumpAction();
    public delegate void OnAttackAction();
    public delegate void OnDashAction();
    public delegate void OnCrouchAction();
    public delegate void OnSprintAction();

    public OnCamAction _onCam = () => { };
    public OnMoveAction _onMove = () => { };
    public OnJumpAction _onJump = () => { };
    public OnAttackAction _onAttack = () => { };
    public OnDashAction _onDash = () => { };
    public OnCrouchAction _onCrouch = () => { };
    public OnSprintAction _onSprint = () => { };
    */
    
    public Vector2 CamInput { get => camInput; }
    public Vector2 RawMoveInput { get => rawMoveInput; }
    public Vector2 MoveInput { get => moveInput; }
    public bool JumpInput { get => jumpInput; }
    public bool AttackInput { get => attackInput; }
    public bool DashInput { get => dashInput; }
    public bool CrouchInput { get => crouchInput; }
    public bool SprintInput { get => sprintInput; }

    public void Initialize(PlayerInput playerInput) {
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

    private void Update() {
        if (onMoveInput) {
            moveInput = Vector2.MoveTowards(moveInput, rawMoveInput, 15f * Time.deltaTime);
            OnMoveAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onMove?.Invoke();
        }
        else {
            if (moveInput != Vector2.zero) {
                moveInput = Vector2.MoveTowards(moveInput, Vector2.zero, 15f * Time.deltaTime);
            } 
        }
    }

    public void OnLook(InputAction.CallbackContext input) {
        if (input.ReadValue<Vector2>() != Vector2.zero) {
            camInput = input.ReadValue<Vector2>();
            OnLookAction?.Invoke(PX3InputPhase.performing); //NEW
        } else {
            camInput = Vector2.zero;
            OnLookAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
    }
    
    public void OnMove(InputAction.CallbackContext input) {
        if (input.performed) {
            rawMoveInput = input.ReadValue<Vector2>();
            onMoveInput = true;
            OnMoveAction?.Invoke(PX3InputPhase.performing); //NEW
        } else if (input.canceled) {
            rawMoveInput = Vector2.zero;
            onMoveInput = false;
            OnMoveAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
    }
    
    public void OnJump(InputAction.CallbackContext input) {
        if (input.started) {
            jumpInput = true;
            
            OnJumpAction?.Invoke(PX3InputPhase.started); //NEW
        } else if (input.canceled) {
            jumpInput = false;
            
            OnJumpAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
        
        if (input.performed) {
            OnJumpAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onJump?.Invoke();
        }
    }
    
    public void OnAttack(InputAction.CallbackContext input) {
        if (input.started) {
            attackInput = true;
            OnAttackAction?.Invoke(PX3InputPhase.started); //NEW
        } else if (input.canceled) {
            attackInput = false;
            OnAttackAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
        
        if (input.performed) {
            OnAttackAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onAttack?.Invoke();
        }
    }
    
    public void OnDash(InputAction.CallbackContext input) {
        if (input.started) {
            dashInput = true;
            OnDashAction?.Invoke(PX3InputPhase.started); //NEW
        } else if (input.canceled) {
            dashInput = false;
            OnDashAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
        
        if (input.performed) {
            OnDashAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onDash?.Invoke();
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext input) {
        if (input.started) {
            crouchInput = true;
            OnCrouchAction?.Invoke(PX3InputPhase.started); //NEW
        } else if (input.canceled) {
            crouchInput = false;
            OnCrouchAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
        
        if (input.performed) {
            OnCrouchAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onCrouch?.Invoke();
        }
    }
    
    public void OnSprint(InputAction.CallbackContext input) {
        if (input.started) {
            sprintInput = true;
            OnSprintAction?.Invoke(PX3InputPhase.started); //NEW
        } else if (input.canceled) {
            sprintInput = false;
            OnSprintAction?.Invoke(PX3InputPhase.canceled); //NEW
        }
        
        if (input.performed) {
            OnSprintAction?.Invoke(PX3InputPhase.performing); //NEW
            //_onSprint?.Invoke();
        }
    }
}
