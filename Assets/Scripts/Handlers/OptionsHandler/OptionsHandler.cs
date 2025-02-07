using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        inputField.onValueChanged.AddListener(KeyboardSound);
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

    private void KeyboardSound(string text) {
        AudioManager.Instance.PlayKeyboardSound();        
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

    public void SelectName() {
        DataManager.Instance.PlayerInfo.Name = inputField.text;
    }

    public void HandleMasterVolume(Slider target) {
        Debug.Log("Value: " + target.value);
    }

    public void HandleMusicVolume(Slider target) {
        Debug.Log("Value: " + target.value);
    }

    public void HandleSFXVolume(Slider target) {
        Debug.Log("Value: " + target.value);
    }

    public void HandleMuteVolume(Toggle target) {
        Debug.Log("Value: " + target.isOn);
    }

    public void HandleDisplayResolution(TMP_Dropdown target) {
        Debug.Log("Value: " + target.value);
    }

    public void HandleDisplayMode(TMP_Dropdown target) {
        Debug.Log("Value: " + target.value);
    }

    public void HandleQuality(TMP_Dropdown target) {
        Debug.Log("Value: " + target.value);
    }
}
