using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    [SerializeField] GameObject[] _managerPrefabs;

    public bool CinematicPause;
    public bool GamePause;
    private bool isHitting;

    public delegate void OnGamePaused();
    public OnGamePaused _onGamePaused = () => { };
    public delegate void OnGameUnpaused();
    public OnGameUnpaused _onGameUnpaused = () => { };
    
    public delegate void OnCutscenePause();
    public OnCutscenePause _OnCutscenePause = () => { };
    public delegate void OnCutsceneUnpause();
    public OnCutsceneUnpause _OnCutsceneUnpause = () => { };

    public delegate void OnHitStop();
    public OnHitStop _OnHitStop = () => { };

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
        GamePause = true;

        _onGamePaused();
    }

    public void UnpauseGame() {
        Time.timeScale = 1f;
        GamePause = false;

        _onGameUnpaused();
    }

    public void CutscenePause() {
        CinematicPause = true;
        
        _OnCutscenePause();        
    }

    public void CutsceneUnpause() {
        CinematicPause = false;
        
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

    public IEnumerator ResolveHitStop() {
        if (isHitting) yield break;
        isHitting = true;
        
        _OnHitStop();
        
        Time.timeScale = .01f;
        yield return new WaitForSecondsRealtime(.2f);
        
        Time.timeScale = 1f;
        isHitting = false;
    }
}
