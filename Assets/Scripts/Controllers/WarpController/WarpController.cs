using System.Collections;
using UnityEngine;

public class WarpController : MonoBehaviour {
    [SerializeField] GameObject _enterTrigger;
    [SerializeField] GameObject _exitTrigger;
    [SerializeField] GameObject _sceneTrigger;

    private bool _state;

    private void Start() {
        Invoke("SwitchToEnter", 1.25f);
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
    
}