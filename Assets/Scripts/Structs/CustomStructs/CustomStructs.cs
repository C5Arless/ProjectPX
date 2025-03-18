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

[System.Serializable]
public struct DialogPages {
    public int dialogIdx;
    public int pageIdx;
}

[System.Serializable]
public struct DialogObject {
    public int IDX;
    public string Name;
    public string Mood;
    public string Line;
    public string[] Options;
    public int NEXT;
}

public class DialogWrapper {
    public DialogObject[] Dialogues;
}
