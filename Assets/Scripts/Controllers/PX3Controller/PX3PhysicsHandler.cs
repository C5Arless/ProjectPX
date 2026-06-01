using UnityEngine;
public class PX3PhysicsHandler {
    private Rigidbody rb;

    private bool isFrozen;

    private Vector3 currentVelocity;
    private Vector3 previousVelocity;
    private Vector3 velocityBeforeFreeze;
    
    #region GetSet
    public Vector3 CurrentVelocity { get => currentVelocity; }
    public Vector3 PreviousVelocity { get => previousVelocity; }
    public Vector3 HorizontalVelocity => new Vector3(currentVelocity.x, 0, currentVelocity.z);
    public Vector3 PreviousHorizontalVelocity => new Vector3(previousVelocity.x, 0, previousVelocity.z);
    public bool IsFrozen { get => isFrozen; }
    #endregion

    public PX3PhysicsHandler(Rigidbody _rb) {
        rb = _rb;
        
        rb.useGravity = false; 
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
        velocityBeforeFreeze = rb.velocity;
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        //rb.isKinematic = true;
    }
    
    public void Unfreeze(bool resumeVelocity) {
        if (!isFrozen) return;

        isFrozen = false;
        //rb.isKinematic = false; 

        if (resumeVelocity) SetVelocity(velocityBeforeFreeze);
        else SetVelocity(Vector3.zero);
    }
    
    public void SetVelocity(Vector3 newVelocity) {
        if (isFrozen) return;
        
        rb.velocity = newVelocity;
        currentVelocity = newVelocity; 
    }

    public void AddVelocityChange(Vector3 force) {
        if (isFrozen) return;
        
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void ApplyCustomGravity(float gravityMagnitude) {
        if (isFrozen) return;
        
        rb.AddForce(Vector3.down * gravityMagnitude, ForceMode.Acceleration);
    }

    public void ApplyLinearMovement(Vector3 moveDirection, float maxSpeed, float acceleration) {
        if (isFrozen) return;
        
        Vector3 targetVelocity = moveDirection * maxSpeed;
        Vector3 velocityChange = Vector3.MoveTowards(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime) - HorizontalVelocity;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}