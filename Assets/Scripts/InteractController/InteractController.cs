using System.Collections;
using UnityEngine;

public class InteractController : MonoBehaviour {    
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _popUp;

    [SerializeField] int pages;
    [SerializeField] InteractionVCameras[] interactionCams;

    private Collider _playerCollider;

    private int pageNumber;

    private bool isInteracting;
    private bool isBusy;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(true);
            _playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(false);
            _playerCollider = null;
        }
    }

    public void OnInteract() {
        if (isBusy) { return; }

        if (isInteracting) {
            Continue();
        } else {
            Enter();  
        }
    }   

    private void Enter() {
        pageNumber = 1;
        
        _popUp.SetActive(false);
        isInteracting = true;

        Debug.Log("Current pages: " + pages);
        Debug.Log("Page number: " + pageNumber);

        PXController _ctx = _playerCollider.GetComponent<PXController>();
        _ctx.DialogEnter(_playerTarget.transform, _focusTarget.transform, interactionCams[0].vcamera);

        CompanionController _comp = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
        _comp.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
    }

    private void Continue() {
        if (pageNumber >= pages) {
            Exit();
        } else {
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

        pageNumber++;
        Debug.Log("Page number: " + pageNumber);
        yield return null;

        foreach (InteractionVCameras _ivcam in interactionCams) {
            if (_ivcam.pageIdx == pageNumber) {
                CameraManager.Instance.SwitchGameVCamera(_ivcam.vcamera);
            }
        }

        isBusy = false;
        yield break;
    }

}
