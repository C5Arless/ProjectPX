using System.Collections;
using UnityEngine;

public class CinematicController : MonoBehaviour {
    [SerializeField] CinematicShot[] _cinematicShots;
    [SerializeField] Material _cinematicFrame;

    private PXController _playerCtx;
    private CompanionController _companionCtx;

    private int shots;
    private int shotNumber;

    private bool isInteracting;
    private bool isBusy;    

    private void Awake() {
        shots = _cinematicShots.Length;
    }

    private void Start() {
        _playerCtx = GameBucket.Instance.PXController;
        _companionCtx = GameBucket.Instance.CompanionCtx;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {                        
            OnInteract();
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

        _cinematicShots[shotNumber - 1].Enter();

        StartCoroutine("FrameIn");
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
        
        _playerCtx.InteractionExit();
        _companionCtx.ExitTalkState();

        StartCoroutine("FrameOut");
    }

    private IEnumerator IterateShot() {
        isBusy = true;
        yield return null;

        shotNumber++;
        yield return null;

        _cinematicShots[shotNumber - 1].Enter();        

        isBusy = false;
        yield break;
    }

    private IEnumerator FrameIn() {
        yield return null;

        while (_cinematicFrame.GetFloat("_Transition") < 0.98f) {
            float size = _cinematicFrame.GetFloat("_Transition");
            float lerpSize = size + .05f;
            _cinematicFrame.SetFloat("_Transition", lerpSize);
            yield return null;
        }

        _cinematicFrame.SetFloat("_Transition", 1f);

        yield break;
    }

    private IEnumerator FrameOut() {
        yield return null;

        while (_cinematicFrame.GetFloat("_Transition") > 0.01f) {
            float size = _cinematicFrame.GetFloat("_Transition");
            float lerpSize = size - .05f;
            _cinematicFrame.SetFloat("_Transition", lerpSize);
            yield return null;
        }

        _cinematicFrame.SetFloat("_Transition", 0f);

        yield break;
    }
}

