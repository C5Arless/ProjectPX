using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LBController : MonoBehaviour, ISpawnable {
    [SerializeField] TMP_Text _stateText;
    [SerializeField] List<Material> materials;

    //State reference
    LBStateHandler _stateHandler;
    LBBaseState _currentRootState;
    LBBaseState _currentSubState;

    //State vars
    private bool isReady;

    private bool isRunning;
    private bool isPaused;
    private bool isSpawning;

    private bool isBug;
    private bool isBall;
    private bool isDead;
    private bool isIdle;
    private bool isPatrolling;
    private bool isPursuing;
    private bool isAttacking;
    private Vector3 initialScale;

    public Vector3 Position { get { return transform.position; } }

    public bool IsReady { get { return isReady; } set { isReady = value; } }
    public bool IsSpawning { get { return isSpawning; } set { isSpawning = value; } }
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }

    public bool IsBug { get { return isBug; } set { isBug = value; } }
    public bool IsBall { get { return isBall; } set { isBall = value; } }
    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public bool IsIdle { get { return isIdle; } set { isIdle = value; } }
    public bool IsPatrolling { get { return isPatrolling; } set { isPatrolling = value; } }
    public bool IsPursuing { get { return isPursuing; } set { isPursuing = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }

    private void Awake() {
        _stateHandler = new LBStateHandler(this);

        initialScale = transform.localScale;
    }

    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }

        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler.Register(this);
        }

        transform.localScale = Vector3.zero;
    }

    private void OnDisable() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused -= PauseBehaviour;
            GameMaster.Instance._onGameUnpaused -= UnpauseBehaviour;
        }

        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler.Unregister(this);
        }
    }

    private void Update() {
        if (isRunning) {
            _stateText.text = _currentRootState.ToString() + " " + _currentSubState.ToString(); //DEBUG
        }
    }

    public void PauseBehaviour() {
        isPaused = true;
    }

    public void UnpauseBehaviour() {
        isPaused = false;
    }

    void FixedUpdate() {
        if (isRunning && !isPaused) {
            _currentRootState.UpdateState();
            _currentSubState.UpdateState();
        }
    }

    public void Spawn() {        
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void IdleStart() {
        StartCoroutine(IdleRoutine());
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

    private void InitializeStateMachine() {
        isBug = true;
        _currentRootState = _stateHandler.Bug();
        _currentRootState.EnterState();

        isIdle = true;
        _currentSubState = _stateHandler.Idle();
        _currentSubState.EnterState();

        isSpawning = false;
        isRunning = true;
    }
    
    private IEnumerator IdleRoutine() {
        yield return new WaitForSeconds(2f);
        
        isIdle = false;
        isPatrolling = true;
        
        yield return null;
    }
}