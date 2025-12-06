using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Collectables : MonoBehaviour {
    [SerializeField] PlayerInfo playerInfo;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {

            StartCoroutine(GatherRoutine());
        }
    }

    private void GatherCollectable() {
        playerInfo.Score++;

        if (playerInfo.CurrentHp < 3) {
            playerInfo.CurrentHp++;
        }
    }

    private IEnumerator GatherRoutine() {
        GatherCollectable();
        yield return null;

        yield return RiseAndShrink();
        
        Destroy(gameObject);
    }

    private IEnumerator RiseAndShrink() {
        Vector3 currentPos = transform.position;
        Vector3 currentScale = transform.localScale;
        float multiplier = 1f;
        
        while (multiplier > 0f) {
            float yOffset = currentPos.y + (1 - multiplier);
            Vector3 scale = currentScale * multiplier;
            
            transform.position =  new Vector3(currentPos.x, yOffset, currentPos.z);
            transform.localScale = scale;
            
            multiplier -= 0.05f;
            yield return null;
        }
        
        GetComponent<VisualEffect>().Stop();
        yield return new WaitForSeconds(1f);
    }
}
