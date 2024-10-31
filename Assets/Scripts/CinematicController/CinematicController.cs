using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public struct InteractionVCameras {
    public GameObject vcamera;
    public int pageIdx;
}
*/

public class CinematicController : MonoBehaviour {
    [SerializeField] int shots;
    [SerializeField] InteractionVCameras[] cinematicCams;
    [SerializeField] GameObject[] _focusTargetDrawer;
    [SerializeField] GameObject[] _playerTargetDrawer;
    [SerializeField] GameObject[] _companionTargetDrawer;
    [SerializeField] GameObject[] _dollyTracksDrawer;

    private Collider _playerCollider;

    private int shotNumber;

    private bool isInteracting;
    private bool isBusy;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {            
            _playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _playerCollider = null;
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

        PXController _ctx = _playerCollider.GetComponent<PXController>();
        _ctx.DialogEnter(_playerTargetDrawer[0].transform, _focusTargetDrawer[0].transform, cinematicCams[0].vcamera);

        CompanionController _comp = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
        _comp.TravelSetUpTalkBehaviour(_companionTargetDrawer[0].transform.position);
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

        PXController _ctx = _playerCollider.GetComponent<PXController>();
        _ctx.DialogExit();

        CompanionController _comp = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
        _comp.ExitTalkState();
    }

    private IEnumerator IteratePage() {
        isBusy = true;
        yield return null;

        shotNumber++;
        Debug.Log("Page number: " + shotNumber);
        yield return null;

        foreach (InteractionVCameras _ivcam in cinematicCams) {
            if (_ivcam.pageIdx == shotNumber) {
                CameraManager.Instance.SwitchGameVCamera(_ivcam.vcamera);
            }
        }

        isBusy = false;
        yield break;
    }

}

