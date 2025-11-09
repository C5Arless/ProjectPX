using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    [SerializeField] GameObject[] _managerPrefabs;

    public delegate void OnGamePaused();
    public OnGamePaused _onGamePaused = () => { };
    public delegate void OnGameUnpaused();
    public OnGameUnpaused _onGameUnpaused = () => { };
    
    public delegate void OnCutscenePause();
    public OnCutscenePause _OnCutscenePause = () => { };
    public delegate void OnCutsceneUnpause();
    public OnCutsceneUnpause _OnCutsceneUnpause = () => { };

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    
    void Start() {
        Debug.Log("Game Master Start");

        StartCoroutine(InitializeManagers());

    }

    public void PauseGame() {
        Time.timeScale = 0f;

        _onGamePaused();
    }

    public void UnpauseGame() {
        Time.timeScale = 1f;

        _onGameUnpaused();
    }

    public void CutscenePause() {
        _OnCutscenePause();        
    }

    public void CutsceneUnpause() {
        _OnCutsceneUnpause();
    }
    
    private IEnumerator InitializeManagers() {
        foreach (GameObject _prefab in _managerPrefabs) {
            Instantiate(_prefab, Vector3.zero, new Quaternion());
            yield return null;
        }

        DataManager.Instance.InitializeData();
        yield return null;
        
        InputManager.Instance.InitializeInput();
        yield return null;
        
        AudioManager.Instance.InitializeMixerVolumes();
        yield return null;

        CameraManager.Instance.InitializeCameras();
        yield return null;

        VideoManager.Instance.InitializeVideoSettings();
        yield return null;
        
        ScenesManager.Instance.MainMenu();
        yield return null;        

        yield break;
    }
}
