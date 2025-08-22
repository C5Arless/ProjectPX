using Cinemachine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class WarpController : MonoBehaviour {
    [SerializeField] GameObject _enterTrigger;
    [SerializeField] GameObject _exitTrigger;
    [SerializeField] GameObject _sceneTrigger;

    private CinematicController _enterCtx;
    private CinematicController _exitCtx;
    private CinemachineBrain _gameBrain;

    private Task _switchTask;

    private bool _state;

    private void Start() {
        RetrieveControllers();

        _switchTask = SwitchTask();
        //StartCoroutine(InitializeSwitch());
    }

    private void RetrieveControllers() {
        _enterCtx = _enterTrigger.GetComponent<CinematicController>();
        _exitCtx = _exitTrigger.GetComponent<CinematicController>();

        _gameBrain = CameraManager.Instance.GameBrain.GetComponent<CinemachineBrain>();
    }

    private void SwitchToEnter() {
        _state = true;
        SwitchTrigger();
    }    

    private void SwitchTrigger() {
        if (_state) {
            _exitCtx.AbortEvent();            
            _exitTrigger.SetActive(false);
            _sceneTrigger.SetActive(true);
            _enterTrigger.SetActive(true);
        } else {
            _enterCtx.AbortEvent();
            _sceneTrigger.SetActive(false);
            _enterTrigger.SetActive(false);           
            _exitTrigger.SetActive(true);
        }
    }
    
    private async Task SwitchTask() {
        await Task.Yield();        
        await await GameBucket.Instance.EventsOrchestrator.GetCurrentEventTask();     

        SwitchToEnter();
        await Task.Yield();
    }

    private IEnumerator InitializeSwitch() {
        while (GameBucket.Instance.PXController == null) {
            yield return null;
        }

        yield return new WaitWhile(() => _exitCtx.IsInteracting);
        
        yield return new WaitForSeconds(1f);

        SwitchToEnter();

        yield break;
    }
}