using UnityEngine;

[ExecuteAlways]
public class CopyPosition : MonoBehaviour {
    [SerializeField] public bool isActive;
    [SerializeField] public GameObject target;

    private void Update() {
        if (isActive) {
            if (target is not null) transform.position = target.transform.position;
        }
    }
}
