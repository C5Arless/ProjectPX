#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PX3Controller))]
public class PX3ControllerEditor : Editor {
    private bool showRootStates = false;
    private bool showSubStates = false;
    
    public override bool RequiresConstantRepaint() {
        return Application.isPlaying;
    }
    
    public override void OnInspectorGUI() {
        // Disegna l'inspector di default (campi pubblici, ecc.)
        DrawDefaultInspector();

        PX3Controller controller = (PX3Controller)target;
        
        // RECUPERO DELLO STATE HANDLER
        // Nota: Assicurati che 'StateHandler' sia accessibile (public o internal) in PX3Controller
        var handler = controller.StateHandler; 

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("==== PX3 STATE MACHINE DEBUG ====", EditorStyles.boldLabel);

        if (handler == null) {
            EditorGUILayout.HelpBox("PX3StateHandler non è inizializzato o non è accessibile.", MessageType.Warning);
            return;
        }

        // --- STATI ATTUALI (CURRENT STATES) ---
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        string rootStateStr = handler.CurrentRootState != null ? handler.CurrentRootState.GetType().Name : "NULL";
        string subStateStr = handler.CurrentSubState != null ? handler.CurrentSubState.GetType().Name : "NULL";

        EditorGUILayout.LabelField("Current Root State:", rootStateStr, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Sub State:", subStateStr, EditorStyles.boldLabel);
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // --- ROOT STATES DICTIONARY ---
        showRootStates = EditorGUILayout.Foldout(showRootStates, "Root States Mask List Layout");
        if (showRootStates) {
            if (handler.RootStates != null && handler.RootStates.Count > 0) {
                EditorGUI.indentLevel++;
                foreach (var kvp in handler.RootStates) {
                    // Visualizza il nome dell'enum e il suo valore booleano
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            } else {
                EditorGUILayout.HelpBox("Dizionario RootStates vuoto o non inizializzato.", MessageType.Info);
            }
        }

        EditorGUILayout.Space();

        // --- SUB STATES DICTIONARY ---
        showSubStates = EditorGUILayout.Foldout(showSubStates, "Sub States Mask List Layout");
        if (showSubStates) {
            if (handler.SubStates != null && handler.SubStates.Count > 0) {
                EditorGUI.indentLevel++;
                foreach (var kvp in handler.SubStates) {
                    // Visualizza il nome dell'enum e il suo valore booleano
                    EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value.ToString());
                }
                EditorGUI.indentLevel--;
            } else {
                EditorGUILayout.HelpBox("Dizionario SubStates vuoto o non inizializzato.", MessageType.Info);
            }
        }
    }
}
#endif