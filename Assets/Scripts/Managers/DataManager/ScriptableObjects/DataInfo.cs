using UnityEngine;

[CreateAssetMenu]
public class DataInfo : ScriptableObject {
    [SerializeField] private int _slotID;
    [SerializeField] private string _name;
    [SerializeField] private int _powerUps;
    [SerializeField] private int _score;
    [SerializeField] private int _currentHp;
    [SerializeField] private int _runtime;
    [Space]
    [SerializeField] private Vector2 _checkpoint;

    public int SlotID { get { return _slotID; } set { _slotID = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public int PowerUps { get { return _powerUps; } set { _powerUps = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    public int CurrentHp { get { return _currentHp; } set { _currentHp = value; } }
    public int Runtime { get { return _runtime; } set { _runtime = value; } }
    public Vector2 Checkpoint { get { return _checkpoint; } set { _checkpoint = value; } }

}
