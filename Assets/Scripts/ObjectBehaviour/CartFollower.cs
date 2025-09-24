using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineDollyCart))]
public class CartFollower : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineDollyCart dollyCart;
    private CinemachineSmoothPath path;

    [Header("Settings")]
    [Tooltip("Numero di segmenti da controllare attorno a quello attuale")]
    [SerializeField] private int searchRadius = 2;

    [Tooltip("Velocità di interpolazione verso la nuova posizione del carrello")]
    [SerializeField] private float lerpSpeed = .8f;

    [Tooltip("Distanza negativa da mantenere dal player nello spazio 3D (sempre < 0)")]
    [SerializeField] private float followDistance = -5f;

    [Tooltip("Tempo minimo tra due aggiornamenti (in secondi)")]
    [SerializeField] private float maxUpdateRate = 0.05f;

    [Tooltip("Distanza minima di movimento del player per aggiornare")]
    [SerializeField] private float updateThreshold = 0.1f;

    private Vector3 lastPlayerPos;
    private float lastUpdateTime;
    private float targetCartPos;
    private float currentCartPos;

    private Vector3 debugClosestPoint;
    private Vector3 debugCartTarget;

    private void Start() {
        if (player != null) lastPlayerPos = player.position;

        dollyCart.m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;

        currentCartPos = dollyCart.m_Position;
        targetCartPos = dollyCart.m_Position;

        if (followDistance > 0) followDistance = -followDistance;

        path = dollyCart.m_Path as CinemachineSmoothPath;
        if (path == null) {
            Debug.LogError("CartFollower richiede un CinemachineSmoothPath come m_Path!");
        }
    }

    private void Update() {
        if (!IsValid()) return;

        if (ShouldUpdatePosition()) {
            targetCartPos = FindCartPositionBehindPlayer();
            lastPlayerPos = player.position;
            lastUpdateTime = Time.time;
        }

        SmoothMoveCart();
    }

    private bool IsValid() {
        return player != null && dollyCart != null && path != null;
    }

    private bool ShouldUpdatePosition() {
        float distMoved = Vector3.Distance(player.position, lastPlayerPos);
        return distMoved >= updateThreshold || (Time.time - lastUpdateTime >= maxUpdateRate);
    }

    private float FindCartPositionBehindPlayer() {
        int resolution = Mathf.Max(1, path.m_Resolution);
        int numSegments = Mathf.Max(1, path.m_Waypoints.Length - 1);

        float currentPos = dollyCart.m_Position; 
        int startSegment = Mathf.FloorToInt(path.ToNativePathUnits(currentPos, CinemachinePathBase.PositionUnits.Distance));
        float closestNative = path.FindClosestPoint(player.position, startSegment, searchRadius, resolution);
        float basePos = path.FromPathNativeUnits(closestNative, CinemachinePathBase.PositionUnits.Distance);

        debugClosestPoint = path.EvaluatePositionAtUnit(basePos, CinemachinePathBase.PositionUnits.Distance);

        float targetDist = Mathf.Abs(followDistance);

        float step = path.PathLength / (resolution * numSegments * 2);
        int maxSteps = resolution * numSegments * 2;

        float bestPos = basePos;
        float bestError = Mathf.Infinity;

        float testPos = basePos;
        for (int i = 0; i < maxSteps; i++) {
            testPos -= step;
            float standardized = path.StandardizeUnit(testPos, CinemachinePathBase.PositionUnits.Distance);

            Vector3 testWorld = path.EvaluatePositionAtUnit(standardized, CinemachinePathBase.PositionUnits.Distance);
            float error = Mathf.Abs(Vector3.Distance(testWorld, player.position) - targetDist);

            if (error < bestError) {
                bestError = error;
                bestPos = standardized;
            }
            else {                
                break;
            }
        }

        debugCartTarget = path.EvaluatePositionAtUnit(bestPos, CinemachinePathBase.PositionUnits.Distance);

        return bestPos;
    }

    private void SmoothMoveCart() {
        currentCartPos = Mathf.Lerp(currentCartPos, targetCartPos, lerpSpeed * Time.deltaTime);
        dollyCart.m_Position = currentCartPos;
    }

    private void OnDrawGizmos() {
        if (player == null || dollyCart == null || path == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(debugClosestPoint, 0.2f);
        Gizmos.DrawLine(player.position, debugClosestPoint);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(debugCartTarget, 0.2f);
        Gizmos.DrawLine(debugClosestPoint, debugCartTarget);
    }
}
