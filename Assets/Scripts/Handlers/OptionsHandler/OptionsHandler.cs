using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsHandler : MonoBehaviour {
    [SerializeField] TMP_InputField inputField;

    private void Start() {
        SubscribeCallbacks();
    }

    private void OnDestroy() {
        UnsubscribeCallbacks();
    }

    private void SubscribeCallbacks() {
        InputManager.Instance._controlsChangedResolver += ControlsChanged;
    }

    private void UnsubscribeCallbacks() {
        InputManager.Instance._controlsChangedResolver -= ControlsChanged;
    }

    private void ControlsChanged() {
        if (InputManager.Instance.GetPlayerInput().currentControlScheme == "Gamepad") {
            inputField.interactable = false;            
        } else {
            inputField.interactable = true;
        }
    }

    public void AddText(TMP_Text text) {
        if (inputField.text.Length < inputField.characterLimit) {
            inputField.text += text.text;

        }
    }

    public void BackspaceText() {
        if (inputField != null && inputField.text.Length > 0) {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
