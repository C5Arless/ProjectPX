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

    public bool IsBusy { get { return _isBusy; } }

    private void Awake() {
        GameBucket.Instance.GameCanvasHandler = this;
    }

    private void Start() {
        _onTransitionInDone += () => { };
    }

    public void DialogIn() {        
        Vector3 startPos = new Vector3(0f, -150f, 0f);
        Vector3 targetPos = new Vector3(0f, 0f, 0f);
        _isBusy = true;

        StartCoroutine(IteratePosition(startPos, targetPos));
    }

    public void DialogOut() {
        Vector3 startPos = new Vector3(0f, 0f, 0f);
        Vector3 targetPos = new Vector3(0f, -150f, 0f);
        _isBusy = true;

        StartCoroutine(IteratePosition(startPos, targetPos));
    }

    private IEnumerator IteratePosition(Vector3 startPos, Vector3 targetPos) {
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

            _dialogWindow.SetActive(false);

            _isBusy = false;
            yield break;
        }
    }
}
