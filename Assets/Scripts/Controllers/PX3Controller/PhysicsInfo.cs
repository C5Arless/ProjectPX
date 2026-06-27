using UnityEngine;

[CreateAssetMenu]
public class PhysicsInfo : ScriptableObject {
    [Header("Physics Info")]
    [SerializeField] 
    [Range(0, 50f)] public float SpeedCap; 
    [SerializeField] 
    [Range(1, 50f)] public float JumpHeightCap;
    [SerializeField] 
    [Range(0, 50f)] public float MaxSpeed;
    [SerializeField] 
    [Range(0, 50f)] public float MaxAcceleration;
    [SerializeField] 
    [Range(0, 50f)] public float Gravity;

    [Header("Current Info")] 
    public float Acceleration;
}