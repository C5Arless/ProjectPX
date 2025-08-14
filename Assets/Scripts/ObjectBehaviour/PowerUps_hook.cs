using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PowerUps_hook : MonoBehaviour {
    [SerializeField] GameObject _event;
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] PowerUps _pu;

    private Task _deployTask;

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

        _deployTask = DeployTask();        
    }

    private void ApplyPowerUp() {
        _playerInfo.PowerUps = (int)_pu;
    }

    private async Task DeployTask() {
        _event.SetActive(true);
        await Task.Yield();

        await await GameBucket.Instance.EventsOrchestrator.GetCurrentEventTask();

        await Task.Yield();

        Destroy(gameObject);
    }
}
