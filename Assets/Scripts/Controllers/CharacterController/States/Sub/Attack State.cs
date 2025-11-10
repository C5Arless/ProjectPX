using System;
using UnityEngine;

public class AttackState : BaseState, IContextInit, IVFXInit {
    private GameObject enemy;
    
    public AttackState(PXController currentContext, StateHandler stateHandler, AnimHandler animHandler) : base(currentContext, stateHandler, animHandler) {
        //State Constructor
    }

    public override void EnterState() {
        //Enter logic
        
        //Check if enemy is near
        CheckEnemy();
        
        InitializeContext();

        ColliderOn(Ctx.AttackCollider);
        
        if (!Ctx.IsGrounded && !Ctx.IsDashing) {
            Ctx.OnKinematic = true;
            Ctx.PlayerRb.isKinematic = true;
            Ctx.SetKinematic();
        }
        
        //if enemy near dont freeze rotation
        if (enemy is null) {
            Ctx.PlayerRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
        else {
            Ctx.PlayerRb.constraints = RigidbodyConstraints.FreezePositionY;
        }
        

        GravityOff();

        InitializeParticles();
        
        Ctx.AnimHandler.PlayDirect(AnimHandler.Attack1());
        Ctx.StartCoroutine("ResetAttack");
        
    }

    public override void UpdateState() {        
        //if enemy near, track enemy
        if (enemy is not null) TrackEnemy();
        
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Exit logic
        
        //reset tracking
        if (enemy is not null) ResetTracking();
        
        Ctx.AttackInput = false;
        Ctx.PlayerRb.velocity.Set(0f, 0f, 0f);

        if (!Ctx.IsGrounded) {
            Ctx.OnKinematic = false;
            Ctx.PlayerRb.isKinematic = false;            
        }

        Ctx.PlayerRb.constraints = RigidbodyConstraints.FreezeRotation;
        Ctx.PlayerRb.ResetInertiaTensor();

        ColliderOff(Ctx.AttackCollider);
        GravityOn();
    }

    public override void CheckSwitchStates() {
        //Switch logic        
        if (Ctx.IsGrounded && Ctx.CanAttack && Ctx.IsWalking) {          
            SwitchState(StateHandler.Walk());
        }
        else if (Ctx.CanAttack && Ctx.IsIdle) {
            SwitchState(StateHandler.Idle());
        }
        else if (Ctx.IsDamaged) {
            SwitchState(StateHandler.Damage());
        } 
        else if (Ctx.IsAttacking && Ctx.IsDashing) {
            SwitchState(StateHandler.Dash());
        }
        else if (Ctx.IsJumping) {
            SwitchState(StateHandler.Jump());
        }
        else if (!Ctx.IsGrounded && Ctx.IsFalling) {
            SwitchState(StateHandler.Fall());
        }
    }

    public void InitializeContext() {
        Ctx.AttackInput = false;
        Ctx.AttackCount--;

        Ctx.IsWalking = false;
        Ctx.IsIdle = false;
        Ctx.IsDashing = false;
        Ctx.IsJumping = false;
        Ctx.IsFalling = false;
    }

    public void InitializeParticles() {
        VFXManager.Instance.SpawnFollowVFX(PlayerVFX.AttackBurst, Ctx.AttackPoint.transform.position, Ctx.AttackPoint.transform.rotation, Ctx.AttackPoint);
    }

    private void CheckEnemy() {
        Collider[] entities = new Collider[15];
        int hits = Physics.OverlapSphereNonAlloc(Ctx.Asset.transform.position, 2f, entities, LayerMask.GetMask("Enemy"));
        
        if (hits > 0) {
            float distance = Single.PositiveInfinity;

            foreach (var entity in entities) {
                if (entity is null) continue;
                
                float tempDistance = (Ctx.Asset.transform.position - entity.transform.position).magnitude;

                if (tempDistance < distance) {
                    distance = tempDistance;
                    
                    enemy = entity.gameObject;
                }
            }
            
        } else {
            enemy = null;
        }
    }

    private void TrackEnemy() {
        Vector3 forward = Ctx.ComputeForward2D(enemy.transform.position, Ctx.Asset.transform.position);
        
        Ctx.Asset.transform.forward = forward;
        Ctx.PlayerForward.transform.forward = forward;
        Ctx.Player.transform.forward = forward;
    }

    private void ResetTracking() {
        enemy = null;
    }
}
