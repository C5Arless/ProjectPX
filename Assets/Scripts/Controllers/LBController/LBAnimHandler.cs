using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LBAnim {
    dead,
    idle,
    patrol,
    pursue,
    attack1,
    attack2,
    morph,
    unmorph,
    damage
}

public class LBAnimHandler : MonoBehaviour {
    [SerializeField] LBController _ctx;
    [SerializeField] Animator _animator;
    
    [SerializeField] MeshVisibility _ball;
    [SerializeField] MeshVisibility _body;
    
    Vector2 currentclip;
    Vector2 targetclip;    

    Dictionary<LBAnim, Vector2> animList = new Dictionary<LBAnim, Vector2>(9);
    public LBAnimHandler() {
        animList[LBAnim.dead] = new Vector2(.99f, .99f);                    //0
        animList[LBAnim.idle] = new Vector2(0f, 0f);                        //1
        animList[LBAnim.patrol] = new Vector2(0f, .99f);                    //2
        animList[LBAnim.pursue] = new Vector2(0f, .99f);                    //3
        animList[LBAnim.attack1] = new Vector2(-.99f, 0f);                  //4
        animList[LBAnim.attack2] = new Vector2(0f, -.99f);                  //5
        animList[LBAnim.morph] = new Vector2(-.99f, .99f);                  //6
        animList[LBAnim.unmorph] = new Vector2(-.99f, -.99f);               //7
        animList[LBAnim.damage] = new Vector2(.99f, -.99f);                 //8
    }

    public Vector2 CurrentClip { get { return currentclip; } }
    public Vector2 TargetClip { get { return targetclip; } }

    public void Play(Vector2 clip) {
        if (clip != currentclip) {
            StopCoroutine(LoadClip());
            targetclip = clip;
            StartCoroutine(LoadClip());        
        }
    }

    public void PlayDirect(Vector2 clip) {
        targetclip = clip;

        //Reset Time Logic
        ResetBlendTime();

        _animator.SetFloat("xAxis", targetclip.x);
        _animator.SetFloat("yAxis", targetclip.y);
        currentclip = targetclip;
        
        //_animator.StartPlayback();
    }

    public void Stop() {
        _animator.StopPlayback();
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

    private void SetAlt(bool alt) {
        _animator.SetBool("onAlt", alt);
    }

    private void ResetBlendTime() {
        StartCoroutine(ResetBlend());
    }

    IEnumerator ResetBlend() {
        SetAlt(true);
        yield return null;

        SetAlt(false);
        yield break;
    }

    IEnumerator LoadClip() {

        //Reset Time Logic
        ResetBlendTime();

        while (currentclip - targetclip != Vector2.zero) {
            Vector2 lerpvalue = Vector2.Lerp(currentclip, targetclip, .21f);
            Mathf.Clamp(lerpvalue.x, -.99f, .99f);
            Mathf.Clamp(lerpvalue.y, -.99f, .99f);
            _animator.SetFloat("xAxis", lerpvalue.x);
            _animator.SetFloat("yAxis", lerpvalue.y);

            currentclip = lerpvalue;
            yield return null;
        }

        yield break;
    }

    public Vector2 Dead() {
        return animList[LBAnim.dead];
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
    public Vector2 Attack1() {
        return animList[LBAnim.attack1];
    }
    public Vector2 Attack2() {
        return animList[LBAnim.attack2];
    }
    public Vector2 Morph() {
        return animList[LBAnim.morph];
    }
    public Vector2 Unmorph() {
        return animList[LBAnim.unmorph];
    }
    public Vector2 Damage() {
        return animList[LBAnim.damage];
    }
}
