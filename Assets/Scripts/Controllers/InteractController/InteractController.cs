using System.Collections;
using UnityEngine;

public class InteractController : MonoBehaviour {    
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _popUp;

    [SerializeField] int pages;
    [SerializeField] InteractionVCameras[] interactionCams;

    private int pageNumber;

    private bool isInteracting;
    private bool isBusy;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(true);            
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            _popUp.SetActive(false);            
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

        GameBucket.Instance.PXController.DialogEnter(_playerTarget.transform, _focusTarget.transform, interactionCams[0].vcamera);

        GameBucket.Instance.CompanionCtx.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
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

        StartCoroutine(ResetCollider());
        
        GameBucket.Instance.PXController.InteractionExit();

        GameBucket.Instance.CompanionCtx.ExitTalkState();

    }

    private IEnumerator IteratePage() {
        isBusy = true;
        yield return null;

        pageNumber++;
        yield return null;

        foreach (InteractionVCameras _ivcam in interactionCams) {
            if (_ivcam.pageIdx == pageNumber) {
                CameraManager.Instance.SwitchGameVCamera(_ivcam.vcamera);
            }
        }

        isBusy = false;
        yield break;
    }
    
    private IEnumerator ResetCollider() {
        transform.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(3f);

        transform.GetComponent<Collider>().enabled = true;
        yield break;
    }
}
