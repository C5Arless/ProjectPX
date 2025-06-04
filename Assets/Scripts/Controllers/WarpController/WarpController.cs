using System.Collections;
using UnityEngine;

public class WarpController : MonoBehaviour {
    [SerializeField] GameObject _enterTrigger;
    [SerializeField] GameObject _exitTrigger;
    [SerializeField] GameObject _sceneTrigger;

    private bool _state;

    private void Start() {
        StartCoroutine(InitializeSwitch());
    }

    private void SwitchToEnter() {
        _state = true;
        SwitchTrigger();
    }    

    private void SwitchTrigger() {
        if (_state) {
            _exitTrigger.GetComponent<CinematicController>().AbortEvent();
            _exitTrigger.SetActive(false);
            _sceneTrigger.SetActive(true);
            _enterTrigger.SetActive(true);
        } else {
            _enterTrigger.GetComponent<CinematicController>().AbortEvent();
            _sceneTrigger.SetActive(false);
            _enterTrigger.SetActive(false);           
            _exitTrigger.SetActive(true);
        }
    }
    
    private IEnumerator InitializeSwitch() {
        while (GameBucket.Instance.PXController == null) {
            yield return null;
        }

        yield return new WaitWhile(() => GameBucket.Instance.PXController.OnDialog);
        
        yield return new WaitForSeconds(1f);

        SwitchToEnter();

        yield break;
    }
}