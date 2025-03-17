using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Vector3 startPos = new Vector3(0f, -800f, 0f);
        Vector3 targetPos = new Vector3(0f, -350f, 0f);  

        StartCoroutine(IteratePosition(startPos, targetPos));        
    }

    public void DialogOut() {
        Vector3 startPos = new Vector3(0f, -350f, 0f);
        Vector3 targetPos = new Vector3(0f, -800f, 0f);

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

    private IEnumerator IteratePosition(Vector3 startPos, Vector3 targetPos) {
        _isBusy = true;

        if (startPos.y < targetPos.y) {            
            _dialogWindow.SetActive(true);
            Vector3 position = startPos;

            _dialogWindow.transform.position = position;

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

            _dialogWindow.transform.position = position;

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

        _name.text = GameBucket.Instance.GetDialogObject(IDX).Name;
        yield return null;

        _isTyping = true;

        string line = GameBucket.Instance.GetDialogObject(IDX).Line;
        while (!string.Equals(_dialogText.text, line)) {
            string subline = line.Substring(0, _dialogText.text.Length + 1);
            char lastchar = subline.ToCharArray()[subline.Length - 1];

            if (lastchar != ' ') {
                AudioManager.Instance.PlayVoice();
            }

            _dialogText.text = subline;

            if (!_isTyping) {
                _dialogText.text = line;
                _isBusy = true;
                yield break;
            }

            yield return new WaitForSeconds(Mathf.PingPong(.001f, .2f));
        }

        _isTyping = false;
        _isBusy = false;
        yield break;
    }
}
