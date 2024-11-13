using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DamageColliders : MonoBehaviour {
    bool paused;
    Collision collision;

    private void OnCollisionEnter(Collision info) {
        if (info.collider.tag == "Obstacle") {
            collision = info;

            Debug.Log(collision.GetContact(0).point);

            if (!paused) {
                StartCoroutine("TriggerInteraction");
                paused = true;
            }
            
        }
    }

    private IEnumerator TriggerInteraction() {
        VFXManager.Instance.SpawnFixedVFX(EnvVFX.Splash, collision.GetContact(0).point, this.transform.rotation);
        yield return new WaitForSeconds(1f);

        paused = false;
        yield break;
    }
}
