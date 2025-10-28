using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LBAnim {
    spin,
    FASTER,
    idle,
    patrol,
    pursue,
    attack,
    shot,
    morph,
    unmorph,
}

public class LBAnimHandler : MonoBehaviour {
    [SerializeField] LBController _ctx;
    [SerializeField] Animator _animator;
    
    [SerializeField] MeshVisibility _ball;
    [SerializeField] MeshVisibility _body;
    
    Dictionary<LBAnim, Vector2> animList = new Dictionary<LBAnim, Vector2>(9);
    Vector2 targetclip;

    public LBAnimHandler() {
        animList[LBAnim.spin] = new Vector2(0f, .99f);                      //0 OK
        animList[LBAnim.FASTER] = new Vector2(.99f, .99f);                  //1 OK
        animList[LBAnim.idle] = new Vector2(0f, 0f);                        //2 OK
        animList[LBAnim.patrol] = new Vector2(-.99f, 0f);                   //3 OK
        animList[LBAnim.pursue] = new Vector2(.99f, 0f);                    //4 OK
        animList[LBAnim.attack] = new Vector2(0f, -.99f);                   //5 OK
        animList[LBAnim.shot] = new Vector2(.99f, -.99f);                   //6
        animList[LBAnim.morph] = new Vector2(-.99f, .99f);                  //7 OK
        animList[LBAnim.unmorph] = new Vector2(-.99f, -.99f);               //8 OK
    }

    public void PlayDirect(Vector2 clip) {
        targetclip = clip;
        
        RestartCurrentState();
        
        _animator.SetFloat("xAxis", targetclip.x);
        _animator.SetFloat("yAxis", targetclip.y);
    }

    public void Stop() {
        _animator.speed = 0f;
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
    
    private void RestartCurrentState() {
        var st = _animator.GetCurrentAnimatorStateInfo(0);
        _animator.Play(st.shortNameHash, 0, 0f);
        _animator.Update(0f);
        _animator.speed = 1f;
    }

    public Vector2 Spin() {
        return animList[LBAnim.spin];
    }
    public Vector2 FASTER() {
        return animList[LBAnim.FASTER];
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
    public Vector2 Attack() {
        return animList[LBAnim.attack];
    }
    public Vector2 Shot() {
        return animList[LBAnim.shot];
    }
    public Vector2 Morph() {
        return animList[LBAnim.morph];
    }
    public Vector2 Unmorph() {
        return animList[LBAnim.unmorph];
    }
}
