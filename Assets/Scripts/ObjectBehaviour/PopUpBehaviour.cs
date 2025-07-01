using Cinemachine;
using UnityEngine;

public class PopUpBehaviour : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(CameraManager.Instance.GetCurrentViewCamera().transform);
    }

    private void LookToCamera() {
        //CinemachineVirtualCamera camera = CameraManager.Instance.GetCurrentGameCamera();
        //Transform target = camera.
    }
}
