using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionCameraBlock : MonoBehaviour, IOrchestratedEvent {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;
    //[SerializeField] GameObject _cameraPivot;
    [SerializeField] bool lockPlayer;
    //[SerializeField] bool hasFollow;
    [Space]
    [SerializeField] int eventIndex;
    [SerializeField] bool isRepeatable;
    [SerializeField] EventPriority priority;
    
    private CancellationTokenSource token;
    private bool isLocked;
    private bool onAction;

    public int EventIndex { get { return eventIndex; } }

    public bool IsRepeatable { get { return isRepeatable; } }

    public EventPriority Priority { get { return priority; } }

    private void OnTriggerEnter(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }
        if (GameBucket.Instance.PXController.OnAction) { return; }

        if (other.tag == "Player") {

            GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);

            /*
            onAction = true;

            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
            
            if (lockPlayer) {
                isLocked = true;
                //TestTask();
                StartCoroutine("LockPlayerPosition");
            } else {
                StartCoroutine("OnActionRoutine");
            }
            */
        }
    }
        
    private void OnTriggerStay(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }
        if (GameBucket.Instance.PXController.OnAction) { return; }
        
        if (other.tag == "Player") {
            EnqueueContinuous();
        }
    }    

    private void OnTriggerExit(Collider other) {
        if (!onAction) { return; }

        if (GameBucket.Instance.PXController.OnDialog) { return; }

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
            if (GameBucket.Instance.PXController.OnDialog) { continue; }

            if (GameBucket.Instance.PXController.CanInteract) {
                _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
                GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);

                CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
                yield return null;
            } else { yield return null; }
        }

        yield break;
    }

    private IEnumerator OnActionRoutine() {
        yield return null;

        while (onAction) {            
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);            
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);       
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
