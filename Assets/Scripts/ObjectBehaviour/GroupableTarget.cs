using Cinemachine;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

public class GroupableTarget : MonoBehaviour, ICinemachineTargetGroup {    
    [SerializeField] SphereCollider _collider;

    private ActionCameraBlock _actionBlock;

    private float _weight;
    private float _drawRadius;
    private float maxDistance;

    private float _weightCap;

    private IEnumerator _weightRoutine;
    private bool _isUpdating;

    private Task currentTask = null;
    private CancellationTokenSource tokenSrc;

    public float Weight { get { return _weight; } }
    public float Radius { get { return _collider.radius; } }

    public Transform Transform => transform;
    public Bounds BoundingBox => _collider.bounds;
    public BoundingSphere Sphere => new BoundingSphere(_collider.bounds.center, _collider.radius);
    public bool IsEmpty => _weight <= 0.01;

    private void Awake() {
        _actionBlock = GetComponentInParent<ActionCameraBlock>();

        _drawRadius = _collider.bounds.extents.x;        

        _weightCap = (_actionBlock.TargetGroup.m_Targets[0].weight * .5f);
        //_weightRoutine = UpdateWeightRoutine();
    }

    private void OnTriggerEnter(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") {
            if (!_isUpdating) {                
                StartCoroutine(UpdateWeightRoutine());
            }

            /*
            if (currentTask == null) {
                tokenSrc = new CancellationTokenSource();
                currentTask = UpdateTargetWeight(tokenSrc.Token);                
            }
            */
        }
    }

    private void OnTriggerStay(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") {
            if (!_isUpdating) {                
                StartCoroutine(UpdateWeightRoutine());
            }

            /*
            if (currentTask == null) {
                tokenSrc = new CancellationTokenSource();
                currentTask = UpdateTargetWeight(tokenSrc.Token);                
            }
            */
        }
    }

    private void OnTriggerExit(Collider other) {
        if (tag != "GroupableTarget") { return; }

        if (other.tag == "GroupablePlayer") {
            if (_isUpdating) {
                _isUpdating = false;
            }

            /*
            if (currentTask != null) {
                tokenSrc.Cancel();
            }
            */
        }
    }

    private async Task UpdateTargetWeight(CancellationToken _token) {        
        var TargetGroup = _actionBlock.TargetGroup;
        int currentIdx = -1;
        float targetWeight = 0;
        float lerpWeight = 0;

        while (!_token.IsCancellationRequested) {

            if (TargetGroup.FindMember(transform) < 0) {
                TargetGroup.AddMember(transform, 0, _collider.radius);                              

                await Task.Yield();
            }


            currentIdx = TargetGroup.FindMember(transform); 
            targetWeight = _weightCap * EvaluateWeightModifier();
            targetWeight = Mathf.Clamp(targetWeight, 0, _weightCap);
            lerpWeight = Mathf.Lerp(TargetGroup.m_Targets[currentIdx].weight, targetWeight, .1f);

            int rounded = (int)lerpWeight * 1000;
            lerpWeight = rounded / 1000;

            _weight = lerpWeight;
            TargetGroup.m_Targets[currentIdx].weight = lerpWeight;
            await Task.Delay(100);

            //UpdateWeight
        }
        
        targetWeight = 0;

        while (TargetGroup.m_Targets[currentIdx].weight > .1f) {            
            currentIdx = TargetGroup.FindMember(transform);
            lerpWeight = Mathf.Lerp(TargetGroup.m_Targets[currentIdx].weight, targetWeight, -.1f);
            
            int rounded = (int)lerpWeight * 1000;
            lerpWeight = rounded / 1000;

            _weight = lerpWeight;            
            TargetGroup.m_Targets[currentIdx].weight = _weight;
            await Task.Delay(100);
        }

        TargetGroup.RemoveMember(transform);
        currentTask = null;

        await Task.Yield();
    }

    private IEnumerator UpdateWeightRoutine() {
        var TargetGroup = _actionBlock.TargetGroup;
        int currentIdx = -1;
        float targetWeight = 0;
        float lerpWeight = 0;

        _isUpdating = true;
        yield return null;

        while (_isUpdating) {

            if (TargetGroup.FindMember(transform) < 0) {
                TargetGroup.AddMember(transform, 0, _collider.radius);

                yield return null;
            }            

            currentIdx = TargetGroup.FindMember(transform);
            targetWeight = _weightCap * EvaluateWeightModifier();
            targetWeight = Mathf.Clamp(targetWeight, 0, _weightCap);
            lerpWeight = Mathf.Lerp(TargetGroup.m_Targets[currentIdx].weight, targetWeight, .01f);


            int rounded = (int)(lerpWeight * Mathf.Pow(10, 3));
            lerpWeight = rounded * Mathf.Pow(10, -3);
            

            var target = TargetGroup.m_Targets[currentIdx];
            target.weight = lerpWeight;

            _weight = target.weight;
            TargetGroup.m_Targets[currentIdx] = target;
            yield return null;

            //UpdateWeight
        }

        targetWeight = 0;

        while (TargetGroup.m_Targets[currentIdx].weight > .1f) {
            currentIdx = TargetGroup.FindMember(transform);
            //lerpWeight = Mathf.Lerp(TargetGroup.m_Targets[currentIdx].weight, targetWeight, -.1f);
            lerpWeight = Mathf.MoveTowards(TargetGroup.m_Targets[currentIdx].weight, targetWeight, .5f);

            int rounded = (int)(lerpWeight * Mathf.Pow(10, 3));
            lerpWeight = rounded * Mathf.Pow(10, -3);

            var target = TargetGroup.m_Targets[currentIdx];
            target.weight = lerpWeight;

            _weight = target.weight;
            TargetGroup.m_Targets[currentIdx] = target;
            yield return null;
        }

        TargetGroup.RemoveMember(transform);
        _isUpdating = false;

        yield break;
    }

    private float EvaluateWeightModifier() {
        Vector3 playerPos = _actionBlock.PlayerPos.transform.position;
        Vector3 currentPos = _collider.center;
        
        float currentDistance = (playerPos - currentPos).magnitude;
        
        if (currentDistance > maxDistance) {
            maxDistance = currentDistance;
        }

        float result = Mathf.InverseLerp(0f, maxDistance, currentDistance);        
        return 1 - result;
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
