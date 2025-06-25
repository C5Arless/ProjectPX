using UnityEngine;

public class PlayerShadow : MonoBehaviour {
    [SerializeField] PXController _ctx;
    [SerializeField] GameObject _shadow;
   
    void Update() {
        UpdatePosition();

        if (_ctx.IsGrounded) {

            if (_shadow.activeSelf) {
                _shadow.SetActive(false);
                return;
            }

        } else {

            if (!_shadow.activeSelf) {
                _shadow.SetActive(true);
                return;
            }
        }
    }

    private void UpdatePosition() {
        transform.position = ComputePosition();
    }

    private Vector3 ComputePosition() {
        Vector3 startPosition = _ctx.JumpPoint.transform.position;
        Physics.Raycast(startPosition, Vector3.down, out RaycastHit hitInfo, 12f, LayerMask.GetMask("Ground"));
        Vector3 targetPosition = hitInfo.point;

        return targetPosition;
    }
}
