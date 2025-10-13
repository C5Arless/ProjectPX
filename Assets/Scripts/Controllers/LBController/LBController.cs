using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class LBController : MonoBehaviour, ISpawnable {
    [SerializeField] TMP_Text _stateText;
    [SerializeField] List<Material> materials;
    [SerializeField] NavMeshAgent navMeshAgent;
    
    [SerializeField] GameObject player; //TESTING

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
    
    private PatrolZone patrolZone;
    
    private Vector3 initialScale;

    public Vector3 Position { get { return transform.position; } }

    public bool IsReady { get { return isReady; } set { isReady = value; } }
    public bool IsSpawning { get { return isSpawning; } set { isSpawning = value; } }
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }
    
    public Dictionary<LBRootStates, bool>  RootStates { get { return rootStates; } }
    public Dictionary<LBSubStates, bool>  SubStates { get { return subStates; } }

    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }

    private void Awake() {
        InitializeStateKeys();
        
        _stateHandler = new LBStateHandler(this);

        initialScale = transform.localScale;
    }
    
    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }
        
        InitializePatrolZone();
        
        if (patrolZone != null) {
            GameBucket.Instance.SpawnHandler.RegisterSpawn(this);
        }

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
            _stateText.text = _currentRootState.ToString() + " " + _currentSubState.ToString(); //DEBUG
        }
    }

    void FixedUpdate() {
        if (isRunning && !isPaused) {
            _currentRootState.UpdateState();
            _currentSubState.UpdateState();
        }
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

    public void IdleStart() { //To be moved to update
        StartCoroutine(IdleRoutine());
    }

    public void SetPatrolPosition() {
        Vector3 patrolPosition = RetrieveWaypoint();
        
        navMeshAgent.SetDestination(patrolPosition);
    }

    public void EnterPatrol() {
        navMeshAgent.isStopped = false;
    }
    
    public void UpdatePatrol() {
        if (navMeshAgent.remainingDistance > navMeshAgent.radius * 2f) { return; }
        
        navMeshAgent.isStopped = true;
        SetSubState(LBSubStates.Idle);
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
    
    private void SetMask(float maskValue) {
        float targetValue = maskValue;
        foreach (var material in materials) {
            material.SetFloat("_Mask", targetValue);
        }
    }

    private void SetScale(float scaleValue) {
        Vector3 scale = initialScale * scaleValue;
        transform.localScale = scale;
    }

    private Vector3 RetrieveWaypoint() {
        int waypointIdx = UnityEngine.Random.Range(0, patrolZone.waypoints.Count - 1);
        Vector3 waypoint = patrolZone.waypoints[waypointIdx].position;
        
        return waypoint;
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

    private void InitializePatrolZone() {
        patrolZone = GameBucket.Instance.SpawnHandler.GetPatrolZone(transform);

        if (patrolZone != null) {
            Debug.Log(patrolZone.areaIndex);
        }
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
    
    private IEnumerator IdleRoutine() {
        yield return new WaitForSeconds(2f);

        if (!subStates[LBSubStates.Pursue]) {
            SetSubState(LBSubStates.Patrol);
        }

        yield return null;
    }
}