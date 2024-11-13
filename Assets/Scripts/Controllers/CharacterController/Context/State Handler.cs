using System.Collections.Generic;

public class StateHandler {
    PXController _context;
    AnimHandler _animHandler;
    Dictionary<States, BaseState> stateList = new Dictionary<States, BaseState>(10); 

    public StateHandler(PXController currentContext, AnimHandler animHandler) {
        _context = currentContext;
        _animHandler = animHandler;

        stateList[States.dead] = new DeadState(_context, this, _animHandler);             //0
        stateList[States.grounded] = new GroundedState(_context, this, _animHandler);     //1
        stateList[States.airborne] = new AirborneState(_context, this, _animHandler);     //2
        stateList[States.damage] = new DamageState(_context, this, _animHandler);         //3
        stateList[States.idle] = new IdleState(_context, this, _animHandler);             //4
        stateList[States.walk] = new WalkState(_context, this, _animHandler);             //5
        stateList[States.attack] = new AttackState(_context, this, _animHandler);         //6
        stateList[States.jump] = new JumpState(_context, this, _animHandler);             //7
        stateList[States.dash] = new DashState(_context, this, _animHandler);             //8
        stateList[States.fall] = new FallState(_context, this, _animHandler);             //9
    }

    public BaseState Dead() {
        return stateList[States.dead];
    }
    public BaseState Grounded() {
        return stateList[States.grounded];
    }
    public BaseState Airborne() {
        return stateList[States.airborne];
    }
    public BaseState Damage() {
        return stateList[States.damage];
    }
    public BaseState Idle() {
        return stateList[States.idle];
    }
    public BaseState Walk() {
        return stateList[States.walk];
    }
    public BaseState Attack() {
        return stateList[States.attack];
    }
    public BaseState Jump() {
        return stateList[States.jump];
    }
    public BaseState Dash() {
        return stateList[States.dash];
    }
    public BaseState Fall() {
        return stateList[States.fall];
    }
}