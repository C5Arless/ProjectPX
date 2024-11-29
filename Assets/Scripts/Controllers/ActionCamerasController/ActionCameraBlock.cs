using UnityEngine;

public class ActionCameraBlock : MonoBehaviour {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;

    private bool pause = true;

    private void Update() {        
        if (CheckUpdateCondition()) {
            _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
        }
    }

    private bool CheckUpdateCondition() {
        if (!pause && !GameBucket.Instance.PXController.OnDialog) {
            return true;
        }
        else return false;
    }

    private void OnTriggerEnter(Collider other) {
        try {
            if (GameBucket.Instance.PXController.OnDialog) { return; }
        } catch {
            //
        }

        if (other.tag == "Player") {
            pause = false;
            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (CheckUpdateCondition()) { return; }

        if (other.tag == "Player") {
            pause = false;
            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (GameBucket.Instance.PXController.OnDialog) { return; }

        if (other.tag == "Player") {
            pause = true;
            GameBucket.Instance.PXController.ActionCameraExit();
        }
    }
}
