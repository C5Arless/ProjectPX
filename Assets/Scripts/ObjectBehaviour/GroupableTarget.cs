using Cinemachine;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class GroupableTarget : MonoBehaviour, ICinemachineTargetGroup {    
    [SerializeField] SphereCollider _collider;

    private ActionCameraBlock _actionBlock;

    private int _weight;
    private float _drawRadius;

    private int _weightCap;

    private Task currentTask = null;
    private CancellationTokenSource tokenSrc;

    public int Weight { get { return _weight; } }
    public float Radius { get { return _collider.radius; } }

    public Transform Transform => transform;
    public Bounds BoundingBox => _collider.bounds;
    public BoundingSphere Sphere => new BoundingSphere(_collider.bounds.center, _collider.radius);
    public bool IsEmpty => _weight <= 0;

    private void Awake() {
        _actionBlock = GetComponentInParent<ActionCameraBlock>();

        _drawRadius = _collider.bounds.extents.x;        

        _weightCap = (int)(_actionBlock.TargetGroup.m_Targets[0].weight / 1.5f);
    }

    private void OnTriggerEnter(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") {
            if (currentTask == null) {
                tokenSrc = new CancellationTokenSource();
                currentTask = UpdateTargetWeight(tokenSrc.Token);                
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") {              
            if (currentTask == null) {
                tokenSrc = new CancellationTokenSource();
                currentTask = UpdateTargetWeight(tokenSrc.Token);                
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") { 
            if (currentTask != null) {
                tokenSrc.Cancel();
            }        
        }
    }

    private async Task UpdateTargetWeight(CancellationToken _token) {        
        var TargetGroup = _actionBlock.TargetGroup;
        int currentIdx = -1;
        int targetWeight = 2;
        float lerpWeight = 0;

        while (!_token.IsCancellationRequested) {

            if (TargetGroup.FindMember(transform) < 0) {
                TargetGroup.AddMember(transform, 0, _collider.radius);

                currentIdx = TargetGroup.FindMember(transform);
                TargetGroup.m_Targets[currentIdx].weight = targetWeight;
                await Task.Yield();
            }

            targetWeight = (int)(_weightCap * EvaluateWeightModifier());
            targetWeight = Mathf.Clamp(targetWeight, 0, _weightCap);
            lerpWeight = Mathf.Lerp(TargetGroup.m_Targets[currentIdx].weight, targetWeight, 2);

            TargetGroup.m_Targets[currentIdx].weight = (int)lerpWeight;
            await Task.Yield();

            //UpdateWeight
        }

        TargetGroup.RemoveMember(transform);
        currentTask = null;

        await Task.Yield();
    }

    private float EvaluateWeightModifier() {
        Vector3 playerPos = _actionBlock.PlayerPos.transform.position;
        Vector3 currentPos = transform.position;
        
        float currentDistance = (playerPos - currentPos).magnitude;

        float result = Mathf.Clamp01(_collider.radius / currentDistance);        

        return result;
    }

    public void GetViewSpaceAngularBounds(Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange) {
        minAngles = Vector2.zero;
        maxAngles = Vector2.zero;
        zRange = Vector2.zero;
    }

    public Bounds GetViewSpaceBoundingBox(Matrix4x4 observer) {        
        Vector3 center = observer.MultiplyPoint(_collider.bounds.center);
        Vector3 size = observer.MultiplyVector(Vector3.one * _collider.radius);
        return new Bounds(center, size);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_collider.bounds.center, _drawRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_collider.bounds.center, Vector3.one * _drawRadius);

        UnityEditor.Handles.Label(_collider.bounds.center, "GroupableTarget");
    }

#endif
}
