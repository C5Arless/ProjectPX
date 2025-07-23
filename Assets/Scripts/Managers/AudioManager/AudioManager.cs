using System.Collections;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] AudioMixer _mixer;

    [SerializeField] GameObject MusicSource;
    [SerializeField] GameObject EnvSource;
    [SerializeField] GameObject SFXSource;
    [SerializeField] GameObject VoiceSource;

    [SerializeField] AudioClipsDrawer _clipsDrawer;

    [SerializeField] OptionsInfo _currentInfo;

    [SerializeField] AudioLowPassFilter _lowPassFilter;

    private AudioSource _currentMusicSource;
    private PlayableDirector _currentPlaybackTrack;

    private bool _musicPlaying;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start() {
        _currentMusicSource = MusicSource.GetComponent<AudioSource>();
        _currentPlaybackTrack = MusicSource.GetComponent<PlayableDirector>();
    }

    public void OnPauseEnterFilter() {
        StartCoroutine(PauseEnterRoutine());
    }

    public void OnPauseExitFilter() {
        StartCoroutine(PauseExitRoutine());
    }

    public void PlayKeyboardSound() {
        int clipID = Random.Range(0, 4);
        SFXTracks sFXTracks = (SFXTracks)clipID;

        PlaySFX(sFXTracks);
    } 

    public void PlayVoice(VoiceName name, VoiceMood mood) {
        int clipID = Random.Range(14, 19);
        SFXTracks voiceTracks = (SFXTracks)clipID;

        PlayVoiceSample(voiceTracks, name, mood);
    }

    public void StopVoice() {
        AudioSource voiceSource = VoiceSource.GetComponent<AudioSource>();

        if (voiceSource != null) {
            voiceSource.Stop();
        }
    }

    public void HandleSignal(string signal) {
        Vector2 _target = new Vector2();
        string _signalX = signal.Substring(0, signal.Length - 1);
        string _signalY = signal.Substring(signal.Length - 1, 1);
        _target.x = int.Parse(_signalX);
        _target.y = int.Parse(_signalY);

        Debug.Log("Handling Signal! [" + _target.x + ", " + _target.y + "]");

        EvaluateSignal(_target);

    }

    public void ChangeVolume(AudioManagerMixer mixer, int value) {
        switch (mixer) {
            case AudioManagerMixer.Master: {
                    float masterValue = (value + .001f) / 10f;
                    _mixer.SetFloat(OptionPayload.MasterVolume.ToString(), Mathf.Log10(masterValue) * 20f);
                    break;
                }
            case AudioManagerMixer.Music: {
                    float musicValue = (value + .001f) / 10f;
                    _mixer.SetFloat(OptionPayload.MusicVolume.ToString(), Mathf.Log10(musicValue) * 20f);
                    break;
                }
            case AudioManagerMixer.Environment: {
                    float envValue = (value + .001f) / 10f;
                    _mixer.SetFloat(OptionPayload.EnvVolume.ToString(), Mathf.Log10(envValue) * 20f);
                    break;
                }
            case AudioManagerMixer.SoundFX: {
                    float sfxValue = (value + .001f) / 10f;
                    _mixer.SetFloat(OptionPayload.SfxVolume.ToString(), Mathf.Log10(sfxValue) * 20f);
                    break;
                }
            default: break;
        }
    }

    public void MuteUnmuteVolume(bool toggle) {
        if (!toggle) {
            float masterValue = (_currentInfo.MasterVolume + .001f) / 10f;
            _mixer.SetFloat(OptionPayload.MasterVolume.ToString(), Mathf.Log10(masterValue) * 20f);
        } else {
            _mixer.SetFloat(OptionPayload.MasterVolume.ToString(), -80f);
        }
    }

    private void EvaluateSignal(Vector2 target) {
        switch ((int)target.x) {
            case 0: {
                    OnIntroSignal((int)target.y);
                    break;
                }
            default: { break; }
        }
    }

    private void OnIntroSignal(int target) {
        switch (target) {
            case 0: {
                    GameBucket.Instance.MenuCtx.CallIntroRoutine();
                    break;
                }
            case 1: {                    
                    break;
                }
            case 2: {
                    GameBucket.Instance.MenuCtx.ShowHideLogo();
                    PlayMusicNow(MusicTracks.MainMenu_Loop);
                    InputManager.Instance.SetActionMap("MainScreen");
                    break;
                }
            case 3: {
                    GameBucket.Instance.MenuCtx.ActivateIntroLights();
                    PlaySFX(SFXTracks.Spotlight);
                    break; 
                }
            default:  break; 
        }
    }

    private void PlayVoiceSample(SFXTracks clip, VoiceName name, VoiceMood mood) {
        AudioSource audioSource = VoiceSource.GetComponent<AudioSource>();

        if (audioSource != null) {
            audioSource.Stop();
        }

        AudioSource voiceSource = VoiceSource.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.clip = _clipsDrawer.sFXTracks[(int)clip];
        voiceSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master/SFX")[0];
        voiceSource.pitch = GetVoiceName(name) + GetVoiceMood(mood);
        voiceSource.volume = 1f;

        StartCoroutine(PlayVoiceClip(voiceSource));
    }

    public void PlaySFX(SFXTracks clip) {
        SFXSource.AddComponent<AudioSource>().playOnAwake = false;

        AudioSource audioSource = SFXSource.GetComponent<AudioSource>();
        audioSource.clip = _clipsDrawer.sFXTracks[(int)clip];
        audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master/SFX")[0];
        audioSource.volume = 1f;

        StartCoroutine(PlayClipOnce(audioSource));
        
    }

    public void PlayMusic(MusicTracks track) {
        if (_clipsDrawer.musicTracks[(int)track].track == null) { return; }

        StartCoroutine(PlayMusicTrack((int)track));
    }

    public void InitializeMixerVolumes() {
        float masterValue = (_currentInfo.MasterVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.MasterVolume.ToString(), Mathf.Log10(masterValue) * 20f);

        float musicValue = (_currentInfo.MusicVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.MusicVolume.ToString(), Mathf.Log10(musicValue) * 20f);

        float envValue = (_currentInfo.EnvVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.EnvVolume.ToString(), Mathf.Log10(envValue) * 20f);

        float sfxValue = (_currentInfo.SfxVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.SfxVolume.ToString(), Mathf.Log10(sfxValue) * 20f);
    }

    public void PlayMusicNow(MusicTracks track) {
        if (_clipsDrawer.musicTracks[(int)track].track == null) { return; }

        if (_clipsDrawer.musicTracks[(int)track].timeline != null) {
            _currentPlaybackTrack.Stop();
            _currentMusicSource.Stop();

            _currentMusicSource.clip = _clipsDrawer.musicTracks[(int)track].track;            
            _currentMusicSource.volume = 1f;
            _currentPlaybackTrack.playableAsset = _clipsDrawer.musicTracks[(int)track].timeline;
            _currentPlaybackTrack.time = 0f;

            _currentPlaybackTrack.Play();
            _currentMusicSource.Play();
            _musicPlaying = true;

        } else {
            _currentPlaybackTrack.Stop();
            _currentMusicSource.Stop();

            _currentMusicSource.clip = _clipsDrawer.musicTracks[(int)track].track;
            _currentMusicSource.loop = true;
            _currentMusicSource.volume = 1f;
        
            _currentMusicSource.Play();        
            _musicPlaying = true;
        }
    }

    public void StopMusic() {
        StartCoroutine(StopMusicTrack());
    }

    private float GetVoiceMood(VoiceMood mood) {
        switch (mood) {
            case VoiceMood.Sad: {
                    return -.15f;
                }
            case VoiceMood.Scared: {
                    return .15f;
                }
            case VoiceMood.Angry: {
                    return .03f;
                }
            case VoiceMood.Excited: {
                    return .05f;
                }
            default: {
                    return 0f;
                }
        }
    }

    private float GetVoiceName(VoiceName name) {
        switch (name) {
            case VoiceName.Companion: {
                    return 1.5f;
                }
            case VoiceName.NPC: {
                    return .5f;
                }
            default: {
                    return 1f;
                }
        }
    }

    private IEnumerator PlayClipOnce(AudioSource source) {
        source.PlayOneShot(source.clip);

        yield return new WaitWhile(() => source.isPlaying);
        
        Destroy(source);
    }

    private IEnumerator PlayVoiceClip(AudioSource source) {
        source.PlayOneShot(source.clip);

        yield return new WaitWhile(() => source.isPlaying);

        Destroy(source);
    }

    private IEnumerator PlayMusicTrack(int target) {                
        if (_clipsDrawer.musicTracks[target].timeline != null) {
            _currentPlaybackTrack.Stop();
            _currentMusicSource.Stop();

            _currentMusicSource.clip = _clipsDrawer.musicTracks[target].track;
            _currentMusicSource.volume = 0f;
            _currentPlaybackTrack.playableAsset = _clipsDrawer.musicTracks[target].timeline;
            _currentPlaybackTrack.time = 0f;

            _currentPlaybackTrack.Play();
            _currentMusicSource.Play();

            _musicPlaying = true;

            while (_currentMusicSource.volume < 1f) {
                _currentMusicSource.volume += .05f;
                yield return null;
            }

            _currentMusicSource.volume = 1f;
            yield return null;

        } else {
            _currentPlaybackTrack.Stop();
            _currentMusicSource.Stop();

            _currentMusicSource.clip = _clipsDrawer.musicTracks[target].track;
            _currentMusicSource.volume = 0f;
            _currentMusicSource.loop = true;

            _currentMusicSource.Play();

            _musicPlaying = true;

            while (_currentMusicSource.volume < 1f) {
                _currentMusicSource.volume += .05f;
                yield return null;
            }

            _currentMusicSource.volume = 1f;
            yield return null;

        }

        yield break;
    }

    private IEnumerator StopMusicTrack() {
        if (_musicPlaying) {
            while (_currentMusicSource.volume > .02f) {
                _currentMusicSource.volume -= .05f;
                yield return null;
            }
            _musicPlaying = false;
            _currentMusicSource.volume = 0f;
        }

        _currentPlaybackTrack.Stop();
        _currentMusicSource.Stop();
        yield break;
    }

    private IEnumerator PauseEnterRoutine() {
        if (_musicPlaying) { 
            while (_currentMusicSource.volume > .6f) {
                _currentMusicSource.volume -= .1f;
                _lowPassFilter.cutoffFrequency -= 4750f;
                yield return null;
            }

            _currentMusicSource.volume = .6f;
            _lowPassFilter.cutoffFrequency = 3000f;

        }

        yield break;
    }

    private IEnumerator PauseExitRoutine() {
        if (_musicPlaying) {
            while (_currentMusicSource.volume < 1f) {
                _currentMusicSource.volume += .1f;
                _lowPassFilter.cutoffFrequency += 4750f;
                yield return null;
            }

            _currentMusicSource.volume = 1f;
            _lowPassFilter.cutoffFrequency = 22000f;

        }

        yield break;
    }
}
