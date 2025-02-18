using UnityEngine;
using UnityEngine.Timeline;

[System.Serializable]
public struct InteractionVCameras {
    public GameObject vcamera;
    public int pageIdx;
}

[System.Serializable]
public struct CinematicVCameras {
    public GameObject vcamera;    
    public int shotIdx;
    public float lenght;
    public bool hasDolly;
}

[System.Serializable]
public struct AudioClipsDrawer {
    public MusicDrawer[] musicTracks;
    public AudioClip[] envTracks;
    public AudioClip[] sFXTracks;
}

[System.Serializable]
public struct MusicDrawer {
    public AudioClip track;
    public TimelineAsset timeline;
}

