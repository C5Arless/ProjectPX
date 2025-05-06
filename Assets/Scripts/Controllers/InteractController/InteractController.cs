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

    [SerializeField] bool hasTrigger;

    InputAction _interactAction;
    InputAction _confirmAction;

    private int pageNumber;

    private bool isInteracting;
    private bool isBusy;

    public int EventIndex => throw new System.NotImplementedException();

    public bool IsRepeatable => throw new System.NotImplementedException();

    public EventPriority Priority => throw new System.NotImplementedException();

    private void Start() {
        InitializeActions();
        //SubscribeActions();
    }

    private void OnEnable() {
        //SubscribeActions();
    }

    private void OnDisable() {
        UnsubscribeActions();
    }

    private void OnDestroy() {
        UnsubscribeActions();
    }

    private void OnTriggerEnter(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }

        if (other.tag == "Player") {
            SubscribeActions();
            EvaluateInteraction();          
        }
    }

    private void OnTriggerStay(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }

        if (other.tag == "Player") {
            //EvaluateInteraction();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (hasTrigger) { return; }

        if (other.tag == "Player") {
            UnsubscribeActions();
            _popUp.SetActive(false);            
        }
    }

    public void OnInteract(InputAction.CallbackContext input) {
        if (!GameBucket.Instance.PXController.OnInteract) { return; }

        if (input.ReadValue<float>() != 0f) {
            Interact();
        }

    }

    public void OnConfirm(InputAction.CallbackContext input) {
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

    public Task FireEvent(CancellationToken token) {
        throw new System.NotImplementedException();
    }

    public void Interact() {
        if (isBusy) { return; }

        if (isInteracting) {
            Continue();
        } else {
            Enter();  
        }
    }   

    private void EvaluateInteraction() {
        if (isInteracting) { return; }

        if (hasTrigger) {
            Interact();
        } else {
            _popUp.SetActive(true);
        }
    }

    private void Enter() {
        pageNumber = 1;
        
        _popUp.SetActive(false);
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

        isBusy = false;
        yield break;
    }

    private IEnumerator EnterRoutine() {
        isBusy = true;
        yield return null;

        GameBucket.Instance.PXController.DialogEnter(_playerTarget.transform, _focusTarget.transform, interactionCams[0].vcamera);

        GameBucket.Instance.CompanionCtx.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
        GameBucket.Instance.CompanionCtx.VisionSetUpTalkBehaviour(_focusTarget);
        yield return null;

        GameBucket.Instance.GameCanvasHandler.DialogIn();
        yield return new WaitWhile(() => GameBucket.Instance.GameCanvasHandler.IsTransitioning);

        GameBucket.Instance.GameCanvasHandler.DialogWrite(_DialogPages[pageNumber - 1].dialogIdx);
        isBusy = false;
        yield break;
    }

    private IEnumerator ExitRoutine() {
        isBusy = true;
        GameBucket.Instance.PXController.InteractionExit();
        GameBucket.Instance.CompanionCtx.ExitTalkState();

        GameBucket.Instance.GameCanvasHandler.DialogOut();

        yield return new WaitWhile(() => GameBucket.Instance.GameCanvasHandler.IsTransitioning);        

        UnsubscribeActions();
        transform.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(2f);

        transform.GetComponent<Collider>().enabled = true;
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
