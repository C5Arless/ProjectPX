using UnityEngine;

public class PX3PhysicsHandler {
    private Rigidbody rb;
    private PX3Controller context;

    private bool isFrozen;
    private bool isBraking;
    
    private Vector3 currentVelocity;
    private Vector3 previousVelocity;

    private Vector3 proxyVelocity;

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

        Vector3 targetVelocity = Vector3.MoveTowards(HorizontalVelocity, velocity, acceleration);
        
        proxyVelocity.x = targetVelocity.x;
        proxyVelocity.z = targetVelocity.z;
    }
    
    public void UpdateGravity(float gravity) {
        if (isFrozen) return;

        if (gravity >= 1) {
            float verticalVelocity = currentVelocity.y - gravity * rb.mass * Time.deltaTime;
            proxyVelocity.y = verticalVelocity;
        } else if (gravity > .025f) {
            proxyVelocity.y = -gravity;
        } else proxyVelocity.y = 0;
    }

    public void ApplyVerticalImpulse(float intensity) {
        if (isFrozen) return;
        
        Vector3 targetVelocity = HorizontalVelocity + Vector3.up * intensity;
        
        previousVelocity = currentVelocity;
        proxyVelocity = targetVelocity;
    }

    public void ApplyStop() {
        if (isFrozen) return;
        
        previousVelocity = currentVelocity;

        proxyVelocity.x = 0f;
        proxyVelocity.z = 0f;
    }
    
    public void FixedUpdate() {
        if (isFrozen) {
            rb.velocity = Vector3.zero;
            currentVelocity = Vector3.zero;
            proxyVelocity = Vector3.zero;
            return;
        }
        
        previousVelocity = currentVelocity;
        
        rb.velocity = proxyVelocity;
        currentVelocity = rb.velocity;
    }
    
    public void Freeze() {
        if (isFrozen) return;

        isFrozen = true;
        previousVelocity = rb.velocity;
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        proxyVelocity = Vector3.zero;
    }
    
    public void Unfreeze(bool resumeVelocity) {
        if (!isFrozen) return;

        isFrozen = false;
        
        proxyVelocity = resumeVelocity ? previousVelocity : Vector3.zero;
    }
}