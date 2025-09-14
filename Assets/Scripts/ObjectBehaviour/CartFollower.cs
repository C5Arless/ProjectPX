using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineDollyCart))]
public class CartFollower : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineDollyCart dollyCart;

    [Header("Settings")]
    [SerializeField] private int searchResolution = 10;        // precisione della ricerca sul path
    [SerializeField] private float updateThreshold = 0.1f;     // distanza minima di movimento del player per aggiornare
    [SerializeField] private float maxUpdateRate = 0.05f;      // tempo minimo tra due update (in secondi)

    private Vector3 lastPlayerPos;
    private float lastUpdateTime;

    private void Start() {
        if (player != null) {
            lastPlayerPos = player.position;
        }
    }

    private void Update() {
        if (player == null || dollyCart == null || dollyCart.m_Path == null) { return; }

        // Controlla se il player si è mosso abbastanza
        float distMoved = Vector3.Distance(player.position, lastPlayerPos);
        if (distMoved < updateThreshold && Time.time - lastUpdateTime < maxUpdateRate) { return; }        

        int startSegment = Mathf.FloorToInt(dollyCart.m_Path.StandardizeUnit(dollyCart.m_Position, CinemachinePathBase.PositionUnits.Distance));

        // Trova il punto più vicino sul path
        float closestPos = dollyCart.m_Path.FindClosestPoint(player.position, startSegment, -1, searchResolution);

        dollyCart.m_Position = closestPos;

        // Aggiorna i riferimenti
        lastPlayerPos = player.position;
        lastUpdateTime = Time.time;
    }
}
