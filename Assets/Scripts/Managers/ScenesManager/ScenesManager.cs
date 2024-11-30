using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour {
    public static ScenesManager Instance { get; private set; }
    
    [SerializeField] Material _fade;
    [SerializeField] Material _dissolve;
    [SerializeField] GameObject _menuPrefab;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _companionPrefab;
    [SerializeField] PlayerInfo _playerInfo;

    [SerializeField]
    [Range(1f, 20f)] float _fadeSpeed;

    [SerializeField]
    [Range(.1f, 2f)] float _dissolveSpeed;

    private bool paused;
    private bool globalPause;

    private Cp _targetCp;
    private GameObject _cpObj;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else { Destroy(gameObject); }
    }

    public bool IsBusy() {
        return globalPause;
    }

    public void StartGame() {
        if (paused) { return; }

        globalPause = true;
        _playerInfo.Checkpoint = new Vector2(1f, 0f);
        StartCoroutine(InitializeStart((int)Scenes.Lab));
    }

    public void LoadGame() {
        if (paused) { return; }

        globalPause = true;
        int n_scene = (int)_playerInfo.Checkpoint.x;
        StartCoroutine(InitializeLoad(n_scene));
    }

    public void MainMenu() {
        if (paused) { return; }

        globalPause = true;
        StartCoroutine(InitializeMainMenu());
    }

    public void QuitGame() {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ReloadOnDeath() {
        globalPause = true;
        StartCoroutine(OnDeath());
    }

    public void Switch(Scenes scene, Cp point) {
        if (paused) { return; }

        globalPause = true;
        int n_scene = (int)scene;        

        if (n_scene != SceneManager.GetActiveScene().buildIndex) {
            StartCoroutine(InitializeSwitch(n_scene, point));
        }
    }

    private IEnumerator InitializeMainMenu() {
        StartCoroutine(TransitionBlackOut(_dissolve));        
        yield return new WaitWhile(() => paused);

        StartCoroutine(SetLoadScene((int)Scenes.MainMenu));
        yield return new WaitWhile(() => paused);

        StartCoroutine(InstantiateMenu());
        yield return new WaitWhile(() => paused);

        yield return new WaitForSeconds(1f);

        StartCoroutine(TransitionOut(_dissolve, _dissolveSpeed));
        yield return new WaitWhile(() => paused);

        InputManager.Instance.SetActionMap("MainScreen");
        globalPause = false;
        yield break;
    }

    private IEnumerator InitializeStart(int n_scene) {
        StartCoroutine(TransitionBlackOut(_fade));
        yield return new WaitWhile(() => paused);

        StartCoroutine(SetLoadScene(n_scene));
        yield return new WaitWhile(() => paused);

        StartCoroutine(RetrievePoint(Cp.CP_2));
        yield return new WaitWhile(() => paused);

        StartCoroutine(InstantiatePlayerAndCompanion());
        yield return new WaitWhile(() => paused);

        StartCoroutine(TransitionOut(_fade, _fadeSpeed));
        yield return new WaitWhile(() => paused);        

        StartCoroutine(TransitionOut(_fade, _fadeSpeed));
        yield return new WaitWhile(() => paused);

        globalPause = false;
        yield break;
    }

    private IEnumerator InitializeLoad(int n_scene) {
        StartCoroutine(TransitionBlackOut(_fade));
        yield return new WaitWhile(() => paused);

        StartCoroutine(SetLoadScene(n_scene));
        yield return new WaitWhile(() => paused);

        StartCoroutine(RetrieveCheckpoints());
        yield return new WaitWhile(() => paused);

        StartCoroutine(InstantiatePlayerAndCompanion());
        yield return new WaitWhile(() => paused);

        StartCoroutine(InstantiateMenu());
        yield return new WaitWhile(() => paused);

        StartCoroutine(TransitionOut(_fade, _fadeSpeed));
        yield return new WaitWhile(() => paused);

        globalPause = false;
        yield break;
    }

    private IEnumerator InitializeSwitch(int n_scene, Cp _point) {
        string currentActionMap = InputManager.Instance.GetActionMap();
        InputManager.Instance.SetActionMap("Disabled");

        StartCoroutine(TransitionIn(_fade, _fadeSpeed));
        yield return new WaitWhile(() => paused);

        StartCoroutine(SetLoadScene(n_scene));
        yield return new WaitWhile(() => paused);

        StartCoroutine(RetrievePoint(_point));
        yield return new WaitWhile(() => paused);

        StartCoroutine(InstantiatePlayerAndCompanion());
        yield return new WaitWhile(() => paused);

        StartCoroutine(TransitionOut(_fade, _fadeSpeed));
        yield return new WaitWhile(() => paused);

        globalPause = false;
        InputManager.Instance.SetActionMap(currentActionMap);
        yield break;
    }

    public IEnumerator OnDeath() {
        string currentActionMap = InputManager.Instance.GetActionMap();
        InputManager.Instance.SetActionMap("Disabled");

        yield return new WaitForSeconds(2f);

        StartCoroutine(InitializeLoad(SceneManager.GetActiveScene().buildIndex));
        yield return null;

        _playerInfo.CurrentHp = 3;

        globalPause = false;
        InputManager.Instance.SetActionMap(currentActionMap);
        yield break;
    }

    private IEnumerator SetLoadScene(int n_scene) {
        paused = true;
        
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(n_scene, LoadSceneMode.Single);

        while (!sceneLoad.isDone) {
            yield return null;
        }

        Time.timeScale = 1;
        paused = false;

        yield break;

    }

    private IEnumerator TransitionIn(Material _material, float speed) {
        paused = true;
        while (_material.GetFloat("_Transition") > 0.01f) {
            float size = _material.GetFloat("_Transition");
            float lerpSize = size - speed * .01f;
            _material.SetFloat("_Transition", lerpSize);
            yield return null;
        }

        _material.SetFloat("_Transition", 0f);
        paused = false;

        yield break;
    }

    private IEnumerator TransitionOut(Material _material, float speed) {
        paused = true;
        while (_material.GetFloat("_Transition") < .98f) {
            float size = _material.GetFloat("_Transition");
            float lerpSize = size + speed * .01f;
            _material.SetFloat("_Transition", lerpSize);
            yield return null;
        }

        _material.SetFloat("_Transition", 1f);
        paused = false;

        yield break;
    }

    private IEnumerator TransitionBlackOut(Material _material) {
        paused = true;
        _material.SetFloat("_Transition", 0f);
        yield return null;

        paused = false;

        yield break;
    }

    private IEnumerator RetrieveCheckpoints() {
        paused = true;
        yield return new WaitForSeconds(.2f);

        GameObject[] _tmp = GameObject.FindGameObjectsWithTag("Checkpoint");

        if (SceneManager.GetActiveScene().buildIndex == _playerInfo.Checkpoint.x) {
            _targetCp = (Cp)_playerInfo.Checkpoint.y;
        } else { _targetCp = Cp.CP_0; }

        foreach (GameObject _obj in _tmp) {
            if (_obj.name == _targetCp.ToString()) {
                _cpObj = _obj;
                //Debug.Log("Checkpoint: " + _cpObj.name + " Found! " + (int)_targetCp);
            }

            yield return null;
        }

        yield return new WaitForSeconds(.2f);

        paused = false;
        yield break;
    }

    private IEnumerator RetrievePoint(Cp _point) {
        paused = true;
        yield return new WaitForSeconds(.2f);

        GameObject[] _tmp = GameObject.FindGameObjectsWithTag("Checkpoint");

        _targetCp = _point;        

        foreach (GameObject _obj in _tmp) {
            if (_obj.name == _targetCp.ToString()) {
                _cpObj = _obj;
                Debug.Log("Point: " + _cpObj.name + " Found! " + (int)_targetCp);
            }

            yield return null;
        }

        yield return new WaitForSeconds(.2f);

        paused = false;
        yield break;
    }

    private IEnumerator InstantiatePlayerAndCompanion() {
        paused = true;
        yield return null;

        Instantiate(_playerPrefab, _cpObj.transform.position, _cpObj.transform.rotation);
        Instantiate(_companionPrefab, _cpObj.transform.position + new Vector3(0f, 1f, 1f), _cpObj.transform.rotation);
        yield return new WaitForSeconds(.2f);

        paused = false;
        yield break;
    }

    private IEnumerator InstantiateMenu() {
        paused = true;        

        if (MenuController.Instance == null) {
            //Debug.Log("No menu found");
            Instantiate(_menuPrefab, new Vector3(0f, -100f, 0f), new Quaternion());
        } else {
            //Debug.Log("Menu found");
        }

        yield return new WaitForSeconds(.2f);

        paused = false;
        yield break;
    }

}
 