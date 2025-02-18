using System.Collections;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] AudioMixer _mixer;

    [SerializeField] GameObject MusicSource;
    [SerializeField] GameObject EnvSource;
    [SerializeField] GameObject SFXSource;

    [SerializeField] AudioClipsDrawer _clipsDrawer;

    [SerializeField] OptionsInfo _currentInfo;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    private void Start() {
        //Set initial volume to mixer
        InitializeMixerVolumes();
    }

    public void PlayKeyboardSound() {
        int clipID = Random.Range(0, 4);
        SFXTracks sFXTracks = (SFXTracks)clipID;

        PlaySFX(sFXTracks);
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
                    MenuController.Instance.CallIntroRoutine();
                    break;
                }
            case 1: {                    
                    break;
                }
            case 2: {
                    MenuController.Instance.ShowHideLogo();
                    PlayMusic(MusicTracks.MainMenu_Loop);
                    InputManager.Instance.SetActionMap("MainScreen");
                    break;
                }
            case 3: {
                    MenuController.Instance.ActivateIntroLights();
                    PlaySFX(SFXTracks.Spotlight);
                    break; 
                }
            default:  break; 
        }
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
        if (_clipsDrawer.musicTracks[(int)track].timeline != null) {
            AudioSource audioSource = MusicSource.GetComponent<AudioSource>();
            PlayableDirector playbackTrack = MusicSource.GetComponent<PlayableDirector>();

            if (audioSource != null) {
                audioSource.clip = _clipsDrawer.musicTracks[(int)track].track;
                playbackTrack.playableAsset = _clipsDrawer.musicTracks[(int)track].timeline;
                audioSource.volume = 1f;

                playbackTrack.Play();
                audioSource.Play();
            }        

        } else {
            AudioSource audioSource = MusicSource.GetComponent<AudioSource>();           

            if (audioSource != null) {
                audioSource.clip = _clipsDrawer.musicTracks[(int)track].track;
                audioSource.loop = true;
                audioSource.volume = 1f;

                audioSource.Play();
            }
        }

    }

    private void InitializeMixerVolumes() {
        float masterValue = (_currentInfo.MasterVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.MasterVolume.ToString(), Mathf.Log10(masterValue) * 20f);

        float musicValue = (_currentInfo.MusicVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.MusicVolume.ToString(), Mathf.Log10(musicValue) * 20f);

        float envValue = (_currentInfo.EnvVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.EnvVolume.ToString(), Mathf.Log10(envValue) * 20f);

        float sfxValue = (_currentInfo.SfxVolume + .001f) / 10f;
        _mixer.SetFloat(OptionPayload.SfxVolume.ToString(), Mathf.Log10(sfxValue) * 20f);
    }

    private IEnumerator PlayClipOnce(AudioSource source) {
        source.PlayOneShot(source.clip);

        yield return new WaitWhile(() => source.isPlaying);
        
        Destroy(source);
    }

    private IEnumerator PlayMusicTrack(AudioSource source, PlayableDirector playback) {
        
        
        yield return null;
    }
}
