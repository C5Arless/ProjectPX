using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionAwakeTrigger : MonoBehaviour {
    [SerializeField] OptionsHandler _optionsHandler;
    [SerializeField] CanvasHandler _canvasHandler;

    [SerializeField] OptionsInfo _currentInfo;
    [SerializeField] OptionPayload _targetValue;

    private void OnDestroy() {
        UnsubscribeActions();
    }

    private void Start() {
        UpdateValues();
        SubscribeActions();

    }

    private void SubscribeActions() {
        _optionsHandler._onAudioDefaultAction += UpdateValues;
        _optionsHandler._onVideoDefaultAction += UpdateValues;

        _canvasHandler._onSwitchOptions += UpdateValues;
    }

    private void UnsubscribeActions() {
        _optionsHandler._onAudioDefaultAction -= UpdateValues;
        _optionsHandler._onVideoDefaultAction -= UpdateValues;

        _canvasHandler._onSwitchOptions -= UpdateValues;
    }

    private void UpdateValues() {
        switch (_targetValue) {
            case OptionPayload.MasterVolume: {
                Slider _slider = GetComponent<Slider>();
                _slider.value = _currentInfo.MasterVolume;                    
                break;
            }
            case OptionPayload.MusicVolume: {
                Slider _slider = GetComponent<Slider>();
                _slider.value = _currentInfo.MusicVolume;
                break;
            }
            case OptionPayload.EnvVolume: {
                Slider _slider = GetComponent<Slider>();
                _slider.value = _currentInfo.EnvVolume;
                break;
            }
            case OptionPayload.SfxVolume: {
                Slider _slider = GetComponent<Slider>();
                _slider.value = _currentInfo.SfxVolume;
                break;
            }
            case OptionPayload.Mute: {
                Toggle _toggle = GetComponent<Toggle>();
                bool _state;

                if (_currentInfo.Mute == 0) {
                    _state = false;
                } else {
                    _state = true;
                }

                _toggle.isOn = _state;
                break;
            }
            case OptionPayload.DisplayResolution: {
                    EvaluateDropDown();
                    break;
                }
            case OptionPayload.DisplayMode: {
                    TMP_Dropdown _dropdown = GetComponent<TMP_Dropdown>();
                    _dropdown.value = _currentInfo.DisplayMode;
                    break;
                }
            case OptionPayload.Quality: {
                    TMP_Dropdown _dropdown = GetComponent<TMP_Dropdown>();
                    _dropdown.value = _currentInfo.Quality;
                    break;
                }
            default: break;
        }
    }

    private void EvaluateDropDown() {
        TMP_Dropdown _dropdown = GetComponent<TMP_Dropdown>();
        int _targetValue = -1;

        switch (_currentInfo.DisplayResolution.x) {
            case 1920f: {
                    _targetValue = 0;
                    break;
                }
            case 1600f: {
                    _targetValue = 1;
                    break;
                }
            case 1366f: {
                    _targetValue = 2;
                    break;
                }
            case 1280f: {
                    _targetValue = 3;
                    break;
                }
            case 800f: {
                    _targetValue = 4;
                    break;
                }
            default: break;
        }

        if (_targetValue != -1) {
            _dropdown.value = _targetValue;
        }
    }
}
