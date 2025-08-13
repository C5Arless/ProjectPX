using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMode : MonoBehaviour {
    [SerializeField] GameObject[] _managerPrefabs;
    [SerializeField] GameObject _gameMaster;
    [SerializeField] GameObject _inputManager;
    [SerializeField] Canvas _debugCanvas;

    private InputAction _debugSelector;
    private InputAction _enableDebug;

    private bool isRunning;

    void Start() {
        Instantiate(_inputManager);

        InitializeActions();
        SubscribeCallbacks();

        Invoke("DeployGame", 3f);
    }

    private void OnDestroy() {
        UnsubscribeCallbacks();    
    }

    private void SubscribeCallbacks() {
        _debugSelector.started += OnDebugSelector;
        _enableDebug.started += OnEnableDebug;
    }

    private void UnsubscribeCallbacks() {
        _debugSelector.started -= OnDebugSelector;
        _enableDebug.started -= OnEnableDebug;
    }

    private void InitializeActions() {
        _debugSelector = InputManager.Instance.GetPlayerInput().actions["DebugSelector"];
        _enableDebug = InputManager.Instance.GetPlayerInput().actions["EnableDebug"];
    }

    private void OnDebugSelector(InputAction.CallbackContext _input) {
        string inputKey = _input.control.name;
        int sceneKey = int.Parse(inputKey);        

        switch (sceneKey) {
            case 1:
                StartCoroutine(InitializeDebugMode(Scenes.Lab, Cp.CP_0));
                break;
            case 2:
                StartCoroutine(InitializeDebugMode(Scenes.Lab, Cp.CP_1));
                break;
            case 3:
                StartCoroutine(InitializeDebugMode(Scenes.Map1, Cp.CP_0));
                break;
            case 4:
                StartCoroutine(InitializeDebugMode(Scenes.Warp1, Cp.CP_0));
                break;
            case 5:
                StartCoroutine(InitializeDebugMode(Scenes.Warp2, Cp.CP_0));
                break;
            case 6:
                StartCoroutine(InitializeDebugMode(Scenes.Warp3, Cp.CP_0));
                break;
            default: break;
        }
    }

    private void OnEnableDebug(InputAction.CallbackContext _input) {
        if (isRunning) { return; }

        if (_input.phase == InputActionPhase.Started) {
            isRunning = true;

            StartCoroutine(EnableDebugMode());
        } 
    }

    private void DeployGame() {
        if (isRunning) { return; }

        UnsubscribeCallbacks();
        StartCoroutine(DeployGameMaster());
    }

    private IEnumerator InitializeDebugMode(Scenes scene, Cp cp) {
        foreach (GameObject _prefab in _managerPrefabs) {
            Instantiate(_prefab, Vector3.zero, new Quaternion());
            yield return null;
        }

        DataManager.Instance.InitializeData();
        yield return null;

        AudioManager.Instance.InitializeMixerVolumes();
        yield return null;

        CameraManager.Instance.InitializeDebugMode();
        yield return null;

        InputManager.Instance.StartGame();
        yield return null;

        ScenesManager.Instance.Switch(scene, cp);
        yield break;
    }

    private IEnumerator DeployGameMaster() {
        Instantiate(_gameMaster);
        yield return null;
       
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator EnableDebugMode() {
        _debugCanvas.gameObject.SetActive(true);
        yield return null;

        InputManager.Instance.SetActionMap("DebugMode");
        yield break;

    }
}
