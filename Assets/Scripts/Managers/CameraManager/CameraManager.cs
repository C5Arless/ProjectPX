using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance { get; private set; }

    [SerializeField] GameObject gameBrain;
    [SerializeField] GameObject menuBrain;
    private GameObject currentBrain;

    private List<GameObject> menuVCameras = new List<GameObject>();
    private GameObject currentVCamera;

    private VCameraMode mode;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }        
    }

    private void Start() {
        SetInitialState();
    }

    public void OnPauseCamera(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {

            MenuController.Instance.Mode = UIMode.Pause;
            StartCoroutine("PauseCameraOut");
        }
    }

    public void OnResumeCamera(InputAction.CallbackContext input) {
        if (MenuController.Instance.Mode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            StartCoroutine("PauseCameraIn");            
        }
    }

    public void OnAnyButton(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {
            SwitchMenuVCamera(MenuVCameras.Menu);
        }
    }

    public void InitializeVCameras() {
        StartCoroutine("RetrieveVCameras");
    }

    public void StartGame() {
        SwitchBrain(gameBrain);
    }

    public void SetCameraMode(VCameraMode target) {
        if (mode == target) { return; }

        if (target == VCameraMode.MenuVCameras) {
            SwitchBrain(menuBrain);
        } else {
            currentVCamera.gameObject.SetActive(false);
            SwitchBrain(gameBrain);
        }

        mode = target;
    }

    public void MenuToSlot() {
        if (currentVCamera == menuVCameras[(int)MenuVCameras.Menu]) {
            SwitchMenuVCamera(MenuVCameras.Slots);
        } else {
            SwitchMenuVCamera(MenuVCameras.Menu);
        }       
    }

    private void SwitchMenuVCamera(MenuVCameras target) {
        if (currentVCamera != menuVCameras[(int)target]) {
            menuVCameras[(int)target].gameObject.SetActive(true);
            currentVCamera.gameObject.SetActive(false);
            currentVCamera = menuVCameras[(int)target].gameObject;
        }
    }

    private void SwitchBrain(GameObject targetBrain) {
        if (targetBrain != currentBrain) {
            targetBrain.SetActive(true);
            currentBrain.SetActive(false);
            currentBrain = targetBrain;
        }
    }

    private void SetInitialState() {
        try {
            currentVCamera = menuVCameras[(int)MenuVCameras.MainScreen].gameObject;
            currentVCamera.SetActive(true);

            currentBrain = menuBrain;
            currentBrain.SetActive(true);

            mode = VCameraMode.MenuVCameras;
        } catch {
            InitializeVCameras();
            Invoke("SetInitialState", .5f);
        }
    }

    private IEnumerator PauseCameraOut() {
        Time.timeScale = 0f;
        InputManager.Instance.SetActionMap("UI");

        if (!menuVCameras[(int)MenuVCameras.PauseStart].gameObject.activeSelf) {
            menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(true);
            currentVCamera = menuVCameras[(int)MenuVCameras.PauseStart].gameObject;
        }

        SwitchBrain(menuBrain);
        yield return null;

        menuVCameras[(int)MenuVCameras.PauseEnd].gameObject.SetActive(true);
        menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(false);
        currentVCamera = menuVCameras[(int)MenuVCameras.PauseEnd].gameObject;

        yield break;
    }

    private IEnumerator PauseCameraIn() {
        menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(true);
        menuVCameras[(int)MenuVCameras.PauseEnd].gameObject.SetActive(false);
        currentVCamera = menuVCameras[(int)MenuVCameras.PauseStart].gameObject;
        yield return new WaitForSecondsRealtime(.25f);

        SwitchBrain(gameBrain);
        InputManager.Instance.SetActionMap("Player");
        Time.timeScale = 1f;

        yield break;
    }

    private IEnumerator RetrieveVCameras() {
        try {
            GameObject parent = GameObject.Find("MenuVCameras");
            Cinemachine.CinemachineVirtualCamera[] cameras = parent.GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();

            foreach (Cinemachine.CinemachineVirtualCamera camera in cameras) {
                menuVCameras.Add(camera.gameObject);
                camera.gameObject.SetActive(false);                
            }
        } catch {
            //Debug.Log("Retrieve failed, trying again...");        
        }

        yield break;
    }
}