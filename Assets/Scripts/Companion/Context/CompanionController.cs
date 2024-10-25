using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class CompanionController : MonoBehaviour {
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] VisualEffect _spark;
    [SerializeField] VisualEffect _trail;
    [SerializeField] Collider _stuckCollider;

    [SerializeField] Transform _asset;
    [SerializeField] Transform _focusDefaultPoint;
    [SerializeField] Transform _focusDefaultPivot;
    [SerializeField] Transform _focusPivot;
    [SerializeField] Transform _focusPoint;

    [SerializeField]
    [Range(0f, 5f)] float horizontalOffset;

    [SerializeField]
    [Range(0f, 5f)] float verticalOffset;

    [SerializeField]
    [Range(0f, 10f)] float maxVelocity;

    [SerializeField]
    [Range(0f, 10f)] float limitDistance;
    
    private Transform _playerHead;

    private CBaseState _currentRootState;
    private CBaseState _currentSubState;
    private CompanionStateHandler _stateHandler;    
    
    //Root States
    private bool isStuck = false;
    private bool isOperative = true;

    //Sub States
    private bool isIdle = false;
    private bool isMoving = false;
    private bool isTalking = false;   
    private bool isUnstucking = false;

    //VISION MODULE
    private Transform visionDiscoveryTransform;
    private Vector3 visionFocusPoint;    
    private bool visionLocked;

    //TRAVEL MODULE
    private Vector3[] travelDestinations = new Vector3[8];
    private Vector3 travelPosition;
    private bool travelLocked = false;

    public bool[] exclusionList = new bool[8];
    private int rngToken = 0;
    private float currentVelocity = 0f;

    private float limitDistanceMax;
    private float playerDistance;

    public VisualEffect Spark { get { return _spark; } set { _spark = value; } }
    public VisualEffect Trail { get { return _trail; } set { _trail = value; } }

    public CBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public CBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public CompanionStateHandler StateHandler { get { return _stateHandler; } set { _stateHandler = value; } }

    public Transform PlayerHead { get { return _playerHead; } set { _playerHead = value; } }
    public Collider StuckCollider { get { return _stuckCollider; } set { _stuckCollider = value; } }

    public bool IsStuck { get { return isStuck; } set { isStuck = value; } }
    public bool IsOperative { get { return isOperative; } set { isOperative = value; } }
    public bool IsIdle { get { return isIdle; } set { isIdle = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public bool IsTalking { get { return isTalking; } set { isTalking = value; } }
    public bool IsUnstucking { get { return isUnstucking; } set { isUnstucking = value; } }

    public float HorizontalOffset { get { return horizontalOffset; } }
    public float VerticalOffset { get { return verticalOffset; } }
    public float MaxVelocity { get { return maxVelocity; } }
    public float LimitDistance { get { return limitDistance; } }

    public Transform VisionDiscoveryTransform { get { return visionDiscoveryTransform; } set { visionDiscoveryTransform = value; } }
    public Vector3 VisionFocusPoint { get { return visionFocusPoint; } set { visionFocusPoint = value; } }    
    public bool VisionLocked { get { return visionLocked; } set { visionLocked = value; } }

    public Vector3[] TravelDestinations { get { return travelDestinations; } set { travelDestinations = value; } }
    public Vector3 TravelPosition { get { return travelPosition; } set { travelPosition = value; } }
    public bool TravelLocked { get { return travelLocked; } set { travelLocked = value; } }

    public int RNGToken { get { return rngToken; } }
    public float CurrentVelocity { get { return currentVelocity; } set { currentVelocity = value; } }
    public float LimitDistanceMax { get { return limitDistanceMax; } set { limitDistanceMax = value; } }
    public float PlayerDistance { get { return playerDistance; } set { playerDistance = value; } }

    void Awake() {
        _stateHandler = new CompanionStateHandler(this);
        _playerHead = GameObject.Find("PlayerHead").transform;

        limitDistanceMax = limitDistance * 2.5f;
        
        _currentRootState = StateHandler.Operative();
        _currentRootState.EnterState();

        _currentSubState = StateHandler.Idle();
        _currentSubState.EnterState();

        InitializeTravelDestinations();
        
    }

    // Update is called once per frame
    void Update() {
        EvaluateHealth();

        _currentRootState.UpdateState();
        _currentSubState.UpdateState();                 

    }

    private void InitializeTravelDestinations() {
        travelDestinations[0] = new Vector3(horizontalOffset, verticalOffset, -horizontalOffset);
        travelDestinations[1] = new Vector3(0f, verticalOffset, -horizontalOffset);
        travelDestinations[2] = new Vector3(-horizontalOffset, verticalOffset, -horizontalOffset);
        travelDestinations[3] = new Vector3(-horizontalOffset, verticalOffset, 0f);
        travelDestinations[4] = new Vector3(-horizontalOffset, verticalOffset, horizontalOffset);
        travelDestinations[5] = new Vector3( 0f, verticalOffset, horizontalOffset);
        travelDestinations[6] = new Vector3(horizontalOffset, verticalOffset, horizontalOffset);
        travelDestinations[7] = new Vector3(horizontalOffset, verticalOffset, 0f);
    }

    private void EvaluateHealth() {
        switch (_playerInfo.CurrentHp) {
            case 1: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.LOW);
                    break;
                }
            case 2: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.MID);
                    break;
                }
            case 3: {
                    _spark.SetInt("HealthDisplayRange", (int)PlayerHealthRange.HIGH);
                    break;
                }
            default: {
                    break;
                }
        }
    }

    private Vector3 ComputeTravelPosition() {
        return _playerHead.position + travelDestinations[rngToken - 1];
    }

    public void ExitTalkState() {
        IsTalking = false;
    }

    public void StorePlayerDistance() {
        playerDistance = Mathf.Abs(Vector3.Distance(transform.position, _playerHead.position));
    }        

    public void UpdateRNGToken() {
        StartCoroutine("CycleRNGToken");
    }   

    public void VisionEnterIdleBehaviour() {
        visionLocked = false;
        StopCoroutine("VisionDiscoveryRoutine");
        StartCoroutine("VisionDiscoveryRoutine");
    }

    public void VisionEnterTalkBehaviour() {
        visionLocked = true;
        visionDiscoveryTransform = _playerHead;
        StartCoroutine("VisionFocusRoutine");
    }

    public void VisionEnterMoveBehaviour() {
        visionLocked = true;
        visionDiscoveryTransform = _focusDefaultPoint;        
    }

    public void VisionUpdateMoveBehaviour() {
        _focusPivot.LookAt(visionDiscoveryTransform.position);
        _asset.LookAt(visionDiscoveryTransform.position);
    }

    public void VisionExitBehaviour() {
        visionLocked = false;
        StopCoroutine("VisionDiscoveryRoutine");
        StopCoroutine("VisionFocusRoutine");
    }

    public void TravelEnterBehaviour() {
        travelLocked = true;
        UpdateRNGToken();
        StartCoroutine("TravelPredictRoutine");
        StartCoroutine("TravelMoveRoutine");
    }

    public void TravelSetUpTalkBehaviour(Vector3 position) {
        travelPosition = position;
        IsTalking = true;
    }

    public void TravelEnterTalkBehaviour() {
        travelLocked = true;        
        StartCoroutine("TravelMoveRoutine");
    }

    public void TravelExitBehaviour() {
        travelLocked = false;
        currentVelocity = 0f;
        _trail.SetFloat("DragDirection", 0f);

        StopCoroutine("TravelPredictRoutine");
        StopCoroutine("TravelMoveRoutine");
    }

    public void UnstuckEnterBehaviour() {        
        travelLocked = true;
        UpdateRNGToken();
        StartCoroutine("UnstuckPredictRoutine");
        StartCoroutine("UnstuckMoveRoutine");
    }

    public void UnstuckExitBehaviour() {
        travelLocked = false;
        currentVelocity = 0f;
        _trail.SetFloat("DragDirection", 0f);

        StopCoroutine("UnstuckPredictRoutine");
        StopCoroutine("UnstuckMoveRoutine");
    }

    public void CheckStuckBehaviour() {
        //StopCoroutine("CheckStuckRoutine");
        StartCoroutine("CheckStuckRoutine");
    }

    public void CheckOperativeBehaviour() {
        //StopCoroutine("CheckOperativeRoutine");
        StartCoroutine("CheckOperativeRoutine");
    }

    private IEnumerator CycleRNGToken() {
        int tempToken = Random.Range(1, 9);
        
        if (rngToken != tempToken) {
            rngToken = tempToken;
            
            yield break;
        }        

        while (rngToken == tempToken) {
            tempToken = Random.Range(1, 9);

            if (rngToken != tempToken) {
                rngToken = tempToken;

                yield break;
            }
        }        
    }

    private IEnumerator TravelPredictRoutine() {
        yield return null;

        while (travelLocked) {
            travelPosition = ComputeTravelPosition();
            yield return null;
        }
        yield break;
    }

    private IEnumerator TravelMoveRoutine() { 
        Vector3 lerpPosition;
        
        yield return null;

        Quaternion travelRotation = transform.rotation;
        
        float travelDistance = Mathf.Abs(Vector3.Distance(transform.position, travelPosition));
        Vector3 travelForward = (travelPosition - transform.position).normalized;
        travelRotation.SetLookRotation(travelForward);
        _focusDefaultPivot.rotation = Quaternion.Euler(0f, travelRotation.eulerAngles.y, 0f);

        while (travelDistance > 0.3f) {
            travelDistance = Mathf.Abs(Vector3.Distance(transform.position, travelPosition));
            travelForward = (travelPosition - transform.position).normalized;
            travelRotation.SetLookRotation(travelForward);
            currentVelocity = Mathf.LerpUnclamped(currentVelocity, maxVelocity, .1f);
            lerpPosition = Vector3.LerpUnclamped(transform.position, travelPosition, currentVelocity * Time.deltaTime);

            _trail.SetFloat("DragDirection", (currentVelocity / maxVelocity));
            _focusDefaultPivot.rotation = Quaternion.Euler(0f, travelRotation.eulerAngles.y, 0f);
            transform.position = lerpPosition;

            yield return null;
        }

        travelLocked = false;
        isMoving = false;
        currentVelocity = 0f;
        _trail.SetFloat("DragDirection", 0f);

        yield break;
    }

    private IEnumerator VisionDiscoveryRoutine() {
        Collider[] targets = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Enemy"));

        yield return null;

        if (playerDistance < limitDistance) {            
            visionDiscoveryTransform = _playerHead;
                        
            visionLocked = true;
            StartCoroutine("VisionFocusRoutine");

            yield break;            

        } else {

            if (targets != null) {
                float distanceBuffer = 10f;

                visionDiscoveryTransform = _focusDefaultPoint;

                foreach (Collider target in targets) {
                    try {
                        Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out RaycastHit hitInfo, 10f);                 

                        if (hitInfo.collider.transform == target.transform) {                         
                            float targetDistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));

                            if (targetDistance <= distanceBuffer) {
                                distanceBuffer = targetDistance;
                                visionDiscoveryTransform = target.transform;                          
                            }
                        
                        }                    

                    } catch {
                        visionDiscoveryTransform = _focusDefaultPoint;
                        break;

                    }
                        yield return null;
                }
                                                
                visionLocked = true;
                StartCoroutine("VisionFocusRoutine");

                yield break;                

            } else {
                visionDiscoveryTransform = _focusDefaultPoint;                
            }
        }
        
        visionLocked = true;
        StartCoroutine("VisionFocusRoutine");

        yield break;
    }

    private IEnumerator VisionFocusRoutine() {
        Vector3 lerpPoint = _focusPoint.position;        
        
        _focusPivot.LookAt(visionDiscoveryTransform.position);
        visionFocusPoint = _focusPoint.position;

        while (Mathf.Abs(Vector3.Distance(lerpPoint, visionFocusPoint)) > 0.2f) {            
            _focusPivot.LookAt(visionDiscoveryTransform.position);
            visionFocusPoint = _focusPoint.position;
            _asset.LookAt(lerpPoint);            
            lerpPoint = Vector3.LerpUnclamped(lerpPoint, visionFocusPoint, 10f * Time.deltaTime);
            yield return null;
        }

        _focusPivot.LookAt(visionDiscoveryTransform.position);
        _asset.LookAt(visionDiscoveryTransform.position);

        while (visionLocked) {           
            _focusPivot.LookAt(visionDiscoveryTransform.position);
            _asset.LookAt(visionDiscoveryTransform.position);
            yield return null;
        }

        yield break;
    }

    private IEnumerator UnstuckPredictRoutine() {
        Collider[] collisions = new Collider[8];

        yield return null; 

        if (exclusionList[rngToken - 1] == false) {

            travelPosition = ComputeTravelPosition();
            yield return null;

        } else {

            UpdateRNGToken();
            yield return null;

        }

        while (isUnstucking) {            
            if (exclusionList[rngToken - 1] == false) {

                collisions = Physics.OverlapSphere(ComputeTravelPosition(), .1f);
 
                if (collisions.Length != 0) {

                    exclusionList[rngToken - 1] = true;

                    UpdateRNGToken();

                } else {

                    travelPosition = ComputeTravelPosition();

                }

            } else {
                
                UpdateRNGToken();

            }            

            yield return null;

        }

        exclusionList = new bool[8];

        yield break;
    }

    private IEnumerator UnstuckMoveRoutine() {
        Vector3 lerpPosition;

        yield return null;

        Quaternion travelRotation = transform.rotation;

        float travelDistance = Mathf.Abs(Vector3.Distance(transform.position, travelPosition));
        Vector3 travelForward = (travelPosition - transform.position).normalized;
        travelRotation.SetLookRotation(travelForward);
        _focusDefaultPivot.rotation = Quaternion.Euler(0f, travelRotation.eulerAngles.y, 0f);

        while (travelDistance > .3f) {
            travelDistance = Mathf.Abs(Vector3.Distance(transform.position, travelPosition));
            travelForward = (travelPosition - transform.position).normalized;
            travelRotation.SetLookRotation(travelForward);
            currentVelocity = Mathf.LerpUnclamped(currentVelocity, maxVelocity, .1f);
            lerpPosition = Vector3.LerpUnclamped(transform.position, travelPosition, currentVelocity * Time.deltaTime);

            _trail.SetFloat("DragDirection", (currentVelocity / maxVelocity));
            _focusDefaultPivot.rotation = Quaternion.Euler(0f, travelRotation.eulerAngles.y, 0f);
            transform.position = lerpPosition;

            yield return null;
        }

        travelLocked = false;
        isUnstucking = false;
        isOperative = true;
        currentVelocity = 0f;
        _trail.SetFloat("DragDirection", 0f);

        yield break;

    }

    private IEnumerator CheckStuckRoutine() {
        yield return null;

        if (Physics.Raycast(transform.position, (_playerHead.position - transform.position).normalized, out RaycastHit hitInfo, playerDistance + horizontalOffset)) {
            yield return null;

            if (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "PlayerAttacks" && hitInfo.collider.tag != "Interact") {
                isStuck = true;
            }
            
            yield break;
            
        }

        yield break;
    }

    private IEnumerator CheckOperativeRoutine() {
        Collider[] colliderList;
        yield return null;

        colliderList = Physics.OverlapSphere(transform.position, .5f, LayerMask.NameToLayer("Companion"), QueryTriggerInteraction.Ignore);
        
        if (colliderList.Length == 0) {
            isOperative = true;
            yield break;
        }        

        yield break;
    }

    //
}
