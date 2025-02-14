using System.Collections;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
//using System;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] AudioMixer _mixer;

    [SerializeField] GameObject masterSource;
    [SerializeField] GameObject MusicSource;
    [SerializeField] GameObject SFXSource;

    [SerializeField] AudioClipsDrawer _clipsDrawer;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
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
                    PlayMusic(MusicTracks.MainMenu_Loop);
                    break;
                }
            case 3: {
                    MenuController.Instance.ActivateIntroLights();
                    PlaySFX(SFXTracks.Spotlight);
                    break; 
                }
            default: { break; }
        }
    }

    private void PlaySFX(SFXTracks clip) {
        SFXSource.AddComponent<AudioSource>().playOnAwake = false;

        AudioSource audioSource = SFXSource.GetComponent<AudioSource>();
        audioSource.clip = _clipsDrawer.sFXTracks[(int)clip];
        audioSource.outputAudioMixerGroup = _mixer.outputAudioMixerGroup;
        audioSource.volume = .05f;

        StartCoroutine(PlayClipOnce(audioSource));
        
    }

    public void PlayMusic(MusicTracks track) {
        if (_clipsDrawer.musicTracks[(int)track].timeline != null) {
            AudioSource audioSource = MusicSource.GetComponent<AudioSource>();
            PlayableDirector playbackTrack = MusicSource.GetComponent<PlayableDirector>();

            if (audioSource != null) {
                audioSource.clip = _clipsDrawer.musicTracks[(int)track].track;
                playbackTrack.playableAsset = _clipsDrawer.musicTracks[(int)track].timeline;
                audioSource.volume = .15f;

                playbackTrack.Play();
                audioSource.Play();
            }        

        } else {
            AudioSource audioSource = MusicSource.GetComponent<AudioSource>();           

            if (audioSource != null) {
                audioSource.clip = _clipsDrawer.musicTracks[(int)track].track;
                audioSource.loop = true;
                audioSource.volume = .15f;

                audioSource.Play();
            }
        }

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
