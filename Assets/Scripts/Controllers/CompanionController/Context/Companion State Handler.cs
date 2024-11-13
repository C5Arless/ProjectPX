using System.Collections.Generic;

public class CompanionStateHandler {
    CompanionController _context;
    Dictionary<CompanionStates, CBaseState> stateList = new Dictionary<CompanionStates, CBaseState>(6); 
        
    public CompanionStateHandler(CompanionController currentContext) {
        _context = currentContext;

        stateList[CompanionStates.stuck] = new CStuckState(_context, this);           //0
        stateList[CompanionStates.operative] = new COperativeState(_context, this);   //1
        stateList[CompanionStates.unstuck] = new CUnstuckState(_context, this);       //2
        stateList[CompanionStates.talk] = new CTalkState(_context, this);             //3
        stateList[CompanionStates.idle] = new CIdleState(_context, this);             //4
        stateList[CompanionStates.move] = new CMoveState(_context, this);             //5
    }

    public CBaseState Stuck() {
        return stateList[CompanionStates.stuck];
    }

    public CBaseState Operative() {
        return stateList[CompanionStates.operative];
    }

    public CBaseState Unstuck() {
        return stateList[CompanionStates.unstuck];
    }

    public CBaseState Talk() {
        return stateList[CompanionStates.talk];
    }

    public CBaseState Idle() {
        return stateList[CompanionStates.idle];
    }

    public CBaseState Move() {
        return stateList[CompanionStates.move];
    }

}