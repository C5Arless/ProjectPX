using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasHandler : MonoBehaviour {
    [SerializeField] GameObject _dialogWindow;

    private void Awake() {
        GameBucket.Instance.GameCanvasHandler = this;
    }

    public void DialogIn() {        
        Vector3 startPos = new Vector3(0f, -150f, 0f);
        Vector3 targetPos = new Vector3(0f, 0f, 0f);

        StartCoroutine(IteratePosition(startPos, targetPos));
    }

    public void DialogOut() {
        Vector3 startPos = new Vector3(0f, 0f, 0f);
        Vector3 targetPos = new Vector3(0f, -150f, 0f);

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
                Debug.Log("DialogIn");
                yield return null;
            }

            //_dialogWindow.transform.position = targetPos;

            yield break;
        } else {            
            Vector3 position = startPos;

            _dialogWindow.transform.position = position;

            while (position.y > targetPos.y) {
                position.y = position.y - 30f;
                _dialogWindow.transform.localPosition = position;
                Debug.Log("DialogOut");
                yield return null;
            }

            //_dialogWindow.transform.position = targetPos;

            _dialogWindow.SetActive(false);

            yield break;
        }
    }
}
