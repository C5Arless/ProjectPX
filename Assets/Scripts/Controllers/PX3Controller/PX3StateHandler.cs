using System.Collections.Generic;

public class PX3StateHandler {
    PX3Controller _ctx;
    PX3AnimHandler _animHandler;
    PX3InputHandler _inputHandler;
    PX3SensorHandler _sensorHandler;
    
    PX3BaseState currentRootState;
    PX3BaseState currentSubState;
    
    
    Dictionary<PX3RootStates, PX3BaseState> rootStateList = new Dictionary<PX3RootStates, PX3BaseState>(4); 
    Dictionary<PX3SubStates, PX3BaseState> subStateList = new Dictionary<PX3SubStates, PX3BaseState>(13);
    
    Dictionary<PX3RootStates, bool> rootMask = new Dictionary<PX3RootStates, bool>(4); 
    Dictionary<PX3SubStates, bool> subMask = new Dictionary<PX3SubStates, bool>(13);
    
    public Dictionary<PX3RootStates, bool> rootStates = new Dictionary<PX3RootStates, bool>(4); 
    public Dictionary<PX3SubStates, bool> subStates = new Dictionary<PX3SubStates, bool>(13);
    
    public PX3BaseState CurrentRootState { get { return currentRootState; } }
    public PX3BaseState CurrentSubState { get { return currentSubState; } }
    
    public PX3StateHandler(PX3Controller ctx, PX3AnimHandler animHandler, 
        PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) {
        _ctx = ctx;
        _animHandler = animHandler;
        _inputHandler = inputHandler;
        _sensorHandler = sensorHandler;

        InitializeStateMask();

        //fill state lists
    }

    public void InitializeStates() {
        
    }

    public void InitializeStateMask() {
        rootMask.Add(PX3RootStates.Dead, false);
        rootMask.Add(PX3RootStates.Grounded, false);
        rootMask.Add(PX3RootStates.Airborne, false);
        rootMask.Add(PX3RootStates.Holding, false);

        subMask.Add(PX3SubStates.Attacking, false);
        subMask.Add(PX3SubStates.Dashing, false);
        subMask.Add(PX3SubStates.Diving, false);
        subMask.Add(PX3SubStates.Falling, false);
        subMask.Add(PX3SubStates.Damaged, false);
        subMask.Add(PX3SubStates.Idle, false);
        subMask.Add(PX3SubStates.Jumping, false);
        subMask.Add(PX3SubStates.Walking, false);
        subMask.Add(PX3SubStates.Running, false);
        subMask.Add(PX3SubStates.Sprinting, false);
        subMask.Add(PX3SubStates.Grabbing, false);
        subMask.Add(PX3SubStates.WallSliding, false);
        subMask.Add(PX3SubStates.Thumbling, false);
    }
    
    public void SetSubState(PX3SubStates state) {
        Dictionary<PX3SubStates, bool> target = new Dictionary<PX3SubStates, bool>(13);
        target = subMask;
        
        foreach (KeyValuePair<PX3SubStates, bool> subState in subStates) {
            if (subState.Value) {
                target[subState.Key] = false;
            }
        }
        
        target[state] = true;
        
        subStates = target;
    }

    public void SetRootState(PX3RootStates state) {
        Dictionary<PX3RootStates, bool> target = new Dictionary<PX3RootStates, bool>(4);
        target = rootMask;
        
        foreach (KeyValuePair<PX3RootStates, bool> rootState in rootStates) {
            if (rootState.Value) {
                target[rootState.Key] = false;
            }
        }
        
        target[state] = true;
        
        rootStates = target;
    }
    
    //StateFactory
}
