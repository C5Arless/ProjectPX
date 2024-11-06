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
