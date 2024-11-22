using UnityEngine;

public class ActionCameraBlock : MonoBehaviour {
    [SerializeField] GameObject _playerPos;
    [SerializeField] GameObject _actionVCamera;

    private bool pause = true;

    private void Update() {
        if (!pause) {
            _playerPos.transform.position = GameBucket.Instance.PXController.transform.position;
            GameBucket.Instance.PXController.UpdateExternalCamera(_playerPos.transform, _actionVCamera.transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            pause = false;
            GameBucket.Instance.PXController.ActionCameraEnter();
            CameraManager.Instance.SwitchGameVCamera(_actionVCamera);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            pause = true;
            GameBucket.Instance.PXController.ActionCameraExit();
        }
    }
}
