using UnityEngine;

public class CollectableShadow : MonoBehaviour {
    [SerializeField] GameObject _shadow;
   
    void Update() {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 12f, LayerMask.GetMask("Ground"))) {
            if (!_shadow.activeSelf) _shadow.SetActive(true);
            _shadow.transform.position = hitInfo.point;
        }
        else {
            if (_shadow.activeSelf) _shadow.SetActive(false);
        }
    }
}
