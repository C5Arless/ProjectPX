using System.Collections;
using UnityEngine;

public class CinematicController : MonoBehaviour {
    [SerializeField] int shots;
    [SerializeField] CinematicShot[] _cinematicShots;

    private PXController _playerCtx;
    private CompanionController _companionCtx;

    private int shotNumber;

    private bool isInteracting;
    private bool isBusy;

    public PXController PlayerCtx { get { return _playerCtx; } }
    public CompanionController CompanionCtx { get { return _companionCtx; } }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {            
            _playerCtx = other.GetComponent<PXController>();
            
            try {
                _companionCtx = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
            } catch {
                Debug.Log("Companion not found");
            }

            Enter();
        }        
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _playerCtx = null;
            _companionCtx = null;
        }
    }

    private void OnInteract() {
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

        _cinematicShots[shotNumber - 1].Enter();

        /*
        _playerCtx.DialogEnter(_cinematicShots[0].PlayerTarget.transform, _focusTargetDrawer[0].transform, cinematicCams[0].vcamera);
        _companionCtx.TravelSetUpTalkBehaviour(_companionTargetDrawer[0].transform.position);
        */
    }

    private void Continue() {
        if (shotNumber >= shots) {
            Exit();
        }
        else {
            NextShot();
        }
    }

    private void NextShot() {
        StartCoroutine(IterateShot());
    }

    private void Exit() {
        isInteracting = false;

        /*
        _playerCtx.DialogExit();
        _companionCtx.ExitTalkState();
        */

        _cinematicShots[shotNumber].Exit();
    }

    private IEnumerator IterateShot() {
        isBusy = true;
        yield return null;

        shotNumber++;
        Debug.Log("Page number: " + shotNumber);
        yield return null;

        _cinematicShots[shotNumber].Enter();

        /*
        foreach (CinematicVCameras _cinematicCam in cinematicCams) {
            if (_cinematicCam.shotIdx == shotNumber) {
                CameraManager.Instance.SwitchGameVCamera(_cinematicCam.vcamera);
            }
        }
        */

        isBusy = false;
        yield break;
    }

}

