#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LBController))]
public class LBControllerEditor : Editor {
    private bool showRootStates = true;
    private bool showSubStates = true;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        LBController controller = (LBController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("==== DEBUG STATES ====", EditorStyles.boldLabel);

        // --- ROOT STATES ---
        showRootStates = EditorGUILayout.Foldout(showRootStates, "Root States");
        if (showRootStates) {
            if (controller.RootStates != null && controller.RootStates.Count > 0)
            {
                EditorGUI.indentLevel++;
                foreach (var kvp in controller.RootStates)
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.HelpBox("RootStates dictionary is empty or not initialized.", MessageType.Info);
            }
        }

        EditorGUILayout.Space();

        // --- SUB STATES ---
        showSubStates = EditorGUILayout.Foldout(showSubStates, "Sub States");
        if (showSubStates) {
            if (controller.SubStates != null && controller.SubStates.Count > 0)
            {
                EditorGUI.indentLevel++;
                foreach (var kvp in controller.SubStates)
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.HelpBox("SubStates dictionary is empty or not initialized.", MessageType.Info);
            }
        }
    }
}
#endif