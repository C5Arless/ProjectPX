using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EventsOrchestrator : MonoBehaviour {    
    private List<IOrchestratedEvent> eventsQueue = new List<IOrchestratedEvent>();
    private bool isRunning;
    private Task currentTask;
    private IOrchestratedEvent currentEvent;

    public List<IOrchestratedEvent> EventsQueue { get { return eventsQueue; } }

    private void Awake() {
        GameBucket.Instance.EventsOrchestrator = this;
    }

    public void EnqueueEvent(IOrchestratedEvent target) {
        if (!eventsQueue.Contains(target)) {
            Debug.Log(target + " event enqueued");
            eventsQueue.Add(target); 
        }

        if (currentEvent?.Priority == EventPriority.Low) {
            Debug.Log(target + " event canceled");
            currentEvent.CancelEvent();
        }

        if (!isRunning) {
            StartCoroutine(BeginOrchestration());
        }

    }

    private async Task RunEvents() {
        while (eventsQueue.Count() > 0) {
            var targetEvent = GetEvent();
            Debug.Log(targetEvent + " retrieved");

            if (targetEvent == null) {
                eventsQueue.Clear();
                await Task.Yield();
                break; 
            }
            
            if (DataManager.Instance.HasEventRun(targetEvent.EventIndex)) {
                Debug.Log(targetEvent + " removed");
                eventsQueue.Remove(targetEvent);
                continue;
            } else {
                Debug.Log(targetEvent + " fired");
                currentEvent = targetEvent;
                await targetEvent.FireEvent();
            }
            

            if (!targetEvent.IsRepeatable) {
                DataManager.Instance.RegisterEvent(targetEvent.EventIndex);                
            } 
            
            eventsQueue.Remove(targetEvent);            

        }

        isRunning = false;

        currentTask = null;
    }

    private IOrchestratedEvent GetEvent() {
        IOrchestratedEvent target = null;
        EventPriority maxPriority = EventPriority.Low;

        foreach (IOrchestratedEvent evt in eventsQueue) {
            if (evt.Priority == maxPriority && target == null) {
                target = evt;                
            } else if (evt.Priority < maxPriority) {
                maxPriority = evt.Priority;
                target = evt;
            }
        }

        return target;
    }

    private IEnumerator BeginOrchestration() {
        isRunning = true;
        yield return null;

        if (currentTask == null) {
            currentTask = RunEvents();
        }

        yield break;
    } 
}
