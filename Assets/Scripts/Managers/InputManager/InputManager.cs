using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    [SerializeField] PlayerInput playerInput;

    public delegate void OnControlsChangedResolver();
    public OnControlsChangedResolver _controlsChangedResolver;

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

    public void SetActionMap(string target) {
        if (_currentActionMap != target) {
            StartCoroutine(EvaluateActionMap(target));
        }        
    }

    
    public void OnControlsChangedEvent(PlayerInput input) {
        if (playerInput.currentControlScheme == "Gamepad") {
            Cursor.lockState = CursorLockMode.Locked;

            if (_controlsChangedResolver != null) {
                _controlsChangedResolver();
            }

        } 
        else if (playerInput.currentControlScheme == "Keyboard&Mouse") {
            Cursor.lockState = CursorLockMode.None;

            if (_controlsChangedResolver != null) {
                _controlsChangedResolver();
            }
        }
    }
    

    public IEnumerator EvaluateActionMap(string target) {
        yield return null;

        playerInput.SwitchCurrentActionMap("Disabled");
        _currentActionMap = "Disabled";
        yield return null;

        playerInput.SwitchCurrentActionMap(target);
        _currentActionMap = target;
        yield break;
    }
}