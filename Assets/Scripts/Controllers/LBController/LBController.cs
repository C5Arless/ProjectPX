using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class LBController : MonoBehaviour, ISpawnable {
    [SerializeField] GameObject ballShell;
    [SerializeField] List<Renderer> renderers;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] int maxHealth;
    [SerializeField] private float visionRange = 10f;
    
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float pursueSpeed = 3f;
    [SerializeField] float idleTime = 3f;
    
    [SerializeField] GameObject player; //TESTING
    
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
    private bool isDamaged;
    private bool isMorphing;
    
    private Dictionary<LBRootStates, bool> rootStates; 
    private Dictionary<LBSubStates, bool> subStates;
    
    private LBAnimHandler animHandler;
    private PatrolZone patrolZone;

    private Vector3 initialScale;
    private Vector3 attackPoint = Vector3.zero;
    
    #region Getters and Setters

    public GameObject BallShell { get { return ballShell; } set { ballShell = value; } }
    public NavMeshAgent Agent { get { return navMeshAgent; } set {  navMeshAgent = value; } }
    public Rigidbody RigidBody { get { return rigidBody; } set {  rigidBody = value; } }
    public int MaxHealth { get { return maxHealth; } }
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
    public bool IsDamaged { get { return isDamaged; } set { isDamaged = value; } }
    public bool IsMorphing { get { return isMorphing; } set { isMorphing = value; }}

    public Dictionary<LBRootStates, bool>  RootStates { get { return rootStates; } }
    public Dictionary<LBSubStates, bool>  SubStates { get { return subStates; } }
    
    public LBAnimHandler AnimHandler { get { return animHandler; } set { animHandler = value; } }
    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    
    #endregion
    
    private void Awake() {
        _stateHandler = new LBStateHandler(this);
        animHandler = GetComponentInChildren<LBAnimHandler>();
        
        initialScale = transform.localScale;
    }
    
    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }

        InitializeFSM();
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

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("PlayerAttacks") && !isDamaged) {
            SetSubState(LBSubStates.Damaged);
        }
        else if (!rigidBody.isKinematic && other.collider.CompareTag("Ground") && subStates[LBSubStates.Attack] && rootStates[LBRootStates.Bug]) {
            SetSubState(LBSubStates.Idle);
        }

    }
    
    void FixedUpdate() {
        if (isRunning && !isPaused) {
            _currentRootState.UpdateState();

            if (!rootStates[LBRootStates.Dead]) {
                _currentSubState.UpdateState();
            }
        }
    }

    public void HandleSignal(int signal) {
        switch (signal) {
            case 0: {
                StartCoroutine(BugAttackRoutine());
                break;
            }
            case 1: {
                isMorphing = true;
                break;
            }
            case 2: {
                isMorphing = false;
                break;
            }
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

    public void InitializeFSM() {
        isReady = false;
        isRunning = false;
        isPaused = false;
        isSpawning = false;
        isDamaged = false;
        
        InitializeStateKeys();
        
        EvaluateSpawn();
    }
    
    private bool CanSeePlayer() {
        Vector3 dir = (player.transform.position - transform.position);
        if (dir.magnitude > 10f) { return false; }

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > 60f * 0.5f) { return false; }

        if (Physics.Raycast(transform.position + Vector3.up * .5f, dir.normalized, out RaycastHit hit, visionRange)) {
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("PlayerAttacks")) {
                return true;
            }
        }

        return false;
    }
    
    public void SetMask(float maskValue) {
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

    public void SetScale(float scaleValue) {
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
    
    private void InitializeStateFlow() {
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

    private IEnumerator BugAttackRoutine() {
        navMeshAgent.enabled = false;
        yield return null;
        
        rigidBody.isKinematic = false;
        rigidBody.ResetInertiaTensor();
        rigidBody.velocity = Vector3.zero;
        yield return null;
        
        RigidBody.AddForce(RigidBody.transform.forward * 5f, ForceMode.Impulse);
        RigidBody.AddForce(RigidBody.transform.up * 4f, ForceMode.Impulse);
        yield break;
    }
    
    private IEnumerator InitializeSpawnZone() {
        yield return null;
        
        patrolZone = GameBucket.Instance.SpawnHandler.GetPatrolZone(transform);
        yield return null;
        
        if (patrolZone != null) {
            Vector3 waypointPosition = patrolZone.RetrieveWaypoint();

            if (NavMesh.SamplePosition(waypointPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas)) {
                Vector3 spawnPosition = hit.position;
                transform.position = spawnPosition;
                
                currentHp = maxHealth;
                rigidBody.isKinematic = true;
                yield return null;
                
                navMeshAgent.enabled = true;
                navMeshAgent.Warp(spawnPosition);
                navMeshAgent.speed = 0f;
                yield return null;
                
                transform.localScale = Vector3.zero;
                GameBucket.Instance.SpawnHandler.RegisterSpawn(this);
            }
        }
        
        yield break;
    }
    
    private IEnumerator SpawnRoutine() {
        float targetScale = 1f;
        float currentScale = 0f;        
        
        SetScale(currentScale);
        SetMask(currentScale);

        while (currentScale < targetScale) {
            currentScale += .01f;
            
            SetScale(currentScale);
            SetMask(currentScale);

            yield return null;
        }

        currentScale = 1f;

        SetMask(currentScale);
        SetScale(currentScale);

        yield return null;

        InitializeStateFlow();

        yield break;
    }

    private void OnDrawGizmos() {
        if (!isRunning) { return; }
        
        if (!subStates[LBSubStates.Pursue]) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + transform.up * .5f, (transform.position + transform.up * .5f) + (transform.forward * 10f));
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((transform.position + transform.up * .5f) + (transform.forward * 10f), 0.2f);
        }
        else {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + transform.up * .5f, player.transform.position);
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(player.transform.position, 0.2f);
        }
    }
}