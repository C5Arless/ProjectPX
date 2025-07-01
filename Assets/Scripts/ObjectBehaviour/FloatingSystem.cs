using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AxisMode {
    Single, Multi
}
public enum SAxis {
    X, Y, Z
}
public enum MAxis {
    XY, XZ, YZ, XYZ
}

public class FloatingSystem : MonoBehaviour {
    public AxisMode mode;
    public SAxis s_axis;
    public MAxis m_axis;

    private Vector3 position;
    private Vector3 center;
    private Vector3 start;
    private Vector3 end;
    private Vector3 shift;

    public bool isActive;
    public float offset;
    public float stopTime;
    public float speed;

    private float startDistance;
    private float endDistance;

    private int dir = 1;

    void Awake() {
        center = this.transform.position;

        if (mode == AxisMode.Single) {
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
            case SAxis.X: {
                shift = transform.right;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
            case SAxis.Y: {
                shift = transform.up;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
            case SAxis.Z: {
                shift = transform.forward;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
        }
    }
    private void InitializeMAxis() {
        switch (m_axis) {
            case MAxis.XZ: {
                shift = transform.right + transform.forward;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
            case MAxis.YZ: {
                shift = transform.up + transform.forward;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
            case MAxis.XY: {
                shift = transform.right + transform.up;
                start = center - (shift * offset);
                end = center + (shift * offset);
                return;
            }
            case MAxis.XYZ: {
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
    [CustomEditor(typeof(FloatingSystem)), CanEditMultipleObjects]
    public class FloatingSystemEditor : Editor {        
        public override void OnInspectorGUI() {
            var editor = (FloatingSystem)target;
            editor.isActive = EditorGUILayout.Toggle("isActive", editor.isActive);
            editor.mode = (AxisMode)EditorGUILayout.EnumPopup("Axis Mode", editor.mode);
            if (editor.mode == AxisMode.Single) {
                editor.s_axis = (SAxis)EditorGUILayout.EnumPopup("Axis", editor.s_axis);
            } else {
                editor.m_axis = (MAxis)EditorGUILayout.EnumPopup("Axis", editor.m_axis);
            }

            editor.offset = EditorGUILayout.Slider("Offset", editor.offset, 0, 30); 
            editor.stopTime = EditorGUILayout.Slider("Stop Time", editor.stopTime, 0f, 10f);
            editor.speed = EditorGUILayout.Slider("Speed", editor.speed, 0, 10);
            
        }
    }
#endif
    #endregion
}
