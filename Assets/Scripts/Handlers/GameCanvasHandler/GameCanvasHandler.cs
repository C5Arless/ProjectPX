using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasHandler : MonoBehaviour {
    [SerializeField] GameObject _dialogWindow;
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _dialogText;
    [Space]
    [SerializeField] GameObject _uiWindow;
    [SerializeField] Slider _lifeDisplay;

    public delegate void OnTransitionInDone();
    public OnTransitionInDone _onTransitionInDone;

    private bool _isBusy;
    private bool _isTyping;
    private bool _isTransitioning;

    public bool IsTransitioning { get { return _isTransitioning; } }
    public bool IsTyping { get { return _isTyping; } }

    private void Awake() {
        GameBucket.Instance.GameCanvasHandler = this;
    }

    private void Start() {
        _onTransitionInDone += () => { };

        InitializeRenderCamera();
    }

    public void ShowUI() {
        if (_isBusy) { return; }

        _isBusy = true;

        _uiWindow.SetActive(true);
        _lifeDisplay.value = DataManager.Instance.PlayerInfo.CurrentHp;
        //Add info here

        StartCoroutine(IterateUIPosition());
    }

    public void DialogIn() {        
        Vector3 startPos = new Vector3(0f, -500f, 0f);
        Vector3 targetPos = new Vector3(0f, -60f, 0f);  

        StartCoroutine(IteratePosition(startPos, targetPos));        
    }

    public void DialogOut() {
        Vector3 startPos = new Vector3(0f, -60f, 0f);
        Vector3 targetPos = new Vector3(0f, -500f, 0f);

        _isTyping = false;
        AudioManager.Instance.StopVoice();
        StartCoroutine(IteratePosition(startPos, targetPos));
    }

    public void DialogWrite(int IDX) {
        StartCoroutine(DisplayDialog(IDX));
    }

    public void DialogClear() {
        StopAllCoroutines();

        AudioManager.Instance.StopVoice();

        _isBusy = false;
        _name.text = string.Empty;
        _dialogText.text = string.Empty;        
    }

    public void DialogSkip() {
        _isTyping = false;

        AudioManager.Instance.StopVoice();
    }

    private Vector2 GetMood(VoiceMood mood) {         
        switch (mood) {
            case VoiceMood.Default: {
                    return new Vector2(.01f, .2f);            
                }
            case VoiceMood.Sad: {
                    return new Vector2(.1f, .5f);
                }
            case VoiceMood.Scared: {
                    return new Vector2(.005f, .02f);
                }
            case VoiceMood.Angry: {
                    return new Vector2(.05f, .05f);
                }
            case VoiceMood.Excited: {
                    return new Vector2(.05f, .02f);
                }
            default: {
                    return Vector2.zero;
                }
        }
    }

    private void InitializeRenderCamera() {
        _dialogWindow.GetComponentInParent<Canvas>().worldCamera = CameraManager.Instance.GameBrain.GetComponent<Camera>();
    }

    private IEnumerator IterateUIPosition() {        
        //Show
        while (_uiWindow.transform.localPosition.y >= 350f) {
            Vector3 distance = new Vector3(0f, 50f, 0f);
            _uiWindow.transform.localPosition -= distance;
            yield return null;
        }
        _uiWindow.transform.localPosition = new Vector3(0f, 350f, 0f);

        yield return new WaitForSecondsRealtime(4f);
        
        //Hide
        while (_uiWindow.transform.localPosition.y <= 700f) {
            Vector3 distance = new Vector3(0f, 50f, 0f);
            _uiWindow.transform.localPosition += distance;
            yield return null;
        }
        _uiWindow.transform.localPosition = new Vector3(0f, 700f, 0f);
        _uiWindow.SetActive(false);
        
        yield return null;
        _isBusy = false;

        yield break;
    }

    private IEnumerator IteratePosition(Vector3 startPos, Vector3 targetPos) {
        _isBusy = true;
        _isTransitioning = true;
        _dialogWindow.transform.localPosition = startPos;

        if (startPos.y < targetPos.y) {            
            _dialogWindow.SetActive(true);
            Vector3 position = startPos;

            _dialogWindow.transform.localPosition = position;

            while (position.y < targetPos.y) {
                position.y = position.y + 30f;
                _dialogWindow.transform.localPosition = position;
                
                yield return null;
            }

            _onTransitionInDone();

            _isTransitioning = false;
            _isBusy = false;
            yield break;

        } else {            
            Vector3 position = startPos;

            _dialogWindow.transform.localPosition = position;

            while (position.y > targetPos.y) {
                position.y = position.y - 30f;
                _dialogWindow.transform.localPosition = position;
                
                yield return null;
            }

            _name.text = string.Empty;
            _dialogText.text = string.Empty;

            _dialogWindow.SetActive(false);

            _isTransitioning = false;
            _isBusy = false;
            yield break;
        }
    }

    private IEnumerator DisplayDialog(int IDX) {
        yield return null;

        yield return new WaitWhile(() => _isBusy);

        _isBusy = true;

        string name = GameBucket.Instance.GetDialogObject(IDX).Name;
        VoiceName nameData = GameBucket.Instance.GetDialogData(IDX).Name;
        _name.text = name;
        yield return null;

        _isTyping = true;

        string mood = GameBucket.Instance.GetDialogObject(IDX).Mood;
        VoiceMood moodData = GameBucket.Instance.GetDialogData(IDX).Mood;

        Vector2 targetMood = GetMood(moodData);
        string line = GameBucket.Instance.GetDialogObject(IDX).Line;

        while (!string.Equals(_dialogText.text, line)) {
            string subline = line.Substring(0, _dialogText.text.Length + 1);
            char lastchar = subline.ToCharArray()[subline.Length - 1];

            if (lastchar != ' ') {                
                AudioManager.Instance.PlayVoice(nameData, moodData);
            }

            _dialogText.text = subline;

            if (!_isTyping) {
                _dialogText.text = line;
                _isBusy = true;
                yield break;
            }

            yield return new WaitForSeconds(Mathf.PingPong(targetMood.x, targetMood.y));
        }

        _isTyping = false;
        _isBusy = false;
        yield break;
    }
}
