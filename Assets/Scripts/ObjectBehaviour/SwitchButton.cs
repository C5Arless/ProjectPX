using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour {
    [SerializeField] private Sprite gamePad;
    [SerializeField] private Sprite keyboard;
    [SerializeField] private Image button;

    private void Update() {
        SwitchSprite();
    }

    private void SwitchSprite() {
        var playerInput = InputManager.Instance?.GetPlayerInput();

        if (playerInput is null || button is null) return;
        
        if (playerInput.currentControlScheme == "Keyboard&Mouse") {
            button.sprite = keyboard;
        }
        else {
            button.sprite = gamePad;
        }
    }
}
