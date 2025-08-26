using Cinemachine;
using UnityEngine;

public class GroupableTarget : MonoBehaviour, ICinemachineTargetGroup {
    [SerializeField]
    [RangeAttribute(0, 100)] int _weight = 1;

    [SerializeField]
    [RangeAttribute(0, 100)] float _radius = 1;

    [SerializeField]
    [RangeAttribute(-100, 100)] float _xOffset = 0;

    [SerializeField]
    [RangeAttribute(-100, 100)] float _yOffset = 0;

    public Transform Transform => transform;

    public Bounds BoundingBox => new Bounds(transform.position + 
        new Vector3(_xOffset, _yOffset, 0f), Vector3.one * _radius * 2);

    public BoundingSphere Sphere => new BoundingSphere(transform.position + 
        new Vector3(_xOffset, _yOffset, 0f), _radius);
    public bool IsEmpty => _weight <= 0;

    public void GetViewSpaceAngularBounds(Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange) {
        minAngles = Vector2.zero;
        maxAngles = Vector2.zero;
        zRange = Vector2.zero;
    }

    public Bounds GetViewSpaceBoundingBox(Matrix4x4 observer) {
        Vector3 center = observer.MultiplyPoint(transform.position + new Vector3(_xOffset, _yOffset, 0f));
        Vector3 size = observer.MultiplyVector(Vector3.one * _radius * 2);
        return new Bounds(center, size);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + new Vector3(_xOffset, _yOffset, 0f), _radius);
        Gizmos.DrawWireCube(transform.position + new Vector3(_xOffset, _yOffset, 0f), Vector3.one * _radius);
        UnityEditor.Handles.Label(transform.position, "GroupableTarget");
    }

#endif
}
