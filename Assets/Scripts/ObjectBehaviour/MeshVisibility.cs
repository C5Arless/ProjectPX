using UnityEngine;

public class MeshVisibility : MonoBehaviour {
    [Tooltip("True => Show; False => Hide")]
    [SerializeField] private bool initialState;

    private Renderer[] renderers;

    private void Awake() {
        renderers = GetComponentsInChildren<Renderer>();
        
        SetState(initialState);
    }

    public void Show() {
        SetState(true);
    }

    public void Hide() {
        SetState(false);
    }
    
    private void SetState(bool state) {
        foreach (var renderer in renderers) {
            renderer.enabled = state;
        }
    }
}
