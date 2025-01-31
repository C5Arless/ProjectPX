using UnityEngine;

[CreateAssetMenu]
public class RecordInfo : ScriptableObject {
    [SerializeField] private string _name = "Default";    
    [SerializeField] private int _score;

    public string Name { get { return _name; } set { _name = value; } }
    public int Score { get { return _score; } set { _score = value; } }

}

