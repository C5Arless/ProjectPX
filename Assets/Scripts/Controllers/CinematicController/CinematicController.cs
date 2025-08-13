using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CinematicController : MonoBehaviour, IOrchestratedEvent {
    [SerializeField] CinematicShot[] _cinematicShots;
    [SerializeField] Material _cinematicFrame;
    [SerializeField] bool _isSkippable;
    [SerializeField] bool _destroyWhenDone;
    [Space]
    [SerializeField] int _eventIndex;
    [SerializeField] bool _isRepeatable;
    [SerializeField] EventPriority _eventPriority;

    private CancellationTokenSource token;

    private int shots;
    private int shotNumber;

    private bool isInteracting;
    private bool isBusy;

    public bool IsInteracting { get { return isInteracting; } } 

    public int EventIndex { get { return _eventIndex; } }
    public bool IsRepeatable { get { return _isRepeatable; } }
    public EventPriority Priority { get { return _eventPriority; } }

    private void Awake() {
        shots = _cinematicShots.Length;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            GameBucket.Instance.EventsOrchestrator.EnqueueEvent(this);
        }        
    }

    public async Task FireEvent() {
        token = new CancellationTokenSource();

        OnInteract();

        isInteracting = true;
        
        while (isInteracting) {
            if (token.IsCancellationRequested) {
                SkipCinematic();
                await Task.Yield();
                break;
            }

            await Task.Yield();
        }

        await Task.Yield();
    }

    public void CancelEvent() {
        if (_isSkippable) {
            token.Cancel();            
        }
    }

    public void AbortEvent() {
        if (token != null) {
            token.Cancel();
        }
    }

    public void DestroyEvent() {
        GameBucket.Instance.EventsOrchestrator.DequeueEvent(this);
        Destroy(gameObject);
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

    public void SkipCinematic() {       
        StopAllCoroutines();
        _cinematicFrame.SetFloat("_Transition", 0f);

        isInteracting = false;

        GameBucket.Instance.PXController.InteractionExit();
        GameBucket.Instance.CompanionCtx.ExitTalkState();

        if (_destroyWhenDone) {
            Destroy(gameObject);
        }        
    }

    private void Enter() {
        shotNumber = 1;                

        _cinematicShots[shotNumber - 1].Enter();

        if (_cinematicShots[shotNumber - 1].HasTransition) {
            StartCoroutine("FrameIn");
        }
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
        GameBucket.Instance.PXController.InteractionExit();
        GameBucket.Instance.CompanionCtx.ExitTalkState();
        
        isInteracting = false;

        if (_cinematicShots[shotNumber - 1].HasTransition) {
            StartCoroutine("FrameOut");
        }

        if (_destroyWhenDone) {
            Destroy(gameObject);
        }
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

