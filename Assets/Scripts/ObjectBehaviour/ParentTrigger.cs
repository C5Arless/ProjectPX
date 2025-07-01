using UnityEngine;

public class ParentTrigger : MonoBehaviour {
    private IParentTrigger parentTrigger;

    private void Awake() {
        parentTrigger = GetComponentInParent<IParentTrigger>();
    }

    private void OnTriggerEnter(Collider other) {
        parentTrigger?.TriggerEnter(other);
    }

    private void OnTriggerStay(Collider other) {
        parentTrigger?.TriggerStay(other);
    }

    private void OnTriggerExit(Collider other) {
        parentTrigger?.TriggerExit(other);
    }
}

