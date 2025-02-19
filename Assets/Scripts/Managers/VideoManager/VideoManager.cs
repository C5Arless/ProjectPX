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
        if (_currentInfo.DisplayMode != 0) {
            Screen.SetResolution((int)target.x, (int)target.y, false);
        }
        else {
            Screen.SetResolution((int)target.x, (int)target.y, true);
        }
    }

    public void SetQuality(OptionQuality target) {
        if (QualitySettings.renderPipeline != _qualityAssets[(int)target]) {
            QualitySettings.SetQualityLevel((int)target, true);

            StartCoroutine(SetQualityAsset(target));                      
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
            default: break;
        }
    }

    public void InitializeVideoSettings() {        
        SetResolution(_currentInfo.DisplayResolution);        

        OptionDisplayMode _mode = (OptionDisplayMode)_currentInfo.DisplayMode;
        SetDisplayMode(_mode);        

        OptionQuality _quality = (OptionQuality)_currentInfo.Quality;
        SetQuality(_quality);               
    }

    private IEnumerator SetQualityAsset(OptionQuality target) {
        yield return new WaitForEndOfFrame();

        QualitySettings.renderPipeline = _qualityAssets[(int)target];
        yield break;
    }
}
