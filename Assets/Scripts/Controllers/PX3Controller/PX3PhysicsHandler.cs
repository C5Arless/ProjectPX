using UnityEngine;

public class PX3PhysicsHandler {
    private Rigidbody rb;
    private PX3Controller context;
    private PX3InputHandler inputHandler;

    private bool isFrozen;

    private Vector3 currentVelocity;
    private Vector3 previousVelocity;
    private Vector3 velocityBeforeFreeze;

    // Cache locali per le costanti cinematiche calcolate a runtime
    private float calculatedBaseGravity;
    private float calculatedJumpForce;

#region GetSet
    public Vector3 CurrentVelocity { get => currentVelocity; }
    public Vector3 PreviousVelocity { get => previousVelocity; }
    public Vector3 HorizontalVelocity => new Vector3(currentVelocity.x, 0, currentVelocity.z);
    public Vector3 PreviousHorizontalVelocity => new Vector3(previousVelocity.x, 0, previousVelocity.z);
    public bool IsFrozen { get => isFrozen; }
    public float JumpForce { get => calculatedJumpForce; }
#endregion

    public PX3PhysicsHandler(Rigidbody _rb, PX3Controller _context, PX3InputHandler _inputHandler) {
        rb = _rb;
        context = _context;
        inputHandler = _inputHandler;
        
        rb.useGravity = false;
        RecalculateJumpConstants();
    }
    
    public void RecalculateJumpConstants() {
        if (context.PhysicsData.TimeToApex <= 0f) return;

        // Formula: Gravity = (2 * Altezza) / Tempo^2
        calculatedBaseGravity = (2f * context.PhysicsData.MaxJumpHeight) / (context.PhysicsData.TimeToApex * context.PhysicsData.TimeToApex);
        
        // Formula: JumpForce = (2 * Altezza) / Tempo
        calculatedJumpForce = (2f * context.PhysicsData.MaxJumpHeight) / context.PhysicsData.TimeToApex;
    }
    
    public void TickPhysics(Vector3 inputDirection, float deltaTime) {
        if (isFrozen) return;

        Vector3 hVel = HorizontalVelocity;
        float verticalVel = currentVelocity.y;

        // Recuperiamo i dati correnti che lo stato attivo ha configurato nel contesto
        float acceleration = context.PhysicsData.CurrentAcceleration;
        float deceleration = context.PhysicsData.CurrentDeceleration;
        float maxSpeed = context.PhysicsData.CurrentSpeedCap;

        // 1. FRENATA / ATTRITO (Se rilasci l'analogico o se lo stato forza una decelerazione)
        if (inputDirection.magnitude == 0f || acceleration == 0f) {
            Vector3 brakeForce = hVel.normalized * deceleration * deltaTime;
            
            if (hVel.magnitude > brakeForce.magnitude) {
                hVel -= brakeForce;
            } else {
                hVel = Vector3.zero;
            }
        }

        // 2. ACCELERAZIONE PROGRESSIVA
        if (inputDirection.magnitude > 0f && acceleration > 0f) {
            hVel += inputDirection.normalized * acceleration * deltaTime;
        }

        // 3. SPEED CAP DINAMICO (Limita solo l'asse orizzontale)
        if (hVel.magnitude > maxSpeed) {
            hVel = hVel.normalized * maxSpeed;
        }

        // Riapplichiamo la velocità finale al Rigidbody
        SetVelocity(new Vector3(hVel.x, verticalVel, hVel.z));
    }

    /// <summary>
    /// Calcola e applica la gravità asimmetrica (Smorzamento, Apex Hang, Caduta Esponenziale) 
    /// basandosi sulla formula cinematica e sullo stato attuale del salto.
    /// </summary>
    public void ApplyCinematicGravity(float deltaTime) {
        if (isFrozen) return;

        float verticalVelocity = currentVelocity.y;

        // FASE 1: SMORZAMENTO ANTICIPATO (Il giocatore rilascia il tasto Salto mentre sale)
        if (verticalVelocity > 0.1f && !inputHandler.JumpInput) {
            context.PhysicsData.CurrentGravity = calculatedBaseGravity * 2.5f; 
        }
        // FASE 2: APEX HANG (Momento di fluttuazione a mezz'aria quando Y è vicino a zero)
        else if (Mathf.Abs(verticalVelocity) < 1.5f) {
            context.PhysicsData.CurrentGravity = calculatedBaseGravity * 0.2f; 
        }
        // FASE 3: CADUTA PESANTE (Accelerazione esponenziale naturale basata sulla velocità di caduta)
        else if (verticalVelocity < -0.1f) {
            // Più cadi velocemente, più la gravità aumenta, fino a un massimo simulato dall'Air Drag del contesto
            float dynamicMultiplier = 1f + Mathf.Abs(verticalVelocity) * 0.15f;
            context.PhysicsData.CurrentGravity = calculatedBaseGravity * dynamicMultiplier;
        }
        // FASE STANDARD: Salita normale con tasto premuto
        else {
            context.PhysicsData.CurrentGravity = calculatedBaseGravity;
        }

        // Applica la gravità finale calcolata sull'asse Y del Rigidbody
        Vector3 vel = rb.velocity;
        vel.y -= context.PhysicsData.CurrentGravity * deltaTime;

        // Applica un freno all'aria (Air Drag) per la velocità di caduta terminale
        if (verticalVelocity < 0f) {
            vel.y = Mathf.MoveTowards(vel.y, -calculatedBaseGravity, context.PhysicsData.AirDrag * deltaTime);
        }

        SetVelocity(vel);
    }

    public bool IsInvertingDirection(Vector3 inputDirection) {
        if (HorizontalVelocity.magnitude < 0.5f || inputDirection.magnitude == 0f) 
            return false;

        float dot = Vector3.Dot(inputDirection.normalized, HorizontalVelocity.normalized);
        return dot < -0.5f;
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
    }
    
    public void Unfreeze(bool resumeVelocity) {
        if (!isFrozen) return;

        isFrozen = false;

        if (resumeVelocity) SetVelocity(velocityBeforeFreeze);
        else SetVelocity(Vector3.zero);
    }
    
    public void SetVelocity(Vector3 newVelocity) {
        if (isFrozen) return;
        
        rb.velocity = newVelocity;
        currentVelocity = newVelocity; 
    }
}