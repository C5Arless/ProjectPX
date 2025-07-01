using UnityEngine;

public class Checkpoint_hook : MonoBehaviour {

    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] Scenes _scene;
    [SerializeField] Cp _cp;

    Vector2 _target;

    private void Awake() {
        _target = new Vector2((int)_scene, (int)_cp);
        
        if (_playerInfo.Checkpoint == _target) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") { return; }        

        _playerInfo.Checkpoint = _target;

        Destroy(gameObject);        
    }
}
