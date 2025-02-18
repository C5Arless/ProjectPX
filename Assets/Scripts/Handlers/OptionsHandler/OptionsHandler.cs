using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour {
    [SerializeField] TMP_InputField inputField;

    [SerializeField] OptionsInfo _currentInfo;    

    private void Start() {
        SubscribeCallbacks();

        //Set initial options state
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

    public void DefaultVideo() {
        Debug.Log("Video options reset to default!");
    }

    public void DefaultAudio() {
        Debug.Log("Audio options reset to default!");
    }

    public void SelectName() {
        DataManager.Instance.PlayerInfo.Name = inputField.text;
    }

    public void HandleMasterVolume(Slider target) {
        _currentInfo.MasterVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.Master, (int)target.value);

        Debug.Log("Value: " + target.value);
    }

    public void HandleMusicVolume(Slider target) {
        _currentInfo.MusicVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.Music, (int)target.value);

        Debug.Log("Value: " + target.value);
    }

    public void HandleEnvVolume(Slider target) {
        _currentInfo.EnvVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.Environment, (int)target.value);

        Debug.Log("Value: " + target.value);
    }

    public void HandleSFXVolume(Slider target) {
        _currentInfo.SfxVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.SoundFX, (int)target.value);

        Debug.Log("Value: " + target.value);
    }

    public void HandleMuteVolume(Toggle target) {
        if (target.isOn) {
            _currentInfo.Mute = 1;
        } else {
            _currentInfo.Mute = 0;
        }

        AudioManager.Instance.MuteUnmuteVolume(target.isOn);

        Debug.Log("Value: " + target.isOn);
    }

    public void HandleDisplayResolution(TMP_Dropdown target) {
        string value = target.options[target.value].text;
        Vector2 resolution = new Vector2();
        string resX = value.Substring(0, 4);
        string resY;

        if (value.Length < 11) {
            resY = value.Substring(7, 3);
        } else {
            resY = value.Substring(7, 4);
        }
        
        resolution.x = int.Parse(resX);
        resolution.y = int.Parse(resY);

        _currentInfo.DisplayResolution = resolution;
    }

    public void HandleDisplayMode(TMP_Dropdown target) {
        _currentInfo.DisplayMode = target.value;
        Debug.Log("Value: " + target.value);
    }

    public void HandleQuality(TMP_Dropdown target) {
        _currentInfo.Quality = target.value;
        Debug.Log("Value: " + target.value);
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

}
