using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    [SerializeField] GameObject[] _managerPrefabs;

    private bool gameStarted;

    private void Awake() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Game Master Start");

        StartCoroutine(InitializeManagers());

    }
    
    private IEnumerator InitializeManagers() {
        foreach (GameObject _prefab in _managerPrefabs) {
            Instantiate(_prefab, Vector3.zero, new Quaternion());
            yield return null;
        }

        DataManager.Instance.InitializeData();
        yield return null;

        VideoManager.Instance.InitializeVideoSettings();
        yield return null;

        AudioManager.Instance.InitializeMixerVolumes();
        yield return null;

        CameraManager.Instance.InitializeCameras();
        yield return null;

        ScenesManager.Instance.MainMenu();
        yield return null;

        AudioManager.Instance.PlayMusic(MusicTracks.MainMenu_Intro);
        yield return null;

        yield break;
    }
}
