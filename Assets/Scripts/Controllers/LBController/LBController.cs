using System.Collections;
using TMPro;
using UnityEngine;

public class LBController : MonoBehaviour, ISpawnable {
    [SerializeField] TMP_Text _stateText;

    //State reference
    LBStateHandler _stateHandler;
    LBBaseState _currentRootState;
    LBBaseState _currentSubState;

    //State vars
    private bool isReady = false;

    private bool isRunning = false;
    private bool isPaused = false;
    private bool isSpawning = false;

    private bool isBug = false;
    private bool isBall = false;
    private bool isDead = false;
    private bool isIdle = false;
    private bool isPatrolling = false;
    private bool isPursuing = false;
    private bool isAttacking = false;

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
        
    }

    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }

        if (GameBucket.Instance != null) {
            GameBucket.Instance.SpawnHandler.Register(this);
        }
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
        Debug.Log("Spawning!");
        isSpawning = true;

        //Set initial size
        //Set initial material mask

        //Routine 0-1

        /*
        _currentRootState = _stateHandler.Bug();
        _currentRootState.EnterState();

        _currentSubState = _stateHandler.Idle();
        _currentSubState.EnterState();

        isRunning = true;
        */
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine() {
        float initialXSize = transform.localScale.x;
        float initialYSize = transform.localScale.y;
        float initialZSize = transform.localScale.z;

        float targetScale = 1f;

        transform.localScale.Set(0f, 0f, 0f);
        float currentScale = 0f;

        while (currentScale <= targetScale) {
            currentScale += .02f;
            Vector3 scale = new Vector3(initialXSize * currentScale, initialYSize * currentScale, initialZSize * currentScale);
            transform.localScale += scale;
            Debug.Log(currentScale);
            yield return null;
        }

        transform.localScale = Vector3.one;

        isBug = true;
        _currentRootState = _stateHandler.Bug();
        _currentRootState.EnterState();

        isIdle = true;
        _currentSubState = _stateHandler.Idle();
        _currentSubState.EnterState();

        isRunning = true;
        isSpawning = false;

        yield break;
    }
}