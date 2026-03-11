using System.Collections;
using UnityEngine;

public class AttackColliderBehaviour : MonoBehaviour {
    private Vector3 contact;
    private bool isBusy;
    
    private void OnCollisionEnter(Collision other) {
        if (isBusy) return;
        Debug.Log("Collision detected: "  + other.collider.name);

        isBusy = true;
        contact = Vector3.zero;
        if (other.collider.CompareTag("Enemy")) {
            contact = other.contacts[0].point;
            
            if (isBusy) {
                StartCoroutine(TriggerHitStop());
            }
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
        isBusy = false;
    }
}
