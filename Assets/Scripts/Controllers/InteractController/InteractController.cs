using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractController : MonoBehaviour, IOrchestratedEvent {    
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _popUp;

    [SerializeField] int pages;

    [SerializeField] InteractionVCameras[] interactionCams;
    [SerializeField] DialogPages[] _DialogPages;

    [SerializeField] int _eventIndex;
    [SerializeField] bool _isRepeatable;
    [SerializeField] EventPriority _eventPriority;

    [SerializeField] bool hasTrigger;

    InputAction _interactAction;
    InputAction _confirmAction;

    private int pageNumber;
    private bool isInteracting;
    private bool isBusy;
    private bool isRunning;
    private bool canConfirm;

    public bool IsInteracting { get { return isInteracting; } }
    public int EventIndex { get { return _eventIndex; } }
    public bool IsRepeatable { get { return _isRepeatable; } }
    public EventPriority Priority { get { return _eventPriority; } }

    private void Start() {
        InitializeActions();
    }

    private void OnDisable() {
        UnsubscribeActions();
    }

    private void OnDestroy() {
        UnsubscribeActions();
    }

    private void OnTriggerEnter(Collider other) {        
        if (other.tag == "Player") {
            SubscribeActions();

            if (hasTrigger) {
                GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);
            } else {
                SwitchPopUp(true);
                GameBucket.Instance.PXController.CanInteract = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (hasTrigger) { return; }

        if (other.tag == "Player") {
            UnsubscribeActions();
            SwitchPopUp(false);
            GameBucket.Instance.PXController.CanInteract = false;
        }
    }

    public async Task FireEvent() {
        if (isRunning) { return; }
        
        GameMaster.Instance.CutscenePause();
        GameBucket.Instance.GameCanvasHandler.HideUI();        

        GameBucket.Instance.PXController.InteractionEnter(_playerTarget.transform, _focusTarget.transform, interactionCams[0].vcamera);

        GameBucket.Instance.CompanionCtx.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
        GameBucket.Instance.CompanionCtx.VisionSetUpTalkBehaviour(_focusTarget);        
        Interact();

        while (isRunning) {
            await Task.Yield();
        }

        GameBucket.Instance.PXController.InteractionExit();
        GameBucket.Instance.CompanionCtx.ExitTalkState();

        StartCoroutine(EvaluateUI());

        await Task.Yield();
    }

    public void CancelEvent() {
        throw new System.NotImplementedException();
    }

    public void DestroyEvent() {
        //if (GameBucket.Instance.PXController.OnInteract) GameBucket.Instance.PXController.OnInteract = false;
        Destroy(gameObject);        
    }

    public void OnInteract(InputAction.CallbackContext input) {
        if (!GameBucket.Instance.PXController.CanInteract) { return; }

        if (input.ReadValue<float>() != 0f) {
            if (!hasTrigger) {
                GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);
            }

        }

    }

    public void OnConfirm(InputAction.CallbackContext input) {
        if (!canConfirm) { return; }

        if (input.ReadValue<float>() != 0f) {
            Interact();
        }
    }

    private void InitializeActions() {
        _interactAction = InputManager.Instance.GetPlayerInput().actions["Interact"];
        _confirmAction = InputManager.Instance.GetPlayerInput().actions["Confirm"];
    }

    private void SubscribeActions() {
        _confirmAction.started += OnConfirm;
        _interactAction.started += OnInteract;
    }

    private void UnsubscribeActions() {
        _confirmAction.started -= OnConfirm;
        _interactAction.started -= OnInteract;
    }

    public void Interact() {
        if (isBusy) { return; }

        isRunning = true;

        if (isInteracting) {
            Continue();
        } else {
            Enter();  
        }
    }   

    private void SwitchPopUp(bool state) {
        if (!hasTrigger) {
            _popUp.SetActive(state);
        }
    }

    private void Enter() {
        pageNumber = 1;
        
        SwitchPopUp(false);
        isInteracting = true;

        StartCoroutine(EnterRoutine());
    }

    private void Continue() {
        if (pageNumber >= pages) {
            Exit();
        } else {
            SwitchPage();
        }
    }

    private void SwitchPage() {
        if (GameBucket.Instance.GameCanvasHandler.IsTyping) {
            GameBucket.Instance.GameCanvasHandler.DialogSkip();
        } else {
            StartCoroutine(IteratePage());
        }
    }

    private void Exit() {
        isInteracting = false;

        StartCoroutine(ExitRoutine());
    }

    private IEnumerator EvaluateUI() {
        yield return new WaitWhile(() => GameBucket.Instance.EventsOrchestrator.IsRunning);

        GameBucket.Instance.GameCanvasHandler.ShowUI();
        GameMaster.Instance.CutsceneUnpause();
        
        yield break;
    }

    private IEnumerator IteratePage() {
        isBusy = true;
        yield return null;

        pageNumber++;
        yield return null;

        foreach (InteractionVCameras _ivcam in interactionCams) {
            if (_ivcam.pageIdx == pageNumber) {
                CameraManager.Instance.SwitchGameVCamera(_ivcam.vcamera);
            }
        }

        GameBucket.Instance.GameCanvasHandler.DialogClear();
        GameBucket.Instance.GameCanvasHandler.DialogWrite(_DialogPages[pageNumber - 1].dialogIdx);

        yield return null;

        canConfirm = true;
        isBusy = false;
        yield break;
    }

    private IEnumerator EnterRoutine() {
        canConfirm = false; 
        isBusy = true;
        yield return null;

        GameBucket.Instance.GameCanvasHandler.DialogIn();
        yield return new WaitWhile(() => GameBucket.Instance.GameCanvasHandler.IsTransitioning);
        canConfirm = true;

        GameBucket.Instance.GameCanvasHandler.DialogWrite(_DialogPages[pageNumber - 1].dialogIdx);
        isBusy = false;
        yield break;
    }

    private IEnumerator ExitRoutine() {
        canConfirm = false;
        isBusy = true;

        GameBucket.Instance.GameCanvasHandler.DialogOut();

        yield return new WaitWhile(() => GameBucket.Instance.GameCanvasHandler.IsTransitioning);        

        UnsubscribeActions();
        transform.GetComponent<Collider>().enabled = false;
        yield return null;

        isRunning = false;
        yield return new WaitForSeconds(2f);

        if (_isRepeatable) {
            transform.GetComponent<Collider>().enabled = true;
        }

        isBusy = false;
        yield break;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (_focusTarget != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_focusTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_focusTarget.transform.position, "InteractFocusTarget");
        }

        if (_playerTarget != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_playerTarget.transform.position, "InteractPlayerTarget");
        }

        if (_companionTarget != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_companionTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_companionTarget.transform.position, "InteractCompanionTarget");
        }
    }

#endif
}
