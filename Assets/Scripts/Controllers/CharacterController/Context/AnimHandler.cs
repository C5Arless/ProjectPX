using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Anim {
    dead,
    idle,
    walk,
    attack1,
    jump,
    fall,
    dash,
    damage
}

public class AnimHandler : MonoBehaviour {
    Animator _animator;
    Vector2 currentclip;
    Vector2 targetclip;    

    Dictionary<Anim, Vector2> animList = new Dictionary<Anim, Vector2>(10);
    public AnimHandler() {
        animList[Anim.dead] = new Vector2(.99f, .99f);                    //0
        animList[Anim.idle] = new Vector2(0f, 0f);                        //1
        animList[Anim.walk] = new Vector2(0f, .99f);                      //2
        animList[Anim.attack1] = new Vector2(-.99f, 0f);                  //3
        animList[Anim.jump] = new Vector2(0f, -.99f);                     //4
        animList[Anim.fall] = new Vector2(-.99f, -.99f);                  //5
        animList[Anim.dash] = new Vector2(.99f, 0f);                      //6
        animList[Anim.damage] = new Vector2(.99f, -.99f);                 //7
    }

    public Vector2 CurrentClip { get { return currentclip; } }
    public Vector2 TargetClip { get { return targetclip; } }

    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void Play(Vector2 clip) {
        if (clip != currentclip) {
            targetclip = clip;
            StartCoroutine(LoadClip());                   
        }
    }
    public void PlayDirect(Vector2 clip) {
        targetclip = clip;
        
        _animator.SetFloat("xAxis", targetclip.x);
        _animator.SetFloat("yAxis", targetclip.y);
        currentclip = targetclip;
    }
    public void SetAlt(bool alt) {
        _animator.SetBool("onAlt", alt);
    }
    IEnumerator LoadClip() {
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
        return animList[Anim.dead];
    }
    public Vector2 Damage() {
        return animList[Anim.damage];
    }
    public Vector2 Idle() {
        return animList[Anim.idle];
    }
    public Vector2 Walk() {
        return animList[Anim.walk];
    }
    public Vector2 Attack1() {
        return animList[Anim.attack1];
    }
    public Vector2 Jump() {
        return animList[Anim.jump];
    }
    public Vector2 Dash() {
        return animList[Anim.dash];
    }
    public Vector2 Fall() {
        return animList[Anim.fall];
    }
}
