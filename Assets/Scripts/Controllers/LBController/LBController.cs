using UnityEngine;

public class LBController : MonoBehaviour {
    //State reference
    LBBaseState _currentRootState;
    LBBaseState _currentSubState;

    public LBBaseState CurrentRootState { get { return _currentRootState; } set { _currentRootState = value; } }
    public LBBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
}