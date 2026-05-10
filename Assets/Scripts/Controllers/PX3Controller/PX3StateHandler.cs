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
    
    Dictionary<PX3RootStates, bool> rootStates = new Dictionary<PX3RootStates, bool>(4); 
    Dictionary<PX3SubStates, bool> subStates = new Dictionary<PX3SubStates, bool>(13);
    
    public Dictionary<PX3RootStates, bool> RootStates { get { return rootStates; } }
    public Dictionary<PX3SubStates, bool> SubStates { get { return subStates; } }
    public PX3BaseState CurrentRootState { get { return currentRootState; } set { currentRootState = value; } }
    public PX3BaseState CurrentSubState { get { return currentSubState; } set { currentSubState = value; } }
    
    public PX3StateHandler(PX3Controller ctx, PX3AnimHandler animHandler, 
        PX3InputHandler inputHandler, PX3SensorHandler sensorHandler) {
        _ctx = ctx;
        _animHandler = animHandler;
        _inputHandler = inputHandler;
        _sensorHandler = sensorHandler;

        InitializeStateMask();
        InitializeStates();
    }

    private void InitializeStates() {
        rootStateList[PX3RootStates.Dead] = new PX3DeadState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        rootStateList[PX3RootStates.Grounded] = new PX3GroundedState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        rootStateList[PX3RootStates.Airborne] = new PX3AirborneState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        rootStateList[PX3RootStates.Holding] = new PX3HoldingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        
        subStateList[PX3SubStates.Attacking] = new PX3AttackingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Dashing] = new PX3DashingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Diving] = new PX3DivingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Falling] = new PX3FallingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Damaged] = new PX3DamagedState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Idle] = new PX3IdleState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Jumping] = new PX3JumpingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Walking] = new PX3WalkingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Running] = new PX3RunningState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Sprinting] = new PX3SprintingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Grabbing] = new PX3GrabbingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.WallSliding] = new PX3WallSlidingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
        subStateList[PX3SubStates.Thumbling] = new PX3ThumblingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
    }

    private void InitializeStateMask() {
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
}
