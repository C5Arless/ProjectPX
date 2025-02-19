using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour {
    [SerializeField] TMP_InputField inputField;

    [SerializeField] OptionsInfo _currentInfo;

    public delegate void OnAudioDefaultAction();
    public OnAudioDefaultAction _onAudioDefaultAction;

    public delegate void OnVideoDefaultAction();
    public OnVideoDefaultAction _onVideoDefaultAction;

    private void Start() {
        SubscribeCallbacks();

    }

    private void OnDestroy() {
        UnsubscribeCallbacks();
    }

    private void SubscribeCallbacks() {
        InputManager.Instance._controlsChangedResolver += ControlsChanged;

        inputField.onValueChanged.AddListener(KeyboardSound);

        _onAudioDefaultAction += DefaultAction;
        _onVideoDefaultAction += DefaultAction;
    }

    private void UnsubscribeCallbacks() {
        InputManager.Instance._controlsChangedResolver -= ControlsChanged;

        _onAudioDefaultAction -= DefaultAction;
        _onVideoDefaultAction -= DefaultAction;
    }

    private void DefaultAction() {
        //
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
        DataManager.Instance.DefaultOption(OptionPayload.DisplayResolution);
        DataManager.Instance.DefaultOption(OptionPayload.DisplayMode);
        DataManager.Instance.DefaultOption(OptionPayload.Quality);

        //VideoBehaviour

        _onVideoDefaultAction();

        Debug.Log("Video options reset to default!");
    }

    public void DefaultAudio() {
        DataManager.Instance.DefaultOption(OptionPayload.MasterVolume);
        DataManager.Instance.DefaultOption(OptionPayload.MusicVolume);
        DataManager.Instance.DefaultOption(OptionPayload.EnvVolume);
        DataManager.Instance.DefaultOption(OptionPayload.SfxVolume);
        DataManager.Instance.DefaultOption(OptionPayload.Mute);

        AudioManager.Instance.InitializeMixerVolumes();

        _onAudioDefaultAction();

        Debug.Log("Audio options reset to default!");
    }

    public void SelectName() {
        DataManager.Instance.PlayerInfo.Name = inputField.text;
    }

    public void HandleMasterVolume(Slider target) {
        if (_currentInfo.Mute == 1) {            
            _currentInfo.MasterVolume = (int)target.value;
            DataManager.Instance.ApplyCurrentOption(OptionPayload.MasterVolume);

        } else {
            _currentInfo.MasterVolume = (int)target.value;

            AudioManager.Instance.ChangeVolume(AudioManagerMixer.Master, (int)target.value);

            DataManager.Instance.ApplyCurrentOption(OptionPayload.MasterVolume);

            Debug.Log("Value: " + target.value);

        }

    }

    public void HandleMusicVolume(Slider target) {
        _currentInfo.MusicVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.Music, (int)target.value);

        DataManager.Instance.ApplyCurrentOption(OptionPayload.MusicVolume);

        Debug.Log("Value: " + target.value);
    }

    public void HandleEnvVolume(Slider target) {
        _currentInfo.EnvVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.Environment, (int)target.value);

        DataManager.Instance.ApplyCurrentOption(OptionPayload.MusicVolume);

        Debug.Log("Value: " + target.value);
    }

    public void HandleSFXVolume(Slider target) {
        _currentInfo.SfxVolume = (int)target.value;

        AudioManager.Instance.ChangeVolume(AudioManagerMixer.SoundFX, (int)target.value);

        DataManager.Instance.ApplyCurrentOption(OptionPayload.SfxVolume);

        Debug.Log("Value: " + target.value);
    }

    public void HandleMuteVolume(Toggle target) {
        if (target.isOn) {
            _currentInfo.Mute = 1;
        } else {
            _currentInfo.Mute = 0;
        }

        AudioManager.Instance.MuteUnmuteVolume(target.isOn);

        DataManager.Instance.ApplyCurrentOption(OptionPayload.Mute);

        Debug.Log("Value: " + target.isOn);
    }

    public void HandleDisplayResolution(TMP_Dropdown target) {
        string value = target.options[target.value].text;
        Vector2 resolution = new Vector2();
        string resX;
        string resY;

        if (value.Length == 11) {
            resX = value.Substring(0, 4);
            resY = value.Substring(7, 4);
        }
        else if (value.Length == 10) {
            resX = value.Substring(0, 4);
            resY = value.Substring(7, 3);
        }
        else {
            resX = value.Substring(0, 3);
            resY = value.Substring(6, 3);
        }

        resolution.x = int.Parse(resX);
        resolution.y = int.Parse(resY);

        _currentInfo.DisplayResolution = resolution;

        //VideoBehaviour
        VideoManager.Instance.SetResolution(_currentInfo.DisplayResolution);

        DataManager.Instance.ApplyCurrentOption(OptionPayload.DisplayResolution);
    }

    public void HandleDisplayMode(TMP_Dropdown target) {
        _currentInfo.DisplayMode = target.value;

        DataManager.Instance.ApplyCurrentOption(OptionPayload.DisplayMode);

        //VideoBehaviour
        VideoManager.Instance.SetDisplayMode((OptionDisplayMode)_currentInfo.DisplayMode);

        Debug.Log("Value: " + target.value);
    }

    public void HandleQuality(TMP_Dropdown target) {
        _currentInfo.Quality = target.value;

        DataManager.Instance.ApplyCurrentOption(OptionPayload.Quality);

        //VideoBehaviour
        VideoManager.Instance.SetQuality((OptionQuality)_currentInfo.Quality);

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
