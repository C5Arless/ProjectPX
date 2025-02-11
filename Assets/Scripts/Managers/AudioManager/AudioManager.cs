using System.Collections;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

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

    public void HandleSignal(int signal) {
        Debug.Log("Handling Signal!");
        
        if (signal == 3) {
            MenuController.Instance.ActivateIntroLights();
            PlaySFX(SFXTracks.Spotlight);
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
        AudioSource audioSource = MusicSource.GetComponent<AudioSource>();
        PlayableDirector playbackTrack = MusicSource.GetComponent<PlayableDirector>();

        if (audioSource != null) {
            audioSource.clip = _clipsDrawer.musicTracks[(int)track].track;
            playbackTrack.playableAsset = _clipsDrawer.musicTracks[(int)track].timeline;
            audioSource.volume = .15f;

            playbackTrack.Play();
            audioSource.Play();
        }
    }

    private IEnumerator PlayClipOnce(AudioSource source) {
        source.PlayOneShot(source.clip);

        yield return new WaitWhile(() => source.isPlaying);
        
        Destroy(source);
    }

    private IEnumerator PlayMusicTrack(AudioSource source) {
        
        
        yield return null;
    }
}
