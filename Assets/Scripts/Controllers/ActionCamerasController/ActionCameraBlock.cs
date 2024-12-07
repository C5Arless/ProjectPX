using System.Collections;
using UnityEngine;

public class ActionCameraBlock : MonoBehaviour {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;
    [SerializeField] bool lockPlayer;
    [SerializeField] bool hasFollow;

    private bool isLocked;
    private bool onAction;

    /*
    private void Update() {        
        if (CheckStayCondition()) {
            _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
        }
    }

    private bool CheckUpdateCondition() {
        if (!GameBucket.Instance.PXController.OnDialog) {
            return true;
        }
        else return false;
    }
    */

    private void OnTriggerEnter(Collider other) {
        //if (!GameBucket.Instance.PXController.CanInteract) { return; }

        if (other.tag == "Player") {
            onAction = true;

            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
            
            if (lockPlayer) {
                isLocked = true;
                StartCoroutine("LockPlayer");
            } else {
                StartCoroutine("OnActionRoutine");
            }
        }
    }

    /*
    private void OnTriggerStay(Collider other) {
        if (CheckStayCondition()) { return; }

        if (other.tag == "Player") {
            isLocked = true;
            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);

            StartCoroutine("LockPlayer");
        }
    }    
    */

    private void OnTriggerExit(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }

        if (other.tag == "Player") {
            onAction = false;
            isLocked = false;
            GameBucket.Instance.PXController.ActionCameraExit();
        }
    }

    private IEnumerator LockPlayer() {
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
            if (GameBucket.Instance.PXController.CanInteract) {
                GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
                yield return null;
            }
            else { yield return null; }
        }

        yield break;
    }
}
