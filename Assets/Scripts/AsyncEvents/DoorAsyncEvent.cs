using System.Threading.Tasks;
using UnityEngine;

public class DoorAsyncEvent : MonoBehaviour, IOrchestratedEvent {
    [SerializeField] GameObject _doorL;
    [SerializeField] GameObject _doorR;
    [Space]
    [SerializeField] int eventIndex;
    [SerializeField] bool isRepeatable;
    [SerializeField] EventPriority priority;

    private const float minScale = .5f;
    private const float maxScale = 2.6f;
    private const float speed = 3.15f;

    private bool isRunning;

    public int EventIndex { get { return eventIndex; } }

    public bool IsRepeatable { get { return isRepeatable; } }

    public EventPriority Priority { get { return priority; } }

    public void CancelEvent() {
        throw new System.NotImplementedException();
    }

    public void DestroyEvent() {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other) {
        if (isRunning) { return; }

        if (other.tag == "Player") {
            GameBucket.Instance.EventsOrchestrator.FireAsyncEvent(this);
        }
    }

    public async Task FireEvent() {
        isRunning = true;

        while (_doorL.transform.localScale.x <= maxScale) {
            Vector3 currentScale = _doorL.transform.localScale;

            currentScale.x += speed * Time.deltaTime;

            _doorL.transform.localScale = currentScale;
            _doorR.transform.localScale = currentScale;

            await Task.Yield();
        }

        Vector3 finalScale = _doorL.transform.localScale;
        finalScale.x = maxScale;

        _doorL.transform.localScale = finalScale;
        _doorR.transform.localScale = finalScale;

        isRunning = false;
    }
}
