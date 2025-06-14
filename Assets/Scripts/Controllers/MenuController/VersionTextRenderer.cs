using TMPro;
using UnityEngine;

public class VersionTextRenderer : MonoBehaviour {
    [SerializeField] TMP_Text _version;

    void Start() {
        _version.text += Application.version;
    }
}
