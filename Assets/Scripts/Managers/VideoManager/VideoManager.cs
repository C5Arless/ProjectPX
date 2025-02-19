using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VideoManager : MonoBehaviour {
    public static VideoManager Instance;

    [SerializeField] UniversalRenderPipelineAsset[] _qualityAssets;
    [SerializeField] OptionsInfo _currentInfo;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void SetResolution(Vector2 target) {
        if (_currentInfo.DisplayResolution != target) {
            
            if (_currentInfo.DisplayMode != 0) {
                Screen.SetResolution((int)_currentInfo.DisplayResolution.x, (int)_currentInfo.DisplayResolution.y, false);
            } else {
                Screen.SetResolution((int)_currentInfo.DisplayResolution.x, (int)_currentInfo.DisplayResolution.y, true);
            }
        }
    }

    public void SetQuality(OptionQuality target) {
        if (QualitySettings.renderPipeline != _qualityAssets[(int)target]) {

            QualitySettings.SetQualityLevel((int)target, true);
            QualitySettings.renderPipeline = _qualityAssets[(int)target];
        }
    }

    public void SetDisplayMode(OptionDisplayMode target) {
        switch (target) {
            case OptionDisplayMode.Fullscreen: {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                }
            case OptionDisplayMode.Windowed: {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                }
            case OptionDisplayMode.Borderless: {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                }
            default: break;
        }
    }

    public void InitializeVideoSettings() {
        OptionDisplayMode _mode = (OptionDisplayMode)_currentInfo.DisplayMode;        
        OptionQuality _quality = (OptionQuality)_currentInfo.Quality;

        SetResolution(_currentInfo.DisplayResolution);
        SetDisplayMode(_mode);
        SetQuality(_quality);
    }
}
