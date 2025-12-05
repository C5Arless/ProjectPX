using UnityEngine;

public class SwitchSprite : MonoBehaviour {
    [SerializeField] private Sprite gamePad;
    [SerializeField] private Sprite keyboard;
    [SerializeField] private SpriteRenderer button;

    private void Update() {
        SwitchSpriteRenderer();
    }

    private void SwitchSpriteRenderer() {
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
