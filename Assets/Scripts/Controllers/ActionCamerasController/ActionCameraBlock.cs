using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionCameraBlock : MonoBehaviour {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;
    [SerializeField] GameObject _cameraPivot;
    [SerializeField] bool lockPlayer;
    [SerializeField] bool hasFollow;

    private bool isLocked;
    private bool onAction;

    private void OnTriggerEnter(Collider other) {
        if (GameBucket.Instance.PXController.OnAction) { return; }

        if (other.tag == "Player") {
            onAction = true;

            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
            
            if (lockPlayer) {
                isLocked = true;
                StartCoroutine("LockPlayerPosition");
            } else {
                StartCoroutine("OnActionRoutine");
            }
        }
    }

    
    private void OnTriggerStay(Collider other) {
        if (!GameBucket.Instance.PXController.CanInteract) { return; }
        if (GameBucket.Instance.PXController.OnAction) { return; }

        if (other.tag == "Player") {
            onAction = true;
            
            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);

            if (lockPlayer) {
                isLocked = true;
                StartCoroutine("LockPlayerPosition");
            }
            else {
                StartCoroutine("OnActionRoutine");
            }
        }
    }    
    

    private void OnTriggerExit(Collider other) {
        if (!onAction) { return; }

        if (GameBucket.Instance.PXController.OnDialog) { return; }

        if (other.tag == "Player") {
            onAction = false;
            isLocked = false;
            GameBucket.Instance.PXController.ActionCameraExit();
        }
    }

    private IEnumerator LockPlayerPosition() {
        yield return null;

        while (isLocked) {
            if (GameBucket.Instance.PXController.CanInteract) {
                _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
                GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
                yield return null;
            } else { yield return null; }
        }

        yield break;
    }

    private IEnumerator OnActionRoutine() {
        yield return null;

        while (onAction) {            
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
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
