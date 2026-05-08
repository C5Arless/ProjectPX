using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class PX3Controller : MonoBehaviour {
    [SerializeField] Rigidbody rb;
    
    PX3BaseState currentRootState;
    PX3BaseState currentSubState;
    
    private Dictionary<PX3RootStates, bool> rootStates; 
    private Dictionary<PX3SubStates, bool> subStates;
    
    #region GetSet

    public PX3BaseState CurrentRootState { get { return currentRootState; } set { currentRootState = value; } }
    public PX3BaseState CurrentSubState { get { return currentSubState; } set { currentRootState = value; } }

    #endregion

 }
