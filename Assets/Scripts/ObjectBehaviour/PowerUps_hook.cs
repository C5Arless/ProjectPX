using UnityEngine;

public class PowerUps_hook : MonoBehaviour
{
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] PowerUps _pu;

    private void Awake() {
        if (_playerInfo.PowerUps >= (int)_pu) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") { return; }

        _playerInfo.PowerUps++;        

        Destroy(gameObject);
    }
}
