using System;
using UnityEngine;

[ExecuteAlways]
public class SelfSize : MonoBehaviour {
    public Vector3 selfSize;
    public bool isRunning;

    private void Awake() {
        selfSize = Vector3.one;
        isRunning = false;
    }

    private void Update() {
        if (isRunning) {
            transform.localScale = selfSize;
        }
    }
}
