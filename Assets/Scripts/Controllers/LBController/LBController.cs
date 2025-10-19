using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class LBController : MonoBehaviour, ISpawnable {
    [SerializeField] TMP_Text _stateText;
    [SerializeField] List<Renderer> renderers;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] int maxHealth;
    
    [SerializeField] GameObject player; //TESTING

    private const float patrolSpeed = 2f;
    private const float pursueSpeed = 3f;
    private const float idleTime = 3f;
    
    private int currentHp;
    
    //State reference
    LBStateHandler _stateHandler;
    LBBaseState _currentRootState;
    LBBaseState _currentSubState;

    //State vars
    private bool isReady;

    private bool isRunning;
    private bool isPaused;
    private bool isSpawning;

    private Dictionary<LBRootStates, bool> rootStates; 
    private Dictionary<LBSubStates, bool> subStates;
    
    private LBAnimHandler animHandler;
    private PatrolZone patrolZone;

    private Vector3 initialScale;
    private Vector3 attackPoint = Vector3.zero;
    
    #region Getters and Setters
    
    public NavMeshAgent Agent { get { return navMeshAgent; } set {  navMeshAgent = value; } }
    public int CurrentHealth { get { return currentHp; } set { currentHp = value; } }
    public Vector3 Position { get { return transform.position; } }
    public Vector3 AttackPoint { get { return attackPoint; } set { attackPoint = value; } }
    public GameObject Player { get { return player; } } //TESTING
    
    public float IdleTime { get { return idleTime; } }
    public float PatrolSpeed { get { return patrolSpeed; } }
    public float PursueSpeed { get { return pursueSpeed; } }
    public PatrolZone PatrolZone { get { return patrolZone; } }

    public bool IsReady { get { return isReady; } set { isReady = value; } }
    public bool IsSpawning { get { return isSpawning; } set { isSpawning = value; } }
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }
    
    public Dictionary<LBRootStates, bool>  RootStates { get { return rootStates; } }
    public Dictionary<LBSubStates, bool>  SubStates { get { return subStates; } }
    
    public LBAnimHandler AnimHandler { get { return animHandler; } set { animHandler = value; } }
    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    
    #endregion
    
    private void Awake() {
        currentHp = maxHealth;
        
        InitializeStateKeys();
        
        _stateHandler = new LBStateHandler(this);
        animHandler = GetComponentInChildren<LBAnimHandler>();

        initialScale = transform.localScale;
    }
    
    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }

        EvaluateSpawn();

        transform.localScale = Vector3.zero;
    }

    private void OnDisable() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused -= PauseBehaviour;
            GameMaster.Instance._onGameUnpaused -= UnpauseBehaviour;
        }

        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler.UnregisterSpawn(this);
        }
    }

    private void Update() {
        if (isRunning) {
            _stateText.text = _currentRootState + " " + _currentSubState; //DEBUG
        }
    }

    void FixedUpdate() {
        if (isRunning && !isPaused) {
            _currentRootState.UpdateState();
            _currentSubState.UpdateState();
        }
    }

    public void HandleSignal(int signal) {
        //
    }

    public void PauseBehaviour() {
        isPaused = true;
    }

    public void UnpauseBehaviour() {
        isPaused = false;
    }
    
    public void Spawn() {
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void Scout() {
        if (CanSeePlayer() && attackPoint == Vector3.zero) {
            SetSubState(LBSubStates.Pursue);
        }
    }

    public void SetSubState(LBSubStates state) {
        Dictionary<LBSubStates, bool> target = new Dictionary<LBSubStates, bool>(5);
        target = InitializeSubStateKeys();
        
        foreach (KeyValuePair<LBSubStates, bool> subState in SubStates) {
            if (subState.Value) {
                target[subState.Key] = false;
            }
        }
        
        target[state] = true;
        
        subStates = target;
    }

    public void SetRootState(LBRootStates state) {
        if (state == LBRootStates.Ball && maxHealth < 2) { return; }
        
        Dictionary<LBRootStates, bool> target = new Dictionary<LBRootStates, bool>(3);
        target = InitializeRootStateKeys();
        
        foreach (KeyValuePair<LBRootStates, bool> rootState in rootStates) {
            if (rootState.Value) {
                target[rootState.Key] = false;
            }
        }
        
        target[state] = true;
        
        rootStates = target;
    }
    
    private bool CanSeePlayer() {
        Vector3 dir = (player.transform.position - transform.position);
        if (dir.magnitude > 10f) { return false; }

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > 60f * 0.5f) { return false; }

        if (Physics.Raycast(transform.position + Vector3.up * .5f, dir.normalized, out RaycastHit hit, 10f)) {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
    
    private void SetMask(float maskValue) {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        float targetValue = maskValue;
        propertyBlock.SetFloat("_Mask", targetValue);
        
        foreach (var renderer in renderers) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                if (renderer.materials[i].HasProperty("_Mask")) {
                    renderer.SetPropertyBlock(propertyBlock, i);
                }
            }
            
        }
    }

    private void SetScale(float scaleValue) {
        Vector3 scale = initialScale * scaleValue;
        transform.localScale = scale;
    }
    
    private void InitializeStateKeys() {
        rootStates = new Dictionary<LBRootStates, bool>(3);
        subStates = new Dictionary<LBSubStates, bool>(5);

        rootStates = InitializeRootStateKeys();
        subStates = InitializeSubStateKeys();
    }
    
    private Dictionary<LBRootStates, bool> InitializeRootStateKeys() {
        Dictionary<LBRootStates, bool> target = new Dictionary<LBRootStates, bool>(3);
        
        target.Add(LBRootStates.Bug, false);
        target.Add(LBRootStates.Ball, false);
        target.Add(LBRootStates.Dead, false);
        
        return target;
    }
    
    private Dictionary<LBSubStates, bool> InitializeSubStateKeys() {
        Dictionary<LBSubStates, bool> target = new Dictionary<LBSubStates, bool>(5);
        
        target.Add(LBSubStates.Idle, false);
        target.Add(LBSubStates.Patrol, false);
        target.Add(LBSubStates.Pursue, false);
        target.Add(LBSubStates.Damaged, false);
        target.Add(LBSubStates.Attack, false);
        
        return target;
    }
    
    private void InitializeStateMachine() {
        SetRootState(LBRootStates.Bug);
        _currentRootState = _stateHandler.Bug();
        _currentRootState.EnterState();

        SetSubState(LBSubStates.Idle);
        _currentSubState = _stateHandler.Idle();
        _currentSubState.EnterState();

        isSpawning = false;
        isRunning = true;
    }

    private void EvaluateSpawn() {
        StartCoroutine(InitializeSpawnZone());
    }

    private IEnumerator InitializeSpawnZone() {
        yield return null;
        
        patrolZone = GameBucket.Instance.SpawnHandler.GetPatrolZone(transform);
        yield return null;
        
        if (patrolZone != null) {
            Vector3 spawnPosition = patrolZone.RetrieveWaypoint();
            transform.position = new Vector3(spawnPosition.x, transform.position.y, spawnPosition.z);
            
            GameBucket.Instance.SpawnHandler.RegisterSpawn(this);
        }
        
        yield break;
    }
    
    private IEnumerator SpawnRoutine() {
        float targetScale = 1f;
        float currentScale = 0f;        
        
        SetScale(currentScale);
        SetMask(currentScale);

        while (currentScale < targetScale) {
            currentScale += .05f;
            
            SetScale(currentScale);
            SetMask(currentScale);

            yield return null;
        }

        currentScale = 1f;

        SetMask(currentScale);
        SetScale(currentScale);

        yield return null;

        InitializeStateMachine();

        yield break;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + transform.up * .5f, (transform.position + transform.up * .5f) + (transform.forward * 10f));
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere((transform.position + transform.up * .5f) + (transform.forward * 10f), 0.2f);
        
    }
}