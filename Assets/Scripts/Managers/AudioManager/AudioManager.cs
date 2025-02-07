using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    private void PlaySFX(SFXTracks clip) {
        SFXSource.AddComponent<AudioSource>().playOnAwake = false;

        AudioSource audioSource = SFXSource.GetComponent<AudioSource>();
        audioSource.clip = _clipsDrawer.sFXTracks[(int)clip];
        audioSource.outputAudioMixerGroup = _mixer.outputAudioMixerGroup;

        StartCoroutine(PlayClip(audioSource));
        
    }

    private IEnumerator PlayClip(AudioSource source) {
        source.PlayOneShot(source.clip);

        yield return new WaitWhile(() => source.isPlaying);
        
        Destroy(source);
    }
}
