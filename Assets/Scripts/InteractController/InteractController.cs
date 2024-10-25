using System.Collections;
using UnityEngine;

public class InteractController : MonoBehaviour {
    [SerializeField] GameObject _vcam;
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _popUp;

    [SerializeField] int pages;

    private InteractionMode mode;

    private Collider _playerCollider;

    private bool isInteracting;
    private int pageNumber;

    private bool isBusy;

    public GameObject VCam { get { return _vcam; } set { _vcam = value; } }
    public GameObject FocusTarget { get { return _focusTarget; } set { _focusTarget = value; } }
    public GameObject PlayerTarget { get { return _playerTarget; } set { _playerTarget = value; } }
    public GameObject CompanionTarget { get { return _companionTarget; } set { _companionTarget = value; } }   

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
        pageNumber = pages;
        
        _popUp.SetActive(false);
        isInteracting = true;

        Debug.Log("Current pages: " + pages);
        Debug.Log("Page number: " + pageNumber);

        PXController _ctx = _playerCollider.GetComponent<PXController>();
        _ctx.DialogEnter(_playerTarget.transform, _focusTarget.transform, _vcam);
    }

    private void Continue() {
        if (pageNumber <= 1) {
            Exit();
        } else {
            SwitchPage();
        }
    }

    private void SwitchPage() {
        //SwitchLogic
        StartCoroutine(IteratePage());
    }

    private void Exit() {
        isInteracting = false;

        PXController _ctx = _playerCollider.GetComponent<PXController>();
        _ctx.DialogExit();
    }

    private IEnumerator IteratePage() {
        isBusy = true;
        yield return null;

        pageNumber--;
        Debug.Log("Page number: " + pageNumber);
        yield return null;

        isBusy = false;
        yield break;
    }
}
