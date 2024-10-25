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
    [SerializeField] GameObject _cam;
    [SerializeField] GameObject _virtualCamera;
    [SerializeField] GameObject _forward;
    [SerializeField] GameObject _playerparent;

    [SerializeField] GameObject _dashPoint; // Spawnpoint Dash VFX
    [SerializeField] GameObject _attackPoint; // Spawnpoint Attack VFX
    [SerializeField] GameObject _jumpPoint; // Spawnpoint Jump VFX
    [SerializeField] GameObject _ringPoint; // Spawnpoint Ring VFX

    [SerializeField] Material _visor; // Material reference for Health Based Display Color

    [SerializeField] PlayerInfo _playerInfo;

    Animator _animator;
    Rigidbody _playerRb;

    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _lookAction;
    InputAction _attackAction;
    InputAction _dashAction;

    InputAction _interactAction; //Interaction

    InputAction _confirmAction; //Dialogue

    [SerializeField] SphereCollider _attackCollider;
    [SerializeField] SphereCollider _dashCollider;

    private Collider _interactionCollider;

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
    private bool onDialog;
    private bool onInteract;
    private bool canDMG = true;
    private bool canDash;
    private bool canAttack = true;
    private bool canJump = true;
    private int dashCount = 1;
    private int attackCount = 1; //Per eventuale sistema di combo
    private int jumpCount = 2;
    private float moveSpeed = 1760f;    
    private float camycurrent;
    private float camytarget;

    private float xaxis;
    private float yaxis;
    private float _currentSens = 69f;

    private float gravity = 9.81f;

    [SerializeField]
    [Range(1f, 100f)] float gravityMultiplier;

    [SerializeField]
    [Range(0.1f, 10f)] float gravitySpeed;

    [SerializeField]
    [Range(0.1f, 80f)] float jumpHeight;

    [SerializeField]
    [Range(0f, 300f)] float _sens;
    
    [SerializeField]
    [Range(0.01f, 1f)] float _sensRatio;

    [SerializeField]
    [Range(0f, 35f)] float slopeAngle;

    //Input vars
    private Vector2 camInput;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool attackInput;
    private bool dashInput;

    //Constructors
    public float CurrentSens { get { return _currentSens; } }
    public float Gravity { get { return gravity; } set { gravity = value; } }
    public float GravityMultiplier { get { return gravityMultiplier; } }
    public float GravitySpeed { get { return gravitySpeed; } }

    public Vector2 CamInput { get { return camInput; } }
    public Vector2 MoveInput { get { return moveInput; } }
    public bool JumpInput { get { return jumpInput; } set { jumpInput = value; } }
    public bool AttackInput { get { return attackInput; } set { attackInput = value; } }
    public bool DashInput { get { return dashInput; } set { dashInput = value; } }

    public Vector3 SurfaceNormal { get { return surfaceNormal; } }
    public bool OnSlope { get { return onSlope; } }
    public bool OnPlatform { get { return onPlatform; } set { onPlatform = value; } }
    public int DashCount { get { return dashCount; } set { dashCount = value; } }
    public int JumpCount { get { return jumpCount; } set { jumpCount = value; } }
    public int AttackCount { get { return attackCount; } set { attackCount = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public bool CanDash { get { return canDash; } set { canDash = value; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool CanJump { get { return canJump; } set { canJump = value; } }
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }

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
    public GameObject Camera { get { return _cam; } }
    public GameObject PlayerForward { get { return _forward; } }
    public Rigidbody PlayerRb { get { return _playerRb; } }
    public Animator Animator { get { return _animator; } }
    public SphereCollider AttackCollider { get { return _attackCollider; } set { _attackCollider = value; } }
    public SphereCollider DashCollider { get { return _dashCollider; } set { _dashCollider = value; } }

    public BaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public BaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public StateHandler StateHandler { get { return _stateHandler; } set { _stateHandler = value; } }
    public AnimHandler AnimHandler { get { return _animHandler; } set { _animHandler = value; } }
    public PlayerInfo PlayerInfo { get { return _playerInfo; } }
    
    // Awake is called before the Start 
    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        _currentSens = _sens;

        _animator = _asset.GetComponentInChildren<Animator>();
        _playerRb = _player.GetComponent<Rigidbody>();
        _animHandler = GameObject.Find("P_Asset").AddComponent<AnimHandler>();
        _stateHandler = new StateHandler(this, _animHandler);

        _currentRootState = StateHandler.Airborne();
        _currentRootState.EnterState();

        _currentSubState = StateHandler.Fall();
        _currentSubState.EnterState();

    }

    // Start is called before the first frame update
    void Start() {
        InitializeActions();
        InitializePowerUps();
        SubscribeCallbacks();
    }

    // Update is called once per frame
    void Update() {
        EvaluateHealth();
        
        if (!isDead && !onDialog) {
            UpdateCamera(_cam, _player, _forward, camInput, _currentSens);
        }
        
    }

    void FixedUpdate() {       
        _currentRootState.UpdateState();
        if (!isDead) { 
            _currentSubState.UpdateState();
        }
    }

    public void OnDestroy() {
        UnsubscribeCallbacks();
    }

    //Input Callbacks
    public void OnLook(InputAction.CallbackContext input) {
        if (input.ReadValue<Vector2>() != Vector2.zero) {
            camInput = input.ReadValue<Vector2>();
        } else {
            camInput = Vector2.zero;
        }
    }
    public void OnJump(InputAction.CallbackContext input) {
        if (onInteract) { return; }

        if (input.ReadValue<float>() != 0f) {
            SetUpJump();
        }
        else {
            jumpInput = false;
        }
    }
    public void OnAttack(InputAction.CallbackContext input) {
        if (input.ReadValue<float>() != 0f) {
            SetUpAttack();
        }
        else {
            attackInput = false;
        }
    }
    public void OnDash(InputAction.CallbackContext input) {
        if (input.ReadValue<float>() != 0f) {
            SetUpDash();
        }
        else {
            dashInput = false;
        }
    }
    public void OnMove(InputAction.CallbackContext input) {
        moveInput = input.ReadValue<Vector2>();
    } 
    public void OnInteract(InputAction.CallbackContext input) {
        if (!onInteract) { return; }

        if (input.ReadValue<float>() == 0f) { return; }

        InteractController ctx = _interactionCollider.GetComponent<InteractController>();
        ctx.OnInteract();

    }
    public void OnConfirm(InputAction.CallbackContext input) {
        if (input.ReadValue<float>() == 0f) { return; }
        
        InteractController ctx = _interactionCollider.GetComponent<InteractController>();
        ctx.OnInteract();
        
    }

    private void SubscribeCallbacks() {
        _jumpAction.started += OnJump;
        _jumpAction.performed += OnJump;
        _jumpAction.canceled += OnJump;

        _attackAction.started += OnAttack;
        _attackAction.performed += OnAttack;
        _attackAction.canceled += OnAttack;

        _dashAction.started += OnDash;
        _dashAction.performed += OnDash;
        _dashAction.canceled += OnDash;

        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;

        _lookAction.started += OnLook;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        
        _confirmAction.started += OnConfirm;
        _confirmAction.canceled += OnConfirm;

        _interactAction.started += OnInteract;
        _interactAction.canceled += OnInteract;
    }
    private void UnsubscribeCallbacks() {
        _jumpAction.started -= OnJump;
        _jumpAction.performed -= OnJump;
        _jumpAction.canceled -= OnJump;

        _attackAction.started -= OnAttack;
        _attackAction.performed -= OnAttack;
        _attackAction.canceled -= OnAttack;

        _dashAction.started -= OnDash;
        _dashAction.performed -= OnDash;
        _dashAction.canceled -= OnDash;

        _moveAction.started -= OnMove;
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;

        _lookAction.started -= OnLook;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
        
        _confirmAction.started -= OnConfirm;
        _confirmAction.canceled -= OnConfirm;
        
        _interactAction.started -= OnInteract;
        _interactAction.canceled -= OnInteract;
    }
    private void InitializeActions() {
        _moveAction = InputManager.Instance.GetPlayerInput().actions["Move"];
        _jumpAction = InputManager.Instance.GetPlayerInput().actions["Jump"];
        _lookAction = InputManager.Instance.GetPlayerInput().actions["Look"];
        _attackAction = InputManager.Instance.GetPlayerInput().actions["Attack"];
        _dashAction = InputManager.Instance.GetPlayerInput().actions["Dash"];

        _confirmAction = InputManager.Instance.GetPlayerInput().actions["Confirm"];

        _interactAction = InputManager.Instance.GetPlayerInput().actions["Interact"];
    }

    //External Callbacks
    public void DialogEnter(Transform playerPos, Transform cameraFocus, GameObject _vcam) {
        onDialog = true;
        InputManager.Instance.SetActionMap("Dialog");
        StartCoroutine(DialogRoutine(playerPos, cameraFocus, _vcam));
    }
    public void DialogExit() {
        onDialog = false;
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
        if (other.tag == "Interact") {
            onInteract = true;
            _interactionCollider = other;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Platform") {
            onPlatform = false;
            _playerparent.transform.SetParent(null); //Platform fix (2)
        }
        if (other.tag == "Interact") {
            onInteract = false;
            _interactionCollider = null;
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Enemy") {
            SetDMGState();
        }
        if (collision.collider.tag == "Death") {
            isDead = true;
            //SetUpDeath();
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
        
        if (!jumpInput) {
            jumpInput = true;
            SetJumpState();
        } else {
            return;
        }
    }
    private void SetUpDash() {
        if (attackInput || jumpInput) { return; }

        if (!dashInput) {
            dashInput = true;
            SetDashState();
        }
        else {
            return;
        }
    }
    private void SetUpAttack() {
        if (dashInput || jumpInput) { return; }

        if (!attackInput) {
            attackInput = true;
            SetAttackState();
        }
        else {
            return;
        }
    }
    private void SetJumpState() {
        if (jumpCount <= 0 || !canJump || isDamaged) { return; }

        isJumping = true;                
    }
    private void SetDashState() {
        if (!canDash) { return; }         
        
        if (!isDamaged) { 
            isDashing = true;
            canDash = false;
        }
    }
    private void SetAttackState() {
        if (!canAttack) { return; }
        if (attackCount <= 0) { return; }
        
        if (!isDamaged) {
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
    private Vector3 ComputeForward2D(Transform _a, Transform _b) {
        Vector2 a = new Vector2(_a.position.x, _a.position.z);
        Vector2 b = new Vector2(_b.position.x, _b.position.z);

        Vector2 targetForward = (a - b).normalized;

        Vector3 forward = new Vector3(targetForward.x, 0f, targetForward.y);

        return forward;
    }

    //Camera Methods
    private void UpdateCamera(GameObject cam, GameObject player, GameObject forward, Vector2 mouseInput, float sens) {
        CalculateCamMotion(mouseInput, sens);

        CamRotation(cam, forward);
    }
    private void CalculateCamMotion(Vector2 mouseInput, float sens) {
        yaxis += mouseInput.x * sens * Time.deltaTime;
        xaxis -= mouseInput.y * sens * Time.deltaTime;
        xaxis = Mathf.Clamp(xaxis, -30f, 60f);
    }
    private void CamRotation(GameObject cam, GameObject forward) {
        cam.transform.rotation = Quaternion.Euler(xaxis, yaxis, 0f);
        forward.transform.rotation = Quaternion.Euler(0f, yaxis, 0f);
    }
    private void VerticalSmoothCam(GameObject cam, GameObject player) {
        camycurrent = cam.transform.position.y;
        camytarget = player.transform.position.y;
        float camylerp = Mathf.Lerp(camycurrent, camytarget, .025f);
        if (camycurrent < camytarget) {
            cam.transform.position = new Vector3(player.transform.position.x, camylerp, player.transform.position.z);
        }
        else {
            cam.transform.position = player.transform.position;
        }
    }

    //Coroutine
    public IEnumerator InitializeMoveSpeed() {
        while (moveSpeed > 600f) {
            moveSpeed = moveSpeed - ((moveSpeed * .6f) * Time.deltaTime);
            yield return null;
        }   
        yield break;
    }
    public IEnumerator ResetAttack() {
        canAttack = false;
        yield return new WaitForSeconds(.4f);

        isAttacking = false;
        canDMG = true;

        yield return new WaitForSeconds(.1f);

        canAttack = true;
        if (isGrounded) {
            attackCount = 1;
        }
        yield break;
    }
    public IEnumerator ResetDash() {
        yield return new WaitForSeconds(.4f);

        isDashing = false;

        if (isGrounded) {
            yield return new WaitForSeconds(.6f);
            canDash = true;
            yield break;
        }

        yield return new WaitForSeconds(2f);
        canDash = true;

        yield break;
    }
    public IEnumerator ResetJump() {
        canJump = true;

        yield return new WaitForSeconds(.2f);
        isJumping = false;
        

        yield break;
    }
    public IEnumerator ResetDMG() {
        canDMG = false;
        yield return new WaitForSeconds(.2f);
        isDamaged = false;
        canDMG = true;
        yield break;
    }
    
    private IEnumerator DialogRoutine(Transform playerTarget, Transform cameraTarget, GameObject _vcam) {
        CameraManager.Instance.SwitchVirtualCamera(_virtualCamera, _vcam);

        yield return null;

        Vector3 targetForward = ComputeForward2D(playerTarget, _asset.transform);        

        while (ComputeDistance2D(_asset.transform, playerTarget) > .3f) {

            targetForward = ComputeForward2D(playerTarget, _asset.transform);
            _cam.transform.forward = targetForward;

            targetForward = ComputeForward2D(playerTarget, _asset.transform); 
            _forward.transform.forward = targetForward;

            yield return null;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }

        while (ComputeDistance2D(_asset.transform, cameraTarget) > 3f) {

            targetForward = ComputeForward2D(cameraTarget, _asset.transform);
            _cam.transform.forward = targetForward;

            targetForward = ComputeForward2D(cameraTarget, _asset.transform);
            _forward.transform.forward = targetForward;

            yield return null;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }

        targetForward = ComputeForward2D(cameraTarget, _asset.transform);
        _cam.transform.forward = targetForward;

        targetForward = ComputeForward2D(cameraTarget, _asset.transform);
        _forward.transform.forward = targetForward;

        yield return null;

        moveInput = new Vector2(0f, 1f);

        yield return null;

        ///

        moveInput = new Vector2(0f, 0f);

        yield return new WaitWhile(() => onDialog);

        yield return new WaitForSeconds(.2f);

        CameraManager.Instance.SwitchVirtualCamera(_vcam, _virtualCamera);
        InputManager.Instance.SetActionMap("Player");

        yield break;
    }
    //
}
