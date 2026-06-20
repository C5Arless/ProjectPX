using UnityEngine;

[CreateAssetMenu]
public class PhysicsInfo : ScriptableObject {
    [Header("Current Info")]
    [SerializeField] public float CurrentAcceleration; 
    [SerializeField] public float CurrentDeceleration; 
    [SerializeField] public float CurrentSpeedCap;     
    [SerializeField] public float CurrentGravity;
    [Space]
    [Header("Ground Movement Info")]
    [SerializeField] public float BaseRunAcceleration = 40f; 
    [SerializeField] public float BaseRunFriction = 25f;     
    [SerializeField] public float BaseMaxRunSpeed = 12f;
    [SerializeField] public float BrakeMultiplier = 4f;
    [Space]
    [Header("Cinematic Jump Info")]
    [SerializeField] public float MaxJumpHeight = 4.5f;       // Altezza massima del salto in unità (metri)
    [SerializeField] public float TimeToApex = 0.4f;          // Tempo (in secondi) per raggiungere il punto più alto
    [SerializeField] public float AirAcceleration = 20f;      // Controllo direzionale in aria
    [SerializeField] public float AirDrag = 5f;
    
}