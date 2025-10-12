using System.Collections.Generic;

public class LBStateHandler {
    LBController _context;
    Dictionary<LBRootStates, LBBaseState> rootStateList = new Dictionary<LBRootStates, LBBaseState>(3);
    Dictionary<LBSubStates, LBBaseState> subStateList = new Dictionary<LBSubStates, LBBaseState>(5);

    public LBStateHandler(LBController currentContext) {
        _context = currentContext;

        rootStateList[LBRootStates.Bug] = new LBBugState(_context, this);
        rootStateList[LBRootStates.Ball] = new LBBallState(_context, this);
        rootStateList[LBRootStates.Dead] = new LBDeadState(_context, this);
        
        subStateList[LBSubStates.Idle] = new LBIdleState(_context, this);
        subStateList[LBSubStates.Patrol] = new LBPatrolState(_context, this);
        subStateList[LBSubStates.Pursue] = new LBPursueState(_context, this);
        subStateList[LBSubStates.Damaged] = new LBPursueState(_context, this);
        subStateList[LBSubStates.Attack] = new LBAttackState(_context, this);        
    }

    public LBBaseState Bug() {
        return rootStateList[LBRootStates.Bug];
    }

    public LBBaseState Ball() {
        return rootStateList[LBRootStates.Ball];
    }

    public LBBaseState Dead() {
        return rootStateList[LBRootStates.Dead];
    }

    public LBBaseState Idle() {
        return subStateList[LBSubStates.Idle];
    }

    public LBBaseState Patrol() {
        return subStateList[LBSubStates.Patrol];
    }

    public LBBaseState Pursue() {
        return subStateList[LBSubStates.Pursue];
    }
    
    public LBBaseState Damaged() {
        return subStateList[LBSubStates.Damaged];
    }

    public LBBaseState Attack() {
        return subStateList[LBSubStates.Attack];
    }

}