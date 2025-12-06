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
    private Task currentEventTask;

    public bool IsRunning { get { return isRunning; } }
    public Task CurrentEventTask { get { return currentEventTask; } }
    public IOrchestratedEvent CurrentEvent { get { return currentEvent; } }
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
        } else {

            if (!eventsQueue.Contains(target)) {
                eventsQueue.Add(target);
            }

            if (currentEvent?.Priority == EventPriority.Low) {
                currentEvent.CancelEvent();
            }
            
            if (currentTask == null) {            
                StartCoroutine(BeginOrchestration());
            }
        }
    }

    public void DequeueEvent(IOrchestratedEvent target) {
        if (eventsQueue.Contains(target)) {
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

            if (targetEvent == null) {
                eventsQueue.Clear();
                await Task.Yield();
                return; 
            }
            
            if (targetEvent.IsRepeatable) {
                currentEvent = targetEvent;
                currentEventTask = currentEvent.FireEvent();

                if (currentEvent.Priority < EventPriority.Low) {
                    GameMaster.Instance.CutscenePause();
                    GameBucket.Instance.GameCanvasHandler.HideUI();

                    await currentEventTask;
                    GameMaster.Instance.CutsceneUnpause();
                }
                else {
                    GameBucket.Instance.GameCanvasHandler.ShowCameraLocked();
                    if (GameBucket.Instance.GameCanvasHandler.IsHidden) {
                        GameBucket.Instance.GameCanvasHandler.ShowUI();
                    }

                    await currentEventTask;
                    
                    GameBucket.Instance.GameCanvasHandler.HideCameraLocked();
                }
                
                await Task.Yield();                

                eventsQueue.Remove(currentEvent);

            } else {

                if (DataManager.Instance.HasEventRun(targetEvent.EventIndex)) {
                    eventsQueue.Remove(targetEvent);
                    targetEvent.DestroyEvent();                    
                } else {
                    currentEvent = targetEvent;
                    currentEventTask = currentEvent.FireEvent();
                    
                    if (currentEvent.Priority < EventPriority.Low) {
                        GameMaster.Instance.CutscenePause();
                        GameBucket.Instance.GameCanvasHandler.HideUI();

                        await currentEventTask;
                        GameMaster.Instance.CutsceneUnpause();
                    }
                    else {
                        GameBucket.Instance.GameCanvasHandler.ShowCameraLocked();
                        if (GameBucket.Instance.GameCanvasHandler.IsHidden) {
                            GameBucket.Instance.GameCanvasHandler.ShowUI();
                        }
                        
                        await currentEventTask;
                        
                        GameBucket.Instance.GameCanvasHandler.HideCameraLocked();
                    }
                    
                    await Task.Yield();                    

                    DataManager.Instance.RegisterEvent(currentEvent.EventIndex);

                    eventsQueue.Remove(currentEvent);                    
                }
            }

            currentEventTask = null;
            //WhileEnd
        }
        
        GameMaster.Instance.CutsceneUnpause();
        
        if (GameBucket.Instance.GameCanvasHandler.IsHidden) {
            GameBucket.Instance.GameCanvasHandler.ShowUI();
        }
        
        isRunning = false;
        currentTask = null;
    }

    public async Task<Task> GetCurrentEventTask() {
        if (!isRunning || currentEventTask == null) {
            await Task.Delay(3000);
            return Task.CompletedTask;

        } else { 
            var taskToWait = currentEventTask;
            return taskToWait;
        }

    }

    private async Task RunAsyncEvent(IOrchestratedEvent _evt) {
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
