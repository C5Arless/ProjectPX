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

    /*
    public void SwitchToExit() {
        _state = false;
        SwitchTrigger();
    }
    */

    private void SwitchTrigger() {
        if (_state) {
            _exitTrigger.SetActive(false);
            _sceneTrigger.SetActive(true);
            _enterTrigger.SetActive(true);
        } else {
            _exitTrigger.SetActive(true);
            _sceneTrigger.SetActive(false);
            _enterTrigger.SetActive(false);
        }
    }
    
    private IEnumerator InitializeSwitch() {
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => GameBucket.Instance.PXController.CanInteract);

        SwitchToEnter();

        yield break;
    }
}