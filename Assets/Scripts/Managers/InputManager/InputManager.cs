using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    [SerializeField] PlayerInput playerInput;

    private string _currentActionMap;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
        }
        else { Destroy(gameObject); }

        _currentActionMap = playerInput.defaultActionMap;
    }

    public void StartGame() {
        SetActionMap("Player");
    }

    public PlayerInput GetPlayerInput() {
        return playerInput;
    }

    public string GetActionMap() {
        return _currentActionMap;
    }

    /*
    public void OnAnyButton(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {
            SetActionMap("UI");
        }
    }
    */

    public void SetActionMap(string target) {
        if (_currentActionMap != target) {
            playerInput.SwitchCurrentActionMap(target);
            _currentActionMap = target;
        }        
    }
}