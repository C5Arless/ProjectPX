using UnityEngine;

public class PX3PhysicsHandler {
    private Rigidbody rb;
    private PX3Controller context;

    private bool isFrozen;
    private bool isBraking;
    
    private Vector3 currentVelocity;
    private Vector3 previousVelocity;

#region GetSet
    public Vector3 CurrentVelocity { get => currentVelocity; }
    public Vector3 PreviousVelocity { get => previousVelocity; }
    public Vector3 HorizontalVelocity => new Vector3(currentVelocity.x, 0, currentVelocity.z);
    public Vector3 PreviousHorizontalVelocity => new Vector3(previousVelocity.x, 0, previousVelocity.z);
    public bool IsFrozen { get => isFrozen; }
    
#endregion

    public PX3PhysicsHandler(Rigidbody _rb, PX3Controller _context) {
        rb = _rb;
        context = _context;
        
        rb.useGravity = false;
    }

    public void UpdateMovement(Vector3 velocity, float acceleration) {
        if (isFrozen) return;

        Vector3 targetVelocity = Vector3.MoveTowards(HorizontalVelocity, velocity, acceleration * Time.deltaTime);
        SetVelocity(targetVelocity);
    }
    
    public void UpdateGravity(float gravity) {
        if (isFrozen) return;
        
        float verticalVelocity = currentVelocity.y - gravity * Time.deltaTime;
        SetVelocity(new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z));
    }

    public void ApplyImpulse(Vector3 direction, float intensity) {
        if (isFrozen) return;
        
        Vector3 targetVelocity = direction * intensity;
        SetVelocity(targetVelocity);
    }

    public void ApplyBrake() {
        if (isFrozen) return;
        
        SetVelocity(Vector3.zero);
    }
    
    public void FixedUpdate() {
        if (isFrozen) {
            rb.velocity = Vector3.zero;
            currentVelocity = Vector3.zero;
            return;
        }

        previousVelocity = currentVelocity;
        currentVelocity = rb.velocity;
    }
    
    public void Freeze() {
        if (isFrozen) return;

        isFrozen = true;
        previousVelocity = rb.velocity;
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentVelocity = Vector3.zero;
    }
    
    public void Unfreeze(bool resumeVelocity) {
        if (!isFrozen) return;

        isFrozen = false;
        
        if (resumeVelocity) SetVelocity(previousVelocity);
        else SetVelocity(Vector3.zero);
    }
    
    private void SetVelocity(Vector3 newVelocity) {
        if (isFrozen) return;
        
        rb.velocity = newVelocity;
        currentVelocity = newVelocity; 
    }
}