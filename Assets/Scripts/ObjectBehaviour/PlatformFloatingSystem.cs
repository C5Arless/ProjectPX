using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PAxisMode {
    Single, Multi
}
public enum PSAxis {
    X, Y, Z
}
public enum PMAxis {
    XY, XZ, YZ, XYZ
}

public class PlatformFloatingSystem : MonoBehaviour {
    public PAxisMode mode;
    public PSAxis s_axis;
    public PMAxis m_axis;

    private Vector3 position;
    private Vector3 center;
    private Vector3 start;
    private Vector3 end;
    private Vector3 shift;

    public bool isActive;
    public bool hasTrigger;

    public float offset;
    public float stopTime;
    public float speed;

    private float startDistance;
    private float endDistance;

    private int dir = 1;

    void Awake() {
        center = this.transform.position;

        if (mode == PAxisMode.Single) {
            InitializeSAxis();
        } else {
            InitializeMAxis();
        }
    }
    void FixedUpdate() {
        if (isActive) {
            GetPosition();
            SetPosition();
        }

    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            TriggerSwitch();
        }
        if (other.tag == "Enemy") {
            TriggerSwitch();
        }
    }
    private void TriggerSwitch() {
        if (!hasTrigger) { return; }

        if (dir == -1) {
            SwitchDir();
        }
    }
    private void SetPosition() {
        if (dir == 1) {
            StartToEnd();
        } else if (dir == -1){
            EndToStart();
        }
    }
    private void GetPosition() {
        position = this.transform.position;
        startDistance = Vector3.Distance(position, start);
        endDistance = Vector3.Distance(position, end);
    }
    private void SwitchDir() {
        if (dir > 0) {
            dir = -1;
        } else {
            dir = 1;
        }
    }
    private void StartToEnd() {
        if (endDistance <= .1f) {
            dir++;
            Invoke("SwitchDir", stopTime);
            return;
        } else {
            Vector3 lerpPosition = Vector3.Lerp(position, end, (speed /100));
            this.transform.position = lerpPosition;
        }
    }
    private void EndToStart() {
        if (startDistance <= .1f) {
            dir--;
            Invoke("SwitchDir", stopTime);
            return;
        }
        else {
            Vector3 lerpPosition = Vector3.Lerp(position, start, (speed /100));
            this.transform.position = lerpPosition;
        }
    }
    private void InitializeSAxis() {
        switch (s_axis) {
            case PSAxis.X: {
                    hasTrigger = false;
                    shift = transform.right;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
            case PSAxis.Y: {
                    hasTrigger = true;
                    shift = transform.up;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
            case PSAxis.Z: {
                    hasTrigger = true;
                    shift = transform.forward;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
        }
    }
    private void InitializeMAxis() {
        switch (m_axis) {
            case PMAxis.XZ: {
                    hasTrigger = false;
                    shift = transform.right + transform.forward;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
            case PMAxis.YZ: {
                    hasTrigger = true;
                    shift = transform.up + transform.forward;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
            case PMAxis.XY: {
                    hasTrigger = true;
                    shift = transform.right + transform.up;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
            case PMAxis.XYZ: {
                    hasTrigger = true;
                    shift = transform.right + transform.up + transform.right;
                    start = center - (shift * offset);
                    end = center + (shift * offset);
                    return;
            }
        }
    }
    public IEnumerator StartCycle() {
        while (dir == 1) {
            GetPosition();
            if (endDistance <= .05f) {
                yield return new WaitForSeconds(stopTime);
                dir = -1;
            } else {
                StartToEnd();
            }
            yield return null;
        }
        while (dir == -1) {
            GetPosition();
            if (startDistance <= .05f) {
                yield return new WaitForSeconds(stopTime);
                dir = 1;
            }
            else {
                EndToStart();
            }
            yield return null;
        }
    }

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(PlatformFloatingSystem)), CanEditMultipleObjects]
    public class PlatformFloatingSystemEditor : Editor {        
        public override void OnInspectorGUI() {
            var editor = (PlatformFloatingSystem)target;
            editor.isActive = EditorGUILayout.Toggle("isActive", editor.isActive);
            editor.mode = (PAxisMode)EditorGUILayout.EnumPopup("Axis Mode", editor.mode);
            if (editor.mode == PAxisMode.Single) {
                editor.s_axis = (PSAxis)EditorGUILayout.EnumPopup("Axis", editor.s_axis);
            } else {
                editor.m_axis = (PMAxis)EditorGUILayout.EnumPopup("Axis", editor.m_axis);
            }

            editor.offset = EditorGUILayout.Slider("Offset", editor.offset, 0, 30); 
            editor.stopTime = EditorGUILayout.Slider("Stop Time", editor.stopTime, 0f, 10f);
            editor.speed = EditorGUILayout.Slider("Speed", editor.speed, 0, 10);
            
        }
    }
#endif
    #endregion
}
