using UnityEngine;

[CreateAssetMenu]
public class UI_Info : ScriptableObject {
    [SerializeField] private UIMode _UIMode;    

    public UIMode UIMode { get { return _UIMode; } set { _UIMode = value; } }    

}

