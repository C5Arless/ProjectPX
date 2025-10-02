using System.Collections.Generic;

public class LBStateHandler {
    LBController _context;
    Dictionary<LBStates, LBBaseState> stateList = new Dictionary<LBStates, LBBaseState>(8);

    public LBStateHandler(LBController currentContext) {
        _context = currentContext;

        stateList[LBStates.bug] = new LBBugState(_context, this);             //0
        stateList[LBStates.ball] = new LBBallState(_context, this);           //1
        stateList[LBStates.dead] = new LBDeadState(_context, this);           //2
        stateList[LBStates.idle] = new LBIdleState(_context, this);           //3
        stateList[LBStates.patrol] = new LBPatrolState(_context, this);       //4
        stateList[LBStates.pursue] = new LBPursueState(_context, this);       //5
        stateList[LBStates.damaged] = new LBPursueState(_context, this);      //6
        stateList[LBStates.attack] = new LBAttackState(_context, this);       //7        
    }

    public LBBaseState Bug() {
        return stateList[LBStates.bug];
    }

    public LBBaseState Ball() {
        return stateList[LBStates.ball];
    }

    public LBBaseState Dead() {
        return stateList[LBStates.dead];
    }

    public LBBaseState Idle() {
        return stateList[LBStates.idle];
    }

    public LBBaseState Patrol() {
        return stateList[LBStates.patrol];
    }

    public LBBaseState Pursue() {
        return stateList[LBStates.pursue];
    }

    public LBBaseState Attack() {
        return stateList[LBStates.attack];
    }

}