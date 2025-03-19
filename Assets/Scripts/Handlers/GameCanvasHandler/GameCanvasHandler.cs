using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameCanvasHandler : MonoBehaviour {
    [SerializeField] GameObject _dialogWindow;
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _dialogText;

    public delegate void OnTransitionInDone();
    public OnTransitionInDone _onTransitionInDone;

    private bool _isBusy;
    private bool _isTyping;

    public bool IsBusy { get { return _isBusy; } }
    public bool IsTyping { get { return _isTyping; } }

    private void Awake() {
        GameBucket.Instance.GameCanvasHandler = this;
    }

    private void Start() {
        _onTransitionInDone += () => { };
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

    private Vector2 GetMood(string mood) {         
        switch (mood) {
            case "Default": {
                    return new Vector2(.01f, .2f);            
                }
            case "Sad": {
                    return new Vector2(.1f, .5f);
                }
            case "Scared": {
                    return new Vector2(.005f, .02f);
                }
            case "Angry": {
                    return new Vector2(.05f, .05f);
                }
            case "Excited": {
                    return new Vector2(.05f, .02f);
                }
            default: {
                    return Vector2.zero;
                }
        }
    }

    private IEnumerator IteratePosition(Vector3 startPos, Vector3 targetPos) {
        _isBusy = true;
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

            _isBusy = false;
            yield break;
        }
    }

    private IEnumerator DisplayDialog(int IDX) {
        yield return null;

        yield return new WaitWhile(() => _isBusy);

        _isBusy = true;

        string name = GameBucket.Instance.GetDialogObject(IDX).Name;
        _name.text = name;
        yield return null;

        _isTyping = true;

        string mood = GameBucket.Instance.GetDialogObject(IDX).Mood;
        Vector2 targetMood = GetMood(mood);
        string line = GameBucket.Instance.GetDialogObject(IDX).Line;

        while (!string.Equals(_dialogText.text, line)) {
            string subline = line.Substring(0, _dialogText.text.Length + 1);
            char lastchar = subline.ToCharArray()[subline.Length - 1];

            if (lastchar != ' ') {                
                AudioManager.Instance.PlayVoice(name, mood);
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
