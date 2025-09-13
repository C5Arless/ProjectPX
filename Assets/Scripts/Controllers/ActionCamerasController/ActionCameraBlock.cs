using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionCameraBlock : MonoBehaviour, IOrchestratedEvent, IParentTrigger {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;
    [SerializeField] bool lockPlayer;
    [Space]
    [SerializeField] CinemachineTargetGroup _targetGroup;
    [Space]
    [SerializeField] int eventIndex;
    [SerializeField] bool isRepeatable;
    [SerializeField] EventPriority priority;
    
    private CancellationTokenSource token;
    private bool isLocked;
    private bool onAction;

    public GameObject PlayerPos { get { return _playerPos; } }
    public CinemachineTargetGroup TargetGroup { get { return _targetGroup; } }

    public int EventIndex { get { return eventIndex; } }

    public bool IsRepeatable { get { return isRepeatable; } }

    public EventPriority Priority { get { return priority; } }

    public void TriggerEnter(Collider other) {
        if (GameBucket.Instance.PXController == null) { return; }
        if (GameBucket.Instance.PXController.OnCinematic) { return; }
        if (GameBucket.Instance.PXController.OnAction) { return; }

        if (other.tag == "Player") {

            GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);            
        }
    }
        
    public void TriggerStay(Collider other) {
        if (GameBucket.Instance.PXController == null) { return; }
        if (GameBucket.Instance.PXController.OnCinematic) { return; }
        if (GameBucket.Instance.PXController.OnAction) { return; }
        
        if (other.tag == "Player") {
            EnqueueContinuous();
        }
    }    

    public void TriggerExit(Collider other) {
        if (!onAction) { return; }

        if (GameBucket.Instance.PXController.OnCinematic) { return; }

        if (other.tag == "Player") {
            ActionExit();
        }
    }

    public async Task FireEvent() {        
        token = new CancellationTokenSource();
        
        onAction = true;

        ActionEnter();
        GameBucket.Instance.PXController.ActionCameraEnter();
        CameraManager.Instance.SwitchGameVCamera(_actionVCamera);

        while (onAction) {

            if (token.IsCancellationRequested) {
                ActionExit();
                await Task.Yield();
                break;
            }

            await Task.Yield();
        }

        await Task.Yield();
    }

    public void CancelEvent() {
        token.Cancel();
    }

    public void DestroyEvent() {
        Destroy(gameObject);
    }

    private void ActionEnter() {
        //GameBucket.Instance.PXController.ActionCameraEnter();
        //CameraManager.Instance.SwitchGameVCamera(_actionVCamera);

        if (lockPlayer) {
            isLocked = true;            
            StartCoroutine("LockPlayerPosition");
        }
        else {
            StartCoroutine("OnActionRoutine");
        }
    }

    private void ActionExit() {
        onAction = false;
        isLocked = false;
        GameBucket.Instance.PXController.ActionCameraExit();
    }

    private void EnqueueContinuous() {
        if (!GameBucket.Instance.EventsOrchestrator.EventsQueue.Contains(this)) {
            GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);
        }
    }

    private IEnumerator LockPlayerPosition() {
        yield return null;

        while (isLocked) {
            if (GameBucket.Instance.PXController.OnCinematic) { continue; }

            _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
            Vector3 actionCameraPosition = CameraManager.Instance.GetCurrentViewCamera().transform.position;
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform.position, actionCameraPosition);

            if (CameraManager.Instance.CurrentGameCamera != _actionVCamera) {
                CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator OnActionRoutine() {
        yield return null;

        while (onAction) {
            Vector3 actionCameraPosition = CameraManager.Instance.GetCurrentViewCamera().transform.position;
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform.position, actionCameraPosition);
            
            if (CameraManager.Instance.CurrentGameCamera != _actionVCamera) {
                CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
            }

            yield return null;
        }

        yield break;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (_actionVCamera != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_actionVCamera.transform.position, 0.2f);
            UnityEditor.Handles.Label(_actionVCamera.transform.position, "ActionVCamera");
        }

        if (_playerPos != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerPos.transform.position, 0.2f);
            UnityEditor.Handles.Label(_playerPos.transform.position, "ActionPlayerPos");
        }
    }

#endif
}
