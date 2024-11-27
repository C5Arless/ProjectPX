using UnityEngine;

public class Stucked : MonoBehaviour {
    [SerializeField] CompanionController _ctx;

    private void OnTriggerEnter(Collider other) {
        if (_ctx.IsMoving || _ctx.IsTalking) { return; }

        if (other.tag != "PlayerAttacks" && other.tag != "Interact" && other.tag != "PlayerVirtualCamera") {
            _ctx.IsStuck = true;
            _ctx.IsOperative = false;
        }         
    }

    private void OnTriggerStay(Collider other) {        
        if (_ctx.IsMoving || _ctx.IsTalking) { return; }

        if (other.tag != "PlayerAttacks" && other.tag != "Interact" && other.tag != "PlayerVirtualCamera") {
            _ctx.IsStuck = true;
            _ctx.IsOperative = false;
        }

        if (_ctx.IsStuck) {

            _ctx.IsUnstucking = true;
        }

    }
    private void OnTriggerExit(Collider other) {
        if (_ctx.IsMoving || _ctx.IsTalking) { return; }

        if (other.tag != "PlayerAttacks" && other.tag != "Interact" && other.tag != "PlayerVirtualCamera") {
            _ctx.IsUnstucking = false;
            _ctx.IsStuck = false;
            _ctx.IsOperative = true;
        }

    }
}
