using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PXController : MonoBehaviour {
    //State reference
    BaseState _currentRootState;
    BaseState _currentSubState;

    //Custom Components
    StateHandler _stateHandler;
    AnimHandler _animHandler;

    //Player references
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _asset;
    [SerializeField] GameObject _camHolder;
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] GameObject _forward;
    [SerializeField] GameObject _playerparent;
    [SerializeField] GameObject _head;

    [SerializeField] GameObject _dashPoint; // Spawnpoint Dash VFX
    [SerializeField] GameObject _attackPoint; // Spawnpoint Attack VFX
    [SerializeField] GameObject _jumpPoint; // Spawnpoint Jump VFX
    [SerializeField] GameObject _ringPoint; // Spawnpoint Ring VFX

    [SerializeField] Material _visor; // Material reference for Health Based Display Color

    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] OptionsInfo _optionsInfo;

    Animator _animator;
    Rigidbody _playerRb;

    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _lookAction;
    InputAction _attackAction;
    InputAction _dashAction;
    InputAction _showUIAction;

    [SerializeField] GameObject _attackCollider;
    [SerializeField] SphereCollider _dashCollider;

    //Root States
    private bool isDead = false;
    private bool isGrounded = false;

    //Sub States
    private bool isIdle = false;
    private bool isDamaged = false;
    private bool isWalking = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isAttacking = false;
    private bool isFalling = false;

    //Context vars
    private Vector3 surfaceNormal;

    private bool onPlatform;
    private bool onSlope;    
    private bool onCinematic;
    private bool onInteract;
    private bool onAction;
    private bool onKinematic;

    private bool canDMG = true;
    private bool canDash;
    private bool canAttack = true;
    private bool canJump = true;
    private bool canFreeLook = true;
    private bool canInteract = true;

    private int dashCount = 1;
    private int attackCount = 1; //Per eventuale sistema di combo
    private int jumpCount = 2;
    private float moveSpeed = 1760f;

    //private float yaw;
    //private float pitch;

    private float xAxis;
    private float yAxis;
    private const float lerpAxisSpeed = .5f;
    private const float minXAxis = -30f;
    private const float maxXAxis = 40f;

    private float gravity = 9.81f;

    [SerializeField]
    [Range(1f, 100f)] float gravityMultiplier;

    [SerializeField]
    [Range(0.1f, 10f)] float gravitySpeed;

    [SerializeField]
    [Range(0.1f, 80f)] float jumpHeight;    

    [SerializeField]
    [Range(0f, 35f)] float slopeAngle;

    //Input vars
    private Vector2 camInput;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;

    //Constructors   
    public float Gravity { get { return gravity; } set { gravity = value; } }
    public float GravityMultiplier { get { return gravityMultiplier; } }
    public float GravitySpeed { get { return gravitySpeed; } }

    public Vector2 CamInput { get { return camInput; } }
    public Vector2 MoveInput { get { return moveInput; } }
    public bool JumpInput { get { return jumpInput; } set { jumpInput = value; } }
    public bool AttackInput { get { return attackInput; } set { attackInput = value; } }
    public bool DashInput { get { return dashInput; } set { dashInput = value; } }

    public bool OnSlope { get { return onSlope; } }
    public bool OnPlatform { get { return onPlatform; } set { onPlatform = value; } }
    public bool OnCinematic { get { return onCinematic; } }
    public bool OnInteract { get { return onInteract; } set { onInteract = value; } }
    public bool OnAction { get { return onAction; } }
    public bool OnKinematic { get { return onKinematic; } set { onKinematic = value; } }

    public Vector3 SurfaceNormal { get { return surfaceNormal; } }
    public int DashCount { get { return dashCount; } set { dashCount = value; } }
    public int JumpCount { get { return jumpCount; } set { jumpCount = value; } }
    public int AttackCount { get { return attackCount; } set { attackCount = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public bool CanDash { get { return canDash; } set { canDash = value; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool CanJump { get { return canJump; } set { canJump = value; } }
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }
    public bool CanFreeLook { get { return canFreeLook; } }
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }

    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public bool IsIdle { get { return isIdle; } set { isIdle = value; } }
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool IsDamaged { get { return isDamaged; } set { isDamaged = value; } }
    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsDashing { get { return isDashing; } set { isDashing = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public bool IsFalling { get { return isFalling; } set { isFalling = value; } }
    
    public GameObject RingPoint { get { return _ringPoint; } } // VFX Spawn ref
    public GameObject DashPoint { get { return _dashPoint; } } // VFX Spawn ref
    public GameObject AttackPoint { get { return _attackPoint; } } // VFX Spawn ref
    public GameObject JumpPoint { get { return _jumpPoint; } } // VFX Spawn ref

    public GameObject Player { get { return _player; } }
    public GameObject Asset { get { return _asset; } }
    public GameObject CameraHolder { get { return _camHolder; } }
    public GameObject PlayerForward { get { return _forward; } }
    public GameObject Head { get { return _head; } }
    public Rigidbody PlayerRb { get { return _playerRb; } }
    public Animator Animator { get { return _animator; } }
    public GameObject AttackCollider { get { return _attackCollider; } set { _attackCollider = value; } }
    public SphereCollider DashCollider { get { return _dashCollider; } set { _dashCollider = value; } }

    public BaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public BaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public StateHandler StateHandler { get { return _stateHandler; } set { _stateHandler = value; } }
    public AnimHandler AnimHandler { get { return _animHandler; } set { _animHandler = value; } }
    public PlayerInfo PlayerInfo { get { return _playerInfo; } }
    
    // Awake is called before the Start 
    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;

        _animator = _asset.GetComponentInChildren<Animator>();
        _playerRb = _player.GetComponent<Rigidbody>();

        _animHandler = _asset.GetComponentInChildren<AnimHandler>();
        _stateHandler = new StateHandler(this, _animHandler);

        _currentRootState = StateHandler.Airborne();
        _currentRootState.EnterState();

        _currentSubState = StateHandler.Fall();
        _currentSubState.EnterState();

        GameBucket.Instance.PXController = this;
    }

    // Start is called before the first frame update
    void Start() {
        InitializeActions();
        InitializePowerUps();
        SubscribeCallbacks();

        //CalculateMouseCamMotion(new Vector2(.1f, .1f), _optionsInfo.MouseSens);
    }

    // Update is called once per frame
    void Update() {
        EvaluateHealth();

        _forward.transform.position = _asset.transform.position;
        //_virtualCamera.transform.position = _asset.transform.position;
    }

    void FixedUpdate() {       
        _currentRootState.UpdateState();
        if (!isDead) { 
            _currentSubState.UpdateState();
        }
    }

    private void LateUpdate() {        
        if (canFreeLook) {
            UpdateCamera(camInput);
        }        
    }

    public void OnDestroy() {
        UnsubscribeCallbacks();
    }

    //Input Callbacks
    public void OnLook(InputAction.CallbackContext input) {        
        if (canFreeLook && input.ReadValue<Vector2>() != Vector2.zero) {
            camInput = input.ReadValue<Vector2>();            
        } else {
            camInput = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext input) {
        if (!canJump) { return; }

        if (input.ReadValue<float>() != 0f) {            
            SetUpJump();
        }
    }

    public void OnAttack(InputAction.CallbackContext input) {
        if (canInteract) { return; }

        if (input.ReadValue<float>() != 0f) {
            SetUpAttack();
        }
    }

    public void OnDash(InputAction.CallbackContext input) {
        if (input.ReadValue<float>() != 0f) {
            SetUpDash();
        }
    }

    public void OnMove(InputAction.CallbackContext input) {
        moveInput = input.ReadValue<Vector2>();
    }     

    public void OnShowUI(InputAction.CallbackContext input) {
        if (onInteract) { return; }

        if (input.ReadValue<float>() != 0f) {            
            //GameBucket.Instance.GameCanvasHandler.ShowUI();
        }
    }

    private void SubscribeCallbacks() {
        _jumpAction.started += OnJump;        

        _attackAction.started += OnAttack;        

        _dashAction.started += OnDash;        

        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;

        _lookAction.started += OnLook;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;

        _showUIAction.started += OnShowUI;         
    }

    private void UnsubscribeCallbacks() {
        _jumpAction.started -= OnJump;        

        _attackAction.started -= OnAttack;        

        _dashAction.started -= OnDash;        

        _moveAction.started -= OnMove;
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;

        _lookAction.started -= OnLook;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;

        _showUIAction.started -= OnShowUI;
    }

    private void InitializeActions() {
        //Player Actions
        _moveAction = InputManager.Instance.GetPlayerInput().actions["Move"];
        _jumpAction = InputManager.Instance.GetPlayerInput().actions["Jump"];
        _lookAction = InputManager.Instance.GetPlayerInput().actions["Look"];
        _attackAction = InputManager.Instance.GetPlayerInput().actions["Attack"];
        _dashAction = InputManager.Instance.GetPlayerInput().actions["Dash"];
        _showUIAction = InputManager.Instance.GetPlayerInput().actions["ShowUI"];
    }

    //External Callbacks
    public void InteractionEnter(Transform playerPos, Transform focusTarget, GameObject _vcam) {        
        onInteract = true;
        canFreeLook = false;
        canInteract = false;

        InputManager.Instance.SetActionMap("Dialog");
        StartCoroutine(InteractRoutine(playerPos, focusTarget, _vcam));
    }

    public void CinematicEnter(Transform playerPos, Transform focusTarget, GameObject _vcam) { 
        onCinematic = true;
        canFreeLook = false;
        canInteract = false;

        InputManager.Instance.SetActionMap("Disabled");
        StartCoroutine(CinematicRoutine(playerPos, focusTarget, _vcam));
    }

    public void InteractionExit() {
        //onDialog = false;
        //CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
        if (!onAction) {
            CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
            canFreeLook = true;
        }        

        //Debug.Log("Switching inputmap from InteractionExit");
        InputManager.Instance.SetActionMap("Player");        

        onInteract = false;
    }

    public void CinematicExit() {
        if (!onAction) {
            CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
            canFreeLook = true;
        }

        //Debug.Log("Switching inputmap from CinematicExit");
        InputManager.Instance.SetActionMap("Player");

        onCinematic = false;
    }

    public void ActionCameraEnter() {
        canFreeLook = false;
        onAction = true;
    }

    public void ActionCameraExit() {
        CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
        canFreeLook = true;
        onAction = false;
    }

    //Collision Callbacks
    private void OnTriggerEnter(Collider other) {        
        if (other.tag == "Enemy") {
            SetDMGState();
        }
        if (other.tag == "Platform") {
            onPlatform = true;
            _playerparent.transform.SetParent(other.transform); //Platform fix (1)
        }
        if (other.tag == "Death") {
            isDead = true;
        }
        if (other.tag == "PowerUps") {
            InitializePowerUps();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Platform") {
            onPlatform = false;
            _playerparent.transform.SetParent(null); //Platform fix (2)
        }        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Enemy") {
            SetDMGState();
        }
        if (collision.collider.tag == "Death") {
            isDead = true;            
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Enemy") {
            SetDMGState();
        }
        if (collision.collider.tag == "Slope") {
            float angle = Vector3.Angle(PlayerRb.transform.up, collision.GetContact(0).normal);
            SetSlope(angle, collision.GetContact(0).normal);
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.tag == "Slope") {
            onSlope = false;
        }
    }
    
    //SetUp Methods
    private void SetUpJump() {
        if (attackInput || dashInput) { return; }
        
        if (jumpCount > 0) {
            jumpInput = true;
            SetJumpState();
        } else {
            canJump = false;
            jumpInput = false;
            return;
        }
    }

    private void SetUpDash() {
        if (attackInput || jumpInput) { return; }

        if (!dashInput && canDash) {
            SetDashState();
        }
        else {
            return;
        }
    }

    private void SetUpAttack() {
        if (dashInput || jumpInput) { return; }

        if (!attackInput && attackCount > 0) {
            SetAttackState();
        }
        else {
            return;
        }
    }

    private void SetJumpState() {
        if (!canJump || isDamaged) {
            jumpInput = false;
            return; 
        }

        isJumping = true;             
    }

    private void SetDashState() {
        if (!canDash || isAttacking) { return; }         
        
        if (!isDamaged) {
            dashInput = true;
            isDashing = true;
            canDash = false;
        }
    }

    private void SetAttackState() {
        if (!canAttack || isDashing) { return; }

        if (!isDamaged) {
            attackInput = true;
            isAttacking = true;
            canDMG = false;
            canAttack = false;
        }
    }   

    private void SetSlope(float angle, Vector3 _surfaceNormal) {
        if (angle <= slopeAngle) {
            onSlope = true;
            surfaceNormal = _surfaceNormal;
        }
    }

    private void SetDMGState() {
        if (!canDMG) { return; }

        if (!isDashing) { 
            isDamaged = true;
        }
    }

    private void InitializePowerUps() {
        if (_playerInfo.PowerUps >= 2) {
            canDash = true;
        }
        else if (_playerInfo.PowerUps == 1) {
            jumpCount = 2;
        }
        else if (_playerInfo.PowerUps <= 0) {
            jumpCount = 1;
        }
    }

    private void EvaluateHealth() {
        switch (_playerInfo.CurrentHp) {
            case 0: {
                    isDead = true;
                    break;
                }
            case 1: {
                    _visor.SetFloat("_HealthDisplayColor", (float)PlayerHealthRange.LOW);
                    break;
                }
            case 2: {
                    _visor.SetFloat("_HealthDisplayColor", (float)PlayerHealthRange.MID);
                    break;
                }
            case 3: {
                    _visor.SetFloat("_HealthDisplayColor", (float)PlayerHealthRange.HIGH);
                    break;
                }
            default: {
                    break;
                }
        }
    }   

    private float ComputeDistance2D(Transform _a, Transform _b) {
        Vector2 a = new Vector2(_a.position.x, _a.position.z);
        Vector2 b = new Vector2(_b.position.x, _b.position.z);

        float distance = Mathf.Abs(Vector2.Distance(a, b));

        return distance;
    }

    public Vector3 ComputeForward2D(Vector3 _head, Vector3 _tail) {
        Vector2 head = new Vector2(_head.x, _head.z);
        Vector2 tail = new Vector2(_tail.x, _tail.z);

        Vector2 targetForward = (head - tail).normalized;

        Vector3 forward = new Vector3(targetForward.x, 0f, targetForward.y);

        return forward;
    }

    public void SetKinematic() {
        StartCoroutine(EvaluateKinematic());
    }

    //Camera Methods
    public void UpdateExternalCamera(Vector3 playerPos, Vector3 cameraPivot) {
        //Updates player forward direction while inside ActionCameraBlock
        if (onCinematic) { return; }

        Vector3 targetForward = ComputeForward2D(playerPos, cameraPivot);
        float currentAngle = Vector3.Angle(_forward.transform.forward, targetForward);

        if (currentAngle > 1f) {
            Vector3 lerpForward = Vector3.Lerp(_forward.transform.forward, targetForward, .05f);
            _camHolder.transform.forward = lerpForward;
            _forward.transform.forward = lerpForward;            

        } else {
            _camHolder.transform.forward = targetForward; 
            _forward.transform.forward = targetForward;
            
            yAxis = _camHolder.transform.rotation.eulerAngles.y;
            xAxis = _camHolder.transform.rotation.eulerAngles.x;        
        }
    }

    private void UpdateCamera(Vector2 camInput) { 
        if (!isDead && !onCinematic && !onInteract) {
            EvaluateCamera(camInput);
        }        
    }

    private void EvaluateCamera(Vector2 camInput) {
        if (InputManager.Instance.GetPlayerInput().currentControlScheme == "Keyboard&Mouse") {
            CalculateMouseCamMotion(camInput, _optionsInfo.MouseSens);
        }
        else {
            CalculatePadCamMotion(camInput, _optionsInfo.PadSens);
        }
    }

    private void CalculateMouseCamMotion(Vector2 mouseInput, float sens) {
        yAxis = _forward.transform.rotation.eulerAngles.y;
        xAxis = _camHolder.transform.rotation.eulerAngles.x;

        float targetY = mouseInput.x * sens * Mathf.PI * Time.deltaTime;
        float targetX = mouseInput.y * sens * Mathf.PI * Time.deltaTime;        

        yAxis += targetY;
        xAxis -= targetX;

        float lerpY = _forward.transform.rotation.eulerAngles.y;
        float lerpX = _camHolder.transform.rotation.eulerAngles.x;

        lerpY = Mathf.LerpAngle(lerpY, yAxis, lerpAxisSpeed);
        lerpX = Mathf.LerpAngle(lerpX, xAxis, lerpAxisSpeed);

        if (lerpX > 180f) lerpX -= 360f;

        lerpX = Mathf.Clamp(lerpX, minXAxis, maxXAxis);

        _camHolder.transform.rotation = Quaternion.Euler(lerpX, lerpY, 0f);
        _forward.transform.rotation = Quaternion.Euler(0f, lerpY, 0f);
    }

    private void CalculatePadCamMotion(Vector2 mouseInput, float sens) {
        float targetY = mouseInput.x * sens * Mathf.PI * Time.deltaTime;
        float targetX = mouseInput.y * sens * Mathf.PI * Time.deltaTime;

        yAxis += targetY;
        xAxis -= targetX;

        yAxis = Mathf.Repeat(yAxis, 360);
        xAxis = Mathf.Clamp(xAxis, -30f, 50f);

        _camHolder.transform.rotation = Quaternion.Euler(xAxis, yAxis, 0f);
        _forward.transform.rotation = Quaternion.Euler(0f, yAxis, 0f);
    }

    private void HandleAttack() {
        _playerRb.velocity.Set(0f, 0f, 0f);
        _playerRb.AddForce(DashDirection() * 8f, ForceMode.Impulse);
        
        if (!isGrounded) {
            _playerRb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
        }
    }

    public void HandleDash() {
        dashInput = false;

        VFXManager.Instance.SpawnFixedVFX(PlayerVFX.AirRing, _ringPoint.transform.position, _ringPoint.transform.rotation);
        VFXManager.Instance.SpawnFollowVFX(PlayerVFX.DashTrail, _ringPoint.transform.position, _ringPoint.transform.rotation, _dashPoint);

        _playerRb.velocity.Set(0f, 0f, 0f);
        _playerRb.AddForce(DashDirection() * 25f, ForceMode.Impulse);
    }

    private Vector3 DashDirection() {
        if (onSlope) {
            Vector3 direction = Vector3.ProjectOnPlane(_asset.transform.forward, surfaceNormal);
            return direction;
        }
        else return _asset.transform.forward;
    }

    //Animator Signals
    public void HandleSignal(int _sig) {
        switch (_sig) {
            case (int)AnimatorSignal.attackSig: {
                    AttackSignal();

                    break;
                }
            case (int)AnimatorSignal.jumpSig: {
                    JumpSignal();

                    break;
                }
            case (int)AnimatorSignal.dashSig: {
                    DashSignal();

                    break;
                }
            case (int)AnimatorSignal.k_attackSig: {
                    KinematicAttackSignal();

                    break;
                }
            case (int)AnimatorSignal.s_dashSig: {
                    DashStartSignal();

                    break;
                }
            case (int)AnimatorSignal.jumpStartSig: {
                    JumpStartSignal();

                    break;
                }
            default: break;                    
        }
        
    }

    private void JumpStartSignal() {        
        //Debug.Log("JumpStartSignal");
    }

    private void JumpSignal() {
        if (!jumpInput) {
            isJumping = false;
            canJump = true;

            if (!IsGrounded) {            
                isFalling = true; 
            } else {
                isIdle = true;
            }
        }

    }

    private void AttackSignal() {
        isAttacking = false;

        if (!IsGrounded) {
            isFalling = true;
        }
        else {
            isIdle = true;
        }
    }

    private void DashSignal() {
        isDashing = false;
    }

    private void DashStartSignal() {
        _playerRb.isKinematic = false;

    }
    
    private void KinematicAttackSignal() {
        _playerRb.isKinematic = false;

        if (isGrounded) {
            HandleAttack();
        }
    }

    //Coroutine
    private IEnumerator EvaluateKinematic() {
        yield return new WaitWhile(() => _playerRb.isKinematic);

        yield return null;

        if (IsAttacking) {
            HandleAttack();
        }
        else if (IsDashing) {
            HandleDash();
        }

        yield break;
    }

    public IEnumerator InitializeMoveSpeed() {
        while (moveSpeed > 600f) {
            moveSpeed = moveSpeed - ((moveSpeed * .6f) * Time.deltaTime);
            yield return null;
        }   
        yield break;
    }

    public IEnumerator ResetAttack() {
        canAttack = false;

        yield return new WaitWhile(() => isAttacking);

        canDMG = true;

        yield return new WaitForSeconds(.1f);

        canAttack = true;

        if (isGrounded) {
            attackCount = 1;
        }

        yield break;
    }

    public IEnumerator ResetDash() {
        yield return new WaitWhile(() => isDashing);

        if (isGrounded) {
            yield return new WaitForSeconds(.6f);
            canDash = true;
            yield break;
        }

        yield return new WaitForSeconds(2f);
        canDash = true;

        yield break;
    }

    public IEnumerator ResetDMG() {
        canDMG = false;
        yield return new WaitForSeconds(.2f);
        isDamaged = false;
        canDMG = true;
        yield break;
    }

    private IEnumerator InteractRoutine(Transform playerTarget, Transform focusTarget, GameObject _vcam) {
        Transform _vcamTransform = _vcam.transform;        

        Vector3 targetForward = ComputeForward2D(_vcamTransform.position, playerTarget.position);
        CameraManager.Instance.SwitchGameVCamera(_vcam);

        CinemachineHardLockToTarget camBody = _virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>();
        CinemachineCollider camCollider = _virtualCamera.GetComponent<CinemachineCollider>();

        float colliderDamping = 1f;
        float cameraDamping = .8f;

        camCollider.enabled = false;
        camCollider.m_Damping = 0f;
        camBody.m_Damping = 0f;

        yield return null;
        
        while (ComputeDistance2D(_asset.transform, playerTarget) > .15f) {
            _camHolder.transform.forward = _vcamTransform.forward;

            targetForward = ComputeForward2D(playerTarget.position, _asset.transform.position); 
            _forward.transform.forward = targetForward;

            yAxis = _camHolder.transform.rotation.eulerAngles.y;
            xAxis = _camHolder.transform.rotation.eulerAngles.x;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }        

        moveInput = new Vector2(0f, 0f);        

        isWalking = false;

        _playerRb.velocity = Vector3.zero;
        _playerRb.ResetInertiaTensor();

        _player.transform.position = new Vector3(playerTarget.transform.position.x, _player.transform.position.y, playerTarget.transform.position.z);        

        targetForward = ComputeForward2D(focusTarget.position, _player.transform.position);
        _asset.transform.forward = targetForward;
        yield return null;

        //Forward and camera after reaching the target point     
        
        while (onInteract) {
            targetForward = ComputeForward2D(playerTarget.position, _vcamTransform.position);
            _forward.transform.forward = targetForward;
            _camHolder.transform.forward = _vcamTransform.forward;

            yAxis = _forward.transform.rotation.eulerAngles.y;
            xAxis = _camHolder.transform.rotation.eulerAngles.x;
            yield return null;
        }        

        //Reset                                        
        camCollider.enabled = true;
        camCollider.m_Damping = colliderDamping;
        camBody.m_Damping = cameraDamping;
        yield return null;

        /*
        targetForward = ComputeForward2D(playerTarget, _vcamTransform);
        _forward.transform.forward = targetForward;
        _camHolder.transform.forward = _vcamTransform.forward;

        yAxis = _forward.transform.rotation.eulerAngles.y;
        xAxis = _camHolder.transform.rotation.eulerAngles.x;
        yield return null;        
        */

        ////This should be inside an async method (EXIT INTERACTION)
        /*
        if (!onAction) {
            CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
        }

        yield return null;

        Debug.Log("Switching inputmap from InteractRoutine");
        InputManager.Instance.SetActionMap("Player");
        
        if (!onAction) {
            canFreeLook = true;
        }

        yield return null;
        onInteract = false;
        canInteract = true;        
        */

        yield break;
    }

    private IEnumerator CinematicRoutine(Transform playerTarget, Transform focusTarget, GameObject _vcam) {
        Transform _vcamTransform = _vcam.transform;

        Vector3 targetForward = ComputeForward2D(_vcamTransform.position, playerTarget.position);        
        CameraManager.Instance.SwitchGameVCamera(_vcam);

        CinemachineHardLockToTarget camBody = _virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>();
        CinemachineCollider camCollider = _virtualCamera.GetComponent<CinemachineCollider>();

        float colliderDamping = 1f;
        float cameraDamping = .8f;

        camCollider.enabled = false;
        camCollider.m_Damping = 0f;
        camBody.m_Damping = 0f;

        yield return null;

        while (ComputeDistance2D(_asset.transform, playerTarget) > .15f) {
            _camHolder.transform.forward = _vcamTransform.forward;

            targetForward = ComputeForward2D(playerTarget.position, _asset.transform.position);
            _forward.transform.forward = targetForward;

            yAxis = _camHolder.transform.rotation.eulerAngles.y;
            xAxis = _camHolder.transform.rotation.eulerAngles.x;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }

        moveInput = new Vector2(0f, 0f);

        isWalking = false;

        _playerRb.velocity = Vector3.zero;
        _playerRb.ResetInertiaTensor();

        _player.transform.position = new Vector3(playerTarget.transform.position.x, _player.transform.position.y, playerTarget.transform.position.z);

        targetForward = ComputeForward2D(focusTarget.position, _player.transform.position);
        _asset.transform.forward = targetForward;
        yield return null;

        //Forward and camera after reaching the target point     

        while (onCinematic) {
            targetForward = ComputeForward2D(playerTarget.position, _vcamTransform.position);
            _forward.transform.forward = targetForward;
            _camHolder.transform.forward = _vcamTransform.forward;

            yAxis = _forward.transform.rotation.eulerAngles.y;
            xAxis = _camHolder.transform.rotation.eulerAngles.x;
            yield return null;
        }

        targetForward = ComputeForward2D(playerTarget.position, _vcamTransform.position);
        _forward.transform.forward = targetForward;
        _camHolder.transform.forward = _vcamTransform.forward;

        yAxis = _forward.transform.rotation.eulerAngles.y;
        xAxis = _camHolder.transform.rotation.eulerAngles.x;
        yield return null;

        //Reset                                        
        camCollider.enabled = true;
        camCollider.m_Damping = colliderDamping;
        camBody.m_Damping = cameraDamping;
        yield return null;

        //This should be inside an async method (EXIT CINEMATIC)
        /*
        if (!onAction) {
            CameraManager.Instance.SwitchGameVCamera(_virtualCamera.gameObject);
            canFreeLook = true;
        }

        yield return null;

        Debug.Log("Switching inputmap from CinematicController");
        InputManager.Instance.SetActionMap("Player");
        */
        yield break;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_asset.transform.position, 3f);
    }
    //
    //
} // 1000 lines of code!
