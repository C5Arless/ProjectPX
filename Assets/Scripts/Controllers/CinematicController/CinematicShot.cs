using Cinemachine;
using System.Collections;
using UnityEngine;

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

    private CinematicController _controller;

    public GameObject FocusTarget { get { return _focusTarget; } set { _focusTarget = value; } }
    public GameObject PlayerTarget { get { return _playerTarget;} set { _playerTarget = value; } }
    public GameObject CompanionTarget { get { return _companionTarget; } set { _companionTarget = value; } }

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

        _controller.PlayerCtx.CinematicEnter(_playerTarget.transform, _focusTarget.transform, _vcamera);
        _controller.CompanionCtx.TravelSetUpTalkBehaviour(_companionTarget.transform.position);

        CameraManager.Instance.SwitchGameVCamera(_vcamera);

        StartCoroutine(DoneAfterLenght());
    }

    public IEnumerator DoneAfterLenght() {
        yield return new WaitForSeconds(_shotLenght);

        _controller.OnInteract();
        Exit();

        yield break;
    }
}
