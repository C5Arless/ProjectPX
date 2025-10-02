using UnityEngine;

public interface ISpawnable {
    public Vector3 Position { get; }
    public bool IsReady { get; set; }
    public bool IsSpawning { get; set; }
    public bool IsRunning { get; }

    void Spawn();    
}