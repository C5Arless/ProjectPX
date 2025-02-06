using UnityEngine;

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
    public AudioClip[] musicTracks;
    public AudioClip[] sFXTracks;
}
