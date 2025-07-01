using UnityEngine;

public class PivotRotation : MonoBehaviour {
    [SerializeField]
    [Range(10f, 100f)] float speed;
    void Update() {
        this.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
