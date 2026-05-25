using System.Collections.Generic;

public class PX3StateHandler {
    PX3Controller _ctx;
    PX3AnimHandler _animHandler;
    PX3InputHandler _inputHandler;
    PX3SensorHandler _sensorHandler;
    
    PX3BaseState currentRootState;
    PX3BaseState currentSubState;
    
    Dictionary<PX3RootStates, PX3BaseState> rootStateList = new Dictionary<PX3RootStates, PX3BaseState>(4); 
    Dictionary<PX3SubStates, PX3BaseState> subStateList = new Dictionary<PX3SubStates, PX3BaseState>(14);
    
    Dictionary<PX3RootStates, bool> rootMasks = new Dictionary<PX3RootStates, bool>(4); 
    Dictionary<PX3SubStates, bool> subMasks = new Dictionary<PX3SubStates, bool>(14);
    
    Dictionary<PX3RootStates, bool> rootStates = new Dictionary<PX3RootStates, bool>(4); 
    Dictionary<PX3SubStates, bool> subStates = new Dictionary<PX3SubStates, bool>(14);
    
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
        InitializeConcreteStates();
    }

    public void Initialize() {
        SetRootState(PX3RootStates.Grounded);
        currentRootState = rootStateList[PX3RootStates.Grounded];
        currentRootState.EnterState();
        
        SetSubState(PX3SubStates.Idle);
        currentSubState = subStateList[PX3SubStates.Idle];
        currentSubState.EnterState();
    }

    public void SendSignal(int sig) {
        currentSubState.HandleSignal(sig);
    }

    private void InitializeStateMask() {
        rootMasks.Add(PX3RootStates.Dead, false);
        rootMasks.Add(PX3RootStates.Grounded, false);
        rootMasks.Add(PX3RootStates.Airborne, false);
        rootMasks.Add(PX3RootStates.Holding, false);

        subMasks.Add(PX3SubStates.Attacking, false);
        subMasks.Add(PX3SubStates.Dashing, false);
        subMasks.Add(PX3SubStates.Diving, false);
        subMasks.Add(PX3SubStates.Falling, false);
        subMasks.Add(PX3SubStates.Damaged, false);
        subMasks.Add(PX3SubStates.Idle, false);
        subMasks.Add(PX3SubStates.Jumping, false);
        subMasks.Add(PX3SubStates.Walking, false);
        subMasks.Add(PX3SubStates.Running, false);
        subMasks.Add(PX3SubStates.Sprinting, false);
        subMasks.Add(PX3SubStates.Grabbing, false);
        subMasks.Add(PX3SubStates.WallSliding, false);
        subMasks.Add(PX3SubStates.Thumbling, false);
        subMasks.Add(PX3SubStates.Crouching, false);
    }
    private void InitializeStates() {
        foreach (var root in rootMasks) {
            rootStates.Add(root.Key, root.Value);
        }
        
        foreach (var sub in subMasks) {
            subStates.Add(sub.Key, sub.Value);
        }
    }
    private void InitializeConcreteStates() {
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
        subStateList[PX3SubStates.Crouching] = new PX3CrouchingState(_ctx, this, _animHandler, _inputHandler, _sensorHandler);
    }
    
    public PX3BaseState GetState(PX3SubStates state) {
        return subStateList[state];
    }
    
    public PX3BaseState GetState(PX3RootStates state) {
        return rootStateList[state];
    }
    
    public void SetSubState(PX3SubStates state) {
        foreach (var subMask in subMasks) {
            subStates[subMask.Key] = subMask.Value;
        }
        
        subStates[state] = true;
    }

    public void SetRootState(PX3RootStates state) {
        foreach (var rootMask in rootMasks) {
            rootStates[rootMask.Key] = rootMask.Value;
        }
        
        rootStates[state] = true;
    }
}
