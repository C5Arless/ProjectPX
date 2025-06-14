using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VersionTextRenderer : MonoBehaviour {
    [SerializeField] TMP_Text _version;

    // Start is called before the first frame update
    void Start() {
        _version.text += PlayerSettings.bundleVersion;
    }
}
