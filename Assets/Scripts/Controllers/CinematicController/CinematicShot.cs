using Cinemachine;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CinematicShot : MonoBehaviour {
    [SerializeField] GameObject _vcamera;
    [SerializeField] GameObject _focusTarget;
    [SerializeField] GameObject _playerTarget;
    [SerializeField] GameObject _companionTarget;
    [SerializeField] GameObject _dollyCart;
    [Space]
    [SerializeField] float _shotLenght;
    [SerializeField][Range(.1f, 50f)] float _speed;
    [SerializeField] bool hasDolly;
    [SerializeField] bool hasTransition;

    private CinematicController _controller;

    public GameObject FocusTarget { get { return _focusTarget; } set { _focusTarget = value; } }
    public GameObject PlayerTarget { get { return _playerTarget;} set { _playerTarget = value; } }
    public GameObject CompanionTarget { get { return _companionTarget; } set { _companionTarget = value; } }

    public bool HasTransition { get { return hasTransition; } }

    private void Awake() {
        _controller = GetComponentInParent<CinematicController>();
    }

    private void Exit() {
        if (hasDolly) {
            _dollyCart.GetComponent<CinemachineDollyCart>().m_Position = 0f;
            _dollyCart.SetActive(false);
        }        
    }

    public void Enter() {
        if (hasDolly) {
            _dollyCart.SetActive(true);
            _dollyCart.GetComponent<CinemachineDollyCart>().m_Position = 0f;
            _dollyCart.GetComponent<CinemachineDollyCart>().m_Speed = _speed;
        }

        _controller.PXController.CinematicEnter(_playerTarget.transform, _focusTarget.transform, _vcamera);
        _controller.CompanionController.TravelSetUpTalkBehaviour(_companionTarget.transform.position);
        _controller.CompanionController.VisionSetUpTalkBehaviour(_companionTarget);

        CameraManager.Instance.SwitchGameVCamera(_vcamera);

        StartCoroutine(DoneAfterLenght());
    }

    public IEnumerator DoneAfterLenght() {
        yield return new WaitForSeconds(_shotLenght);

        _controller.OnInteract();
        Exit();

        yield break;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (_focusTarget != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_focusTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_focusTarget.transform.position, "FocusTarget");
        }

        if (_playerTarget != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_playerTarget.transform.position, "PlayerTarget");
        }

        if (_companionTarget != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_companionTarget.transform.position, 0.2f);
            UnityEditor.Handles.Label(_companionTarget.transform.position, "CompanionTarget");
        }
    }

#endif
}
