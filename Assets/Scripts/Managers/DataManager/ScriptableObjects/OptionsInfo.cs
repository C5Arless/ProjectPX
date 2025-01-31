using UnityEngine;

[CreateAssetMenu]
public class OptionsInfo : ScriptableObject {
    private int _masterVolume;
    private int _musicVolume;
    private int _sfxVolume;
    private bool _mute;

    private int _displayResolution;
    private int _displayMode;
    private int _quality;


    public int MasterVolume { get { return _masterVolume; } set { _masterVolume = value; } }
    public int MusicVolume { get { return _musicVolume; } set { _musicVolume = value; } }
    public int SfxVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    public bool Mute { get { return _mute; } set { _mute = value; } }

    public int DisplayResolution { get { return _displayResolution; } set { _displayResolution = value; } }
    public int DisplayMode { get { return _displayMode; } set { _displayMode = value; } }
    public int Quality { get { return _quality; } set { _quality = value; } }

}

