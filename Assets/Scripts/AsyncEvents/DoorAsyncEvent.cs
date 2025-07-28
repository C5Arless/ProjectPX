using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DoorAsyncEvent : MonoBehaviour, IOrchestratedEvent {
    [SerializeField] int eventIndex;
    [SerializeField] bool isRepeatable;
    [SerializeField] EventPriority priority;

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

        await Task.Yield();

        isRunning = false;
    }

    void Start() {
        
    }
    
    void Update() {
        
    }
}
