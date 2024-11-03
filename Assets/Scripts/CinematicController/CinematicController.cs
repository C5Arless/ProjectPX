using System.Collections;
using UnityEngine;

public class CinematicController : MonoBehaviour {
    [SerializeField] int shots;
    [SerializeField] CinematicVCameras[] cinematicCams;
    [SerializeField] GameObject[] _focusTargetDrawer;
    [SerializeField] GameObject[] _playerTargetDrawer;
    [SerializeField] GameObject[] _companionTargetDrawer;
    [SerializeField] GameObject[] _dollyTracksDrawer;

    private PXController _playerCtx;
    private CompanionController _companionCtx;

    private int shotNumber;

    private bool isInteracting;
    private bool isBusy;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {            
            _playerCtx = other.GetComponent<PXController>();
            
            try {
                _companionCtx = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
            } catch {
                Debug.Log("Companion not found");
            }
        }        
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _playerCtx = null;
            _companionCtx = null;
        }
    }

    public void OnInteract() {
        if (isBusy) { return; }

        if (isInteracting) {
            Continue();
        }
        else {
            Enter();
        }
    }

    private void Enter() {
        shotNumber = 1;
        
        isInteracting = true;

        Debug.Log("Current pages: " + shots);
        Debug.Log("Page number: " + shotNumber);

        _playerCtx.DialogEnter(_playerTargetDrawer[0].transform, _focusTargetDrawer[0].transform, cinematicCams[0].vcamera);

        _companionCtx.TravelSetUpTalkBehaviour(_companionTargetDrawer[0].transform.position);
    }

    private void Continue() {
        if (shotNumber >= shots) {
            Exit();
        }
        else {
            SwitchPage();
        }
    }

    private void SwitchPage() {
        StartCoroutine(IteratePage());
    }

    private void Exit() {
        isInteracting = false;

        _playerCtx.DialogExit();

        _companionCtx.ExitTalkState();
    }

    private IEnumerator IteratePage() {
        isBusy = true;
        yield return null;

        shotNumber++;
        Debug.Log("Page number: " + shotNumber);
        yield return null;

        foreach (CinematicVCameras _cinematicCam in cinematicCams) {
            if (_cinematicCam.shotIdx == shotNumber) {
                CameraManager.Instance.SwitchGameVCamera(_cinematicCam.vcamera);
            }
        }

        isBusy = false;
        yield break;
    }

}

