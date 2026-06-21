using UnityEngine;

[CreateAssetMenu]
public class PhysicsInfo : ScriptableObject {
    [Header("Physics Info")]
    [SerializeField] 
    [Range(0, 50f)] public float SpeedCap; 
    [SerializeField] 
    [Range(2, 50f)] public float JumpHeightCap;
    [SerializeField] 
    [Range(0, 50f)] public float MaxSpeed;
    
}