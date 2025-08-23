using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance { get; private set; }

    [SerializeField] GameObject gameBrain;
    [SerializeField] GameObject menuBrain;
    private CinemachineBrain _gBrain;
    private CinemachineBrain _mBrain;

    private GameObject currentBrain;

    private List<GameObject> menuVCameras = new List<GameObject>();
    private GameObject currentMenuVCamera;

    private GameObject currentGameVCamera;
    private Camera gameCamera;

    private VCameraMode mode;

    public GameObject CurrentGameCamera { get { return currentGameVCamera; } set { currentGameVCamera = value; } }
    public GameObject GameBrain { get { return gameBrain; } }
    public GameObject MenuBrain { get { return menuBrain; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }        
    }

    private void Start() {
        _gBrain = gameBrain.GetComponent<CinemachineBrain>();
        _mBrain = menuBrain.GetComponent<CinemachineBrain>();
    }

    public Camera GetCurrentViewCamera() {
        return gameCamera;
    }

    public void InitializeDebugMode() {
        currentBrain = gameBrain;
        currentBrain.SetActive(true);

        mode = VCameraMode.GameVCameras;
    }

    public void InitializeCameras() {
        try {
            currentMenuVCamera = menuVCameras[(int)MenuVCameras.MainScreen].gameObject;
            currentMenuVCamera.SetActive(true);

            currentBrain = menuBrain;
            currentBrain.SetActive(true);
            
            mode = VCameraMode.MenuVCameras;
        }
        catch {
            InitializeVCameras();
            Invoke("InitializeCameras", .5f);
        }
    }

    public void InitializeVCameras() {
        StartCoroutine("RetrieveVCameras");
    }

    public void PauseOut() {
        StartCoroutine("PauseCameraOut");
    }

    public void PauseIn() {
        StartCoroutine("PauseCameraIn");
    }

    public void StartGame() {
        SwitchBrain(gameBrain);
    }

    public void SetCameraMode(VCameraMode target) {
        if (mode == target) { return; }

        if (target == VCameraMode.MenuVCameras) {
            SwitchBrain(menuBrain);
        } else {
            currentMenuVCamera.gameObject.SetActive(false);
            SwitchBrain(gameBrain);
        }

        mode = target;
    }

    public void MenuToSlot() {
        if (currentMenuVCamera == menuVCameras[(int)MenuVCameras.Menu]) {
            SwitchMenuVCamera(MenuVCameras.Slots);
        } else {
            SwitchMenuVCamera(MenuVCameras.Menu);
        }       
    }    

    public void SwitchGameVCamera(GameObject target) {
        if (currentGameVCamera != null) {
            if (currentGameVCamera != target) StartCoroutine(SwitchGameCamera(target));

        } else {
            currentGameVCamera = target;
            currentGameVCamera.SetActive(true);
            gameCamera = gameBrain.GetComponent<CinemachineBrain>().OutputCamera;
        }
        /*
        if (currentGameVCamera != target) {
            target.SetActive(true);

            try {
                currentGameVCamera.SetActive(false);
            } catch {
                //Debug.Log("Current game camera is null!");
            }

            currentGameVCamera = target;
            gameCamera = gameBrain.GetComponent<CinemachineBrain>().OutputCamera;
        }
        */
    }

    public void SwitchMenuVCamera(MenuVCameras target) {
        if (currentMenuVCamera != menuVCameras[(int)target]) {
            menuVCameras[(int)target].gameObject.SetActive(true);
            currentMenuVCamera.gameObject.SetActive(false);

            currentMenuVCamera = menuVCameras[(int)target].gameObject;
        }
    }

    private void SwitchBrain(GameObject targetBrain) {
        if (targetBrain != currentBrain) {
            targetBrain.SetActive(true);

            if (currentBrain != null) { currentBrain.SetActive(false); }            

            currentBrain = targetBrain;
        }
    }    

    private IEnumerator PauseCameraOut() {
        Time.timeScale = 0f;
        InputManager.Instance.SetActionMap("UI");

        if (!menuVCameras[(int)MenuVCameras.PauseStart].gameObject.activeSelf) {
            menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(true);
            currentMenuVCamera = menuVCameras[(int)MenuVCameras.PauseStart].gameObject;
        }

        SwitchBrain(menuBrain);
        yield return null;

        menuVCameras[(int)MenuVCameras.PauseEnd].gameObject.SetActive(true);
        menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(false);
        currentMenuVCamera = menuVCameras[(int)MenuVCameras.PauseEnd].gameObject;

        yield break;
    }

    private IEnumerator PauseCameraIn() {
        menuVCameras[(int)MenuVCameras.PauseStart].gameObject.SetActive(true);
        menuVCameras[(int)MenuVCameras.PauseEnd].gameObject.SetActive(false);
        currentMenuVCamera = menuVCameras[(int)MenuVCameras.PauseStart].gameObject;
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

    private IEnumerator SwitchGameCamera(GameObject target) {        
        target.SetActive(true);
        yield return null;

        yield return new WaitWhile(() => _gBrain.IsBlending);
        yield return null;

        currentGameVCamera.SetActive(false);
        
        currentGameVCamera = target;
        currentGameVCamera.SetActive(true);

        gameCamera = gameBrain.GetComponent<CinemachineBrain>().OutputCamera;

        yield break;
    }
}