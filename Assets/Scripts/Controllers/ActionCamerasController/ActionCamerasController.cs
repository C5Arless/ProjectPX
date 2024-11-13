using UnityEngine;

public class ActionCamerasController : MonoBehaviour {
    private PXController _playerCtx;
    private CompanionController _companionCtx;

    public PXController PXController { get { return _playerCtx; } }
    public CompanionController CompanionCtx { get {  return _companionCtx; } }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _playerCtx = other.GetComponent<PXController>();

            try {
                _companionCtx = GameObject.FindGameObjectWithTag("Companion").GetComponent<CompanionController>();
            }
            catch {
                Debug.Log("Companion not found");
            }
        }
    }
    
}
