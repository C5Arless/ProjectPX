using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractController : MonoBehaviour {    
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _popUp;

    [SerializeField] int pages;

    [SerializeField] InteractionVCameras[] interactionCams;
    [SerializeField] DialogPages[] _DialogPages;

    private int pageNumber;

    private bool isInteracting;
    private bool isBusy;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(true);            
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(false);            
        }
    }

    public void OnInteract() {
        if (isBusy) { return; }

        if (isInteracting) {
            Continue();
        } else {
            Enter();  
        }
    }   

    private void Enter() {
        pageNumber = 1;
        
        _popUp.SetActive(false);
        isInteracting = true;

        StartCoroutine(EnterRoutine());

        /*
        GameBucket.Instance.PXController.DialogEnter(_playerTarget.transform, _focusTarget.transform, interactionCams[0].vcamera);

        GameBucket.Instance.CompanionCtx.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
        GameBucket.Instance.CompanionCtx.VisionSetUpTalkBehaviour(_focusTarget);

        GameBucket.Instance.GameCanvasHandler.DialogIn();
        GameBucket.Instance.GameCanvasHandler.DialogWrite(_DialogPages[pageNumber - 1].dialogIdx);
        */
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
        yield return null;
    }

    private IEnumerator ExitRoutine() {
        isBusy = true;
        GameBucket.Instance.PXController.InteractionExit();
        GameBucket.Instance.CompanionCtx.ExitTalkState();

        GameBucket.Instance.GameCanvasHandler.DialogOut();

        yield return new WaitForSeconds(1f);

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
            UnityEditor.Handles.Label(_focusTarget.transform.position, "FocusTarget");
        }

        if (_playerTarget != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_playerTarget.transform.position, "PlayerTarget");
        }

        if (_companionTarget != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_companionTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_companionTarget.transform.position, "CompanionTarget");
        }
    }

    #endif
}
