using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LBAnim {
    idle,
    patrol,
    pursue,
}

public class LBAnimHandler : MonoBehaviour {
    [SerializeField] LBController _ctx;
    [SerializeField] Animator _animator;
    
    [SerializeField] MeshVisibility _ball;
    [SerializeField] MeshVisibility _body;
    
    Dictionary<LBAnim, Vector2> animList = new Dictionary<LBAnim, Vector2>(3);
    Vector2 targetclip;

    public LBAnimHandler() {
        animList[LBAnim.idle] = new Vector2(0f, 0f);
        animList[LBAnim.patrol] = new Vector2(-.99f, 0f);
        animList[LBAnim.pursue] = new Vector2(.99f, 0f);
    }

    public void PlayDirect(Vector2 clip) {
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isMorphing", false);
        _animator.SetBool("isUnmorphing", false);
        _animator.SetBool("isSpinning", false);
        
        targetclip = clip;
        
        _animator.SetFloat("xAxis", targetclip.x);
        _animator.SetFloat("yAxis", targetclip.y);

        _animator.speed = 1f;
    }
    
    public void ResetAnimator() {
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isMorphing", false);
        _animator.SetBool("isUnmorphing", false);
        _animator.SetBool("isSpinning", false);
        
        _animator.SetFloat("xAxis", targetclip.x);
        _animator.SetFloat("yAxis", targetclip.y);
        _animator.Play("AnimationsTree", 0, 0);
        _animator.speed = 0f;
    }
    
    public void PlayAttack() {
        _animator.SetFloat("xAxis", 0f);
        _animator.SetFloat("yAxis", 0f);
        
        _animator.SetBool("isMorphing", false);
        _animator.SetBool("isUnmorphing", false);
        _animator.SetBool("isSpinning", false);
        
        _animator.SetBool("isAttacking", true);
        
        _animator.speed = 1f;
    }
    
    public void PlayMorph() {
        _animator.SetFloat("xAxis", 0f);
        _animator.SetFloat("yAxis", 0f);
        
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isSpinning", false);
        _animator.SetBool("isUnmorphing", false);
        
        _animator.SetBool("isMorphing", true);
        
        _animator.speed = 1f;
    }
    
    public void PlayUnmorph() {
        _animator.SetFloat("xAxis", 0f);
        _animator.SetFloat("yAxis", 0f);
        
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isMorphing", false);
        _animator.SetBool("isSpinning", false);
        
        _animator.SetBool("isUnmorphing", true);
        
        _animator.speed = 1f;
    }

    public void PlaySpin() {
        _animator.SetFloat("xAxis", 0f);
        _animator.SetFloat("yAxis", 0f);
        
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isMorphing", false);
        _animator.SetBool("isUnmorphing", false);
        
        _animator.SetBool("isSpinning", true);
        
        _animator.speed = 1f;
    }

    public void StopSpin() {
        _animator.SetBool("isSpinning", false);
    }

    public void StopAttack() {
        _animator.SetBool("isAttacking", false);
    }

    public void StopMorph() {
        _animator.SetBool("isMorphing", false);
    }

    public void StopUnmorph() {
        _animator.SetBool("isUnmorphing", false);
    }

    public void Stop() {
        _animator.speed = 0f;
    }

    public void Resume() {
        _animator.speed = 1f;
    }
    
    public void BallVisibility(int state) {
        if (state != 0) {
            _ball.Show();
        } else _ball.Hide();
    }
    
    public void BodyVisibility(int state) {
        if (state != 0) {
            _body.Show();
        } else _body.Hide();
    }
    
    public void SendSignalToCtx(int _sig) {
        _ctx.HandleSignal(_sig);
    }
    
    public Vector2 Idle() {
        return animList[LBAnim.idle];
    }
    public Vector2 Patrol() {
        return animList[LBAnim.patrol];
    }
    public Vector2 Pursue() {
        return animList[LBAnim.pursue];
    }
}
