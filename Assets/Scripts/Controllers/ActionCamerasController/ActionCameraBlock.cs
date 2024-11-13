using UnityEngine;

public class ActionCameraBlock : MonoBehaviour {
    [SerializeField] ActionCamerasController _actionCamerasCtx;
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;

    private bool pause = true;

    private void Update() {
        if (!pause) {
            _playerPos.transform.position = _actionCamerasCtx.PXController.transform.position;
            _actionCamerasCtx.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            pause = false;
            _actionCamerasCtx.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            pause = true;
            _actionCamerasCtx.PXController.ActionCameraExit();
        }
    }
}
