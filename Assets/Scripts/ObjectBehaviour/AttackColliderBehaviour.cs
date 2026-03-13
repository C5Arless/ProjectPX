using System.Collections;
using UnityEngine;

public class AttackColliderBehaviour : MonoBehaviour {
    private Vector3 contact;
    private bool isBusy;
    
    private void OnCollisionEnter(Collision other) {
        if (isBusy) return;
        
        if (other.collider.CompareTag("Enemy")) {
            Debug.Log("Collision detected: "  + other.collider.name);
            isBusy = true;
            contact = Vector3.zero;

            var enemy = other.gameObject.GetComponent<IDamageable>();
            Debug.Log("Enemy: "  + enemy);
            if (enemy.IsDamaged || enemy.IsDead) return;
            
            contact = other.contacts[0].point;
            StartCoroutine(TriggerHitStop());
        }
    }
    
    private void ResolveVFX() {
        VFXManager.Instance.SpawnFixedVFX(EnvVFX.Hit, contact, Quaternion.identity);
    }

    private IEnumerator TriggerHitStop() {
        GameMaster.Instance._OnHitStop += ResolveVFX;
        yield return null;
        
        yield return StartCoroutine(GameMaster.Instance.ResolveHitStop());
        
        GameMaster.Instance._OnHitStop -= ResolveVFX;
        yield return new WaitForSeconds(.3f);
        isBusy = false;
    }
}
