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

    public bool IsRunning { get { return isRunning; } }

    public List<IOrchestratedEvent> EventsQueue { get { return eventsQueue; } }

    private void Awake() {
        if (GameBucket.Instance.EventsOrchestrator == null) {
            GameBucket.Instance.EventsOrchestrator = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }        
    }

    public void EnqueueEvent(IOrchestratedEvent target) {
        if (DataManager.Instance.HasEventRun(target.EventIndex)) {
            DequeueEvent(target);
        }

        if (!eventsQueue.Contains(target)) {
            Debug.Log(target + " event enqueued");
            eventsQueue.Add(target);
        }

        if (currentTask == null) {            
            StartCoroutine(BeginOrchestration());
        }

        if (currentEvent?.Priority == EventPriority.Low) {
            Debug.Log(currentEvent + " event canceled");
            currentEvent.CancelEvent();
        }
        
    }

    public void DequeueEvent(IOrchestratedEvent target) {
        if (!eventsQueue.Contains(target)) {
            Debug.Log(target + " event removed");

            if (currentEvent == target) {
                target.CancelEvent();
            }

            eventsQueue.Remove(target);
        }
    }

    public void FireAsyncEvent(IOrchestratedEvent _evt) {
        if (DataManager.Instance.HasEventRun(_evt.EventIndex)) { return; } 

        Task asyncEvent = RunAsyncEvent(_evt);
    }

    public void FlushQueue() {
        eventsQueue.Clear();
        currentTask = null;

        if (isRunning) {
            isRunning = false;
        }
    }

    private async Task RunEvents() {
        isRunning = true;
        await Task.Yield();

        while (eventsQueue.Count() > 0) {
            var targetEvent = GetEvent();
            Debug.Log(targetEvent + " retrieved");

            if (targetEvent == null) {
                Debug.Log(targetEvent + " is null?");
                eventsQueue.Clear();
                await Task.Yield();
                return; 
            }
            
            if (targetEvent.IsRepeatable) {
                currentEvent = targetEvent;

                Debug.Log(targetEvent + " fired");
                await targetEvent.FireEvent();
                Debug.Log(targetEvent + " done!");

                eventsQueue.Remove(targetEvent);
            } else {

                if (DataManager.Instance.HasEventRun(targetEvent.EventIndex)) {
                    Debug.Log(targetEvent + " removed");
                    eventsQueue.Remove(targetEvent);
                    targetEvent.DestroyEvent();                    
                } else {
                    currentEvent = targetEvent;

                    Debug.Log(targetEvent + " fired");
                    await targetEvent.FireEvent();
                    Debug.Log(targetEvent + " done!");

                    DataManager.Instance.RegisterEvent(targetEvent.EventIndex);                                
            
                    eventsQueue.Remove(targetEvent);
                }
            }

        }

        isRunning = false;
        currentTask = null;
    }

    private async Task RunAsyncEvent(IOrchestratedEvent _evt) {
        Debug.Log("Event running!");
        await Task.Yield();

        await _evt.FireEvent();

        if (!_evt.IsRepeatable) {
            DataManager.Instance.RegisterEvent(_evt.EventIndex);
        }
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
        yield return null;

        if (currentTask == null) {
            currentTask = RunEvents();
            yield return null;
        }

        yield break;
    } 
}
