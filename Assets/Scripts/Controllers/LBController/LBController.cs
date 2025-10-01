using TMPro;
using UnityEngine;

public class LBController : MonoBehaviour {
    [SerializeField] TMP_Text _stateText;

    //State reference
    LBStateHandler _stateHandler;
    LBBaseState _currentRootState;
    LBBaseState _currentSubState;

    //State vars
    private bool isRunning = false;
    private bool isPaused = false;

    private bool isBug = true;
    private bool isBall = false;
    private bool isIdle = true;
    private bool isPatrolling = false;
    private bool isPursuing = false;
    private bool isAttacking = false;

    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }

    public bool IsBug { get { return isBug; } set { isBug = value; } }
    public bool IsBall { get { return isBall; } set { isBall = value; } }
    public bool IsIdle { get { return isIdle; } set { isIdle = value; } }
    public bool IsPatrolling { get { return isPatrolling; } set { isPatrolling = value; } }
    public bool IsPursuing { get { return isPursuing; } set { isPursuing = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }

    private void Awake() {
        _stateHandler = new LBStateHandler(this);

        _currentRootState = _stateHandler.Bug();
        _currentRootState.EnterState();

        _currentSubState = _stateHandler.Idle();
        _currentSubState.EnterState();
    }

    private void Start() {
        if (GameMaster.Instance != null) {
            GameMaster.Instance._onGamePaused += PauseBehaviour;
            GameMaster.Instance._onGameUnpaused += UnpauseBehaviour;
        }

        //Spawn
    }

    private void OnDisable() {
        GameMaster.Instance._onGamePaused -= PauseBehaviour;
        GameMaster.Instance._onGameUnpaused -= UnpauseBehaviour;
    }

    private void Update() {
        _stateText.text = _currentRootState.ToString() + " " + _currentSubState.ToString();
    }

    public void PauseBehaviour() {
        //
    }

    public void UnpauseBehaviour() {
        //
    }

    void FixedUpdate() {
        if (isRunning && !isPaused) {
            _currentRootState.UpdateState();
            _currentSubState.UpdateState();
        }
    }
}