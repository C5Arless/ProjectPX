using UnityEngine;

public class PowerUps_hook : MonoBehaviour {
    [SerializeField] GameObject _event;
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] PowerUps _pu;

    private void Awake() {
        if (_playerInfo.PowerUps >= (int)_pu) {
            Destroy(_event);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") { return; }

        DeployEvent();
    }

    private void DeployEvent() {
        ApplyPowerUp();
        _event.SetActive(true);

        Destroy(gameObject);
    }

    private void ApplyPowerUp() {
        _playerInfo.PowerUps++;
    }
}
