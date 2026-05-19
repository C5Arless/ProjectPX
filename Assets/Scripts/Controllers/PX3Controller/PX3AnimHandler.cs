using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3AnimHandler : MonoBehaviour {
    PX3Controller _ctx;
    PX3StateHandler _stateHandler;
    Animator _animator;

    private void Awake() { 
        _animator = GetComponent<Animator>();
    }

    public void Initialize(PX3Controller ctx, PX3StateHandler stateHandler) {
        _ctx = ctx;
        _stateHandler = stateHandler;
    }
    
    
}

