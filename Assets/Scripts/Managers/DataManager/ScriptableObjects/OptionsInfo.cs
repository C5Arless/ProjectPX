using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class OptionsInfo : ScriptableObject {
    private int _masterVolume;
    private int _musicVolume;
    private int _envVolume;
    private int _sfxVolume;
    private int _mute;

    private Vector2 _displayResolution;
    private int _displayMode;
    private int _quality;  

    public int MasterVolume { get { return _masterVolume; } set { _masterVolume = value; } }
    public int MusicVolume { get { return _musicVolume; } set { _musicVolume = value; } }
    public int EnvVolume { get { return _envVolume; } set { _envVolume = value; } }
    public int SfxVolume { get { return _sfxVolume; } set { _sfxVolume = value; } }
    public int Mute { get { return _mute; } set { _mute = value; } }

    public Vector2 DisplayResolution { get { return _displayResolution; } set { _displayResolution = value; } }
    public int DisplayMode { get { return _displayMode; } set { _displayMode = value; } }
    public int Quality { get { return _quality; } set { _quality = value; } }

}

