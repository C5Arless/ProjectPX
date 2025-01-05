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
    [SerializeField] GameObject _head;

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

    private float xaxis;
    private float yaxis;

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
    public float CurrentSens { get { return _sens; } }
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
    public bool OnDialog { get { return onDialog; } } 
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
    public bool CanInteract { get { return canInteract; } }

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
    public GameObject Head { get { return _head; } }
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

    }

    // Update is called once per frame
    void Update() {
        EvaluateHealth();

        _forward.transform.position = _asset.transform.position;

        
        if (canFreeLook) {
            UpdateCamera(camInput);
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
        if (canFreeLook && input.ReadValue<Vector2>() != Vector2.zero) {
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
    }

    public void OnAttack(InputAction.CallbackContext input) {
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

    public void OnInteract(InputAction.CallbackContext input) {
        if (!onInteract) { return; }

        if (input.ReadValue<float>() != 0f) { 
            InteractController ctx = _interactionCollider.GetComponent<InteractController>();
            ctx.OnInteract();             
        }

    }

    public void OnConfirm(InputAction.CallbackContext input) {
        if (input.ReadValue<float>() != 0f) { 
            InteractController ctx = _interactionCollider.GetComponent<InteractController>();
            ctx.OnInteract();

            //Debug.Log("OnConfirm from Dialog ActionMap");
        }        
        
    }

    private void SubscribeCallbacks() {
        _jumpAction.started += OnJump;
        //_jumpAction.performed += OnJump;
        //_jumpAction.canceled += OnJump;

        _attackAction.started += OnAttack;
        //_attackAction.performed += OnAttack;
        //_attackAction.canceled += OnAttack;

        _dashAction.started += OnDash;
        //_dashAction.performed += OnDash;
        //_dashAction.canceled += OnDash;

        _moveAction.started += OnMove;
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;

        _lookAction.started += OnLook;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        
        _confirmAction.started += OnConfirm;
        //_confirmAction.canceled += OnConfirm;

        _interactAction.started += OnInteract;
        //_interactAction.canceled += OnInteract;
    }

    private void UnsubscribeCallbacks() {
        _jumpAction.started -= OnJump;
        //_jumpAction.performed -= OnJump;
        //_jumpAction.canceled -= OnJump;

        _attackAction.started -= OnAttack;
        //_attackAction.performed -= OnAttack;
        //_attackAction.canceled -= OnAttack;

        _dashAction.started -= OnDash;
        //_dashAction.performed -= OnDash;
        //_dashAction.canceled -= OnDash;

        _moveAction.started -= OnMove;
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;

        _lookAction.started -= OnLook;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
        
        _confirmAction.started -= OnConfirm;
        //_confirmAction.canceled -= OnConfirm;
        
        _interactAction.started -= OnInteract;
        //_interactAction.canceled -= OnInteract;
    }

    private void InitializeActions() {
        //Player Actions
        _moveAction = InputManager.Instance.GetPlayerInput().actions["Move"];
        _jumpAction = InputManager.Instance.GetPlayerInput().actions["Jump"];
        _lookAction = InputManager.Instance.GetPlayerInput().actions["Look"];
        _attackAction = InputManager.Instance.GetPlayerInput().actions["Attack"];
        _dashAction = InputManager.Instance.GetPlayerInput().actions["Dash"];
        _interactAction = InputManager.Instance.GetPlayerInput().actions["Interact"];

        //Dialog Actions
        _confirmAction = InputManager.Instance.GetPlayerInput().actions["Confirm"];

    }

    //External Callbacks
    public void DialogEnter(Transform playerPos, Transform focusTarget, GameObject _vcam) {
        onDialog = true;
        canFreeLook = false;
        canInteract = false;

        InputManager.Instance.SetActionMap("Dialog");
        StartCoroutine(DialogRoutine(playerPos, focusTarget, _vcam));
    }

    public void CinematicEnter(Transform playerPos, Transform focusTarget, GameObject _vcam) {
        onDialog = true;
        canFreeLook = false;
        canInteract = false;

        InputManager.Instance.SetActionMap("Disabled");
        StartCoroutine(DialogRoutine(playerPos, focusTarget, _vcam));
    }

    public void InteractionExit() {
        onDialog = false;

        if (!onAction) {
            canFreeLook = true;
        }
    }

    public void ActionCameraEnter() {
        canFreeLook = false;
        onAction = true;
    }

    public void ActionCameraExit() {
        CameraManager.Instance.SwitchGameVCamera(_virtualCamera);
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
        
        if (!jumpInput && jumpCount > 0) {
            jumpInput = true;
            SetJumpState();
        } else {
            return;
        }
    }

    private void SetUpDash() {
        if (attackInput || jumpInput) { return; }

        if (!dashInput && canDash) {
            dashInput = true;
            SetDashState();
        }
        else {
            return;
        }
    }

    private void SetUpAttack() {
        if (dashInput || jumpInput) { return; }

        if (!attackInput && canAttack) {
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

    public void SetKinematic() {
        StartCoroutine(EvaluateKinematic());
    }

    //Camera Methods
    public void UpdateExternalCamera(Transform playerPos, Transform cameraPivot) {
        Vector3 targetForward = ComputeForward2D(playerPos, cameraPivot);
        float currentAngle = Vector3.Angle(_forward.transform.forward, targetForward);

        if (currentAngle > 1f) {

            Vector3 lerpForward = Vector3.Lerp(_forward.transform.forward, targetForward, .1f);
            _cam.transform.forward = lerpForward;
            _forward.transform.forward = lerpForward;            

        } else {
            _cam.transform.forward = targetForward; 
            _forward.transform.forward = targetForward;

            yaxis = _cam.transform.rotation.eulerAngles.y;
            xaxis = _cam.transform.rotation.eulerAngles.x;        
        }

        /*
        _cam.transform.forward = ComputeForward2D(playerPos, cameraPivot);        
        _forward.transform.forward = ComputeForward2D(playerPos, cameraPivot);

        yaxis = _cam.transform.rotation.eulerAngles.y;
        xaxis = _cam.transform.rotation.eulerAngles.x;
        */
    }

    private void UpdateCamera(Vector2 camInput) {        
        if (!isDead && !onDialog) {
            UpdateFreeLookCamera(_cam, _forward, camInput, _sens);
        }        
    }

    private void UpdateFreeLookCamera(GameObject cam, GameObject forward, Vector2 mouseInput, float sens) {
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

    //Animator Signals
    public void HandleSignal(int signal) {
        switch (signal) {
            case 1: {
                    AttackSignal();

                    break;
                }
            case 2: {
                    JumpSignal();

                    break;
                }
            case 3: {
                    DashSignal();

                    break;
                }
            case 4: {
                    KinematicSignal();

                    break;
                }
            default: break;                    
        }
        
    }

    private void JumpSignal() {
        isJumping = false;

        if (!IsGrounded) { 
            isFalling = true; 
        } else {
            isIdle = true;
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

    private void KinematicSignal() {
        onKinematic = true;
    }

    //Coroutine
    private IEnumerator EvaluateKinematic() {
        yield return new WaitUntil(() => onKinematic);

        _playerRb.isKinematic = true;
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

        yield break;
    }

    public IEnumerator ResetDash() {
        yield return new WaitWhile(() => isAttacking);

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

        yield return new WaitWhile(() => isJumping);

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
        CameraManager.Instance.SwitchGameVCamera(_vcam);

        yield return null;

        Vector3 targetForward = ComputeForward2D(playerTarget, _asset.transform);        

        while (ComputeDistance2D(_asset.transform, playerTarget) > .6f) {

            targetForward = ComputeForward2D(playerTarget, _asset.transform);
            _cam.transform.forward = targetForward;

            targetForward = ComputeForward2D(playerTarget, _asset.transform); 
            _forward.transform.forward = targetForward;

            yield return null;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }

        /*
        while (ComputeDistance2D(_asset.transform, cameraTarget) > .6f) {

            targetForward = ComputeForward2D(cameraTarget, _asset.transform);
            _cam.transform.forward = targetForward;

            targetForward = ComputeForward2D(cameraTarget, _asset.transform);
            _forward.transform.forward = targetForward;

            yield return null;

            moveInput = new Vector2(0f, 1f);

            yield return null;
        }
        */

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

        if (!onAction) {
            CameraManager.Instance.SwitchGameVCamera(_virtualCamera);
        }

        onInteract = false;

        yield return null;

        canInteract = true;
        InputManager.Instance.SetActionMap("Player");

        yield break;
    }
}
