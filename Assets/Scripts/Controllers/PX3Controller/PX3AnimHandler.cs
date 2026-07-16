using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PX3AnimHandler : MonoBehaviour {
    PX3Controller _ctx;
    PX3StateHandler _stateHandler;
    Animator _animator;

    PX3A_RootSet currentRootset;
    private int currentSubset;

    public void Initialize(PX3Controller ctx, PX3StateHandler stateHandler) {
        _ctx = ctx;
        _stateHandler = stateHandler;
        _animator = GetComponent<Animator>();
    }

    public void SetIRCBlend(float blend) {
        _animator.SetFloat("IRCBlend", blend);
    }

    public void Stop() {
        _animator.speed = 0f;
    }

    public void Resume() {
        _animator.speed = 1f;
    }
    
    public void SignalToState(int sig) {
        _stateHandler.SendSignal(sig);
    }

    public void SignalToCtx(int sig) {
        _ctx.HandleSignal(sig);
    }

    public void PlayClip(PX3A_GroundSet target) {
        if (currentRootset == PX3A_RootSet.Ground &&
            currentSubset == (int)target) return; 
        
        Stop();
        
        SetSubset(target);
        
        Resume();
    }
    
    public void PlayClip(PX3A_AirSet target) {
        if (currentRootset == PX3A_RootSet.Air &&
            currentSubset == (int)target) return; 
        
        Stop();
        
        SetSubset(target);
        
        Resume();
    }
    
    public void PlayClip(PX3A_MixedSet target) {
        if (currentRootset == PX3A_RootSet.Mixed &&
            currentSubset == (int)target) return; 
        
        Stop();
        
        SetSubset(target);
        
        Resume();
    }

    public void PlayDeath() {
        Stop();
        
        SetRootset(PX3A_RootSet.Death);
        
        Resume();
    }
    
    private void SetRootset(PX3A_RootSet target) {
        currentRootset = target;
        _animator.SetInteger("RootSet", (int)currentRootset);
    }

    private void SetSubset(PX3A_GroundSet target) {
        if (currentRootset != PX3A_RootSet.Ground) {
            SetRootset(PX3A_RootSet.Ground);
        }
        
        currentSubset = (int)target;
        _animator.SetInteger("GroundSet", currentSubset);
    }
    
    private void SetSubset(PX3A_MixedSet target) {
        if (currentRootset != PX3A_RootSet.Mixed) {
            SetRootset(PX3A_RootSet.Mixed);
        }
        
        currentSubset = (int)target;
        _animator.SetInteger("MixSet", currentSubset);
    }

    private void SetSubset(PX3A_AirSet target) {
        if (currentRootset != PX3A_RootSet.Air) {
            SetRootset(PX3A_RootSet.Air);
        }
        
        currentSubset = (int)target;
        _animator.SetInteger("AirSet", currentSubset);
    }
}

