using UnityEngine;

public class GameBucket : MonoBehaviour {
    public static GameBucket Instance { get; private set; }

    private PXController _playerCtx;
    private CompanionController _companionCtx;
    private GameCanvasHandler _gameCanvasHandler;

    public PXController PXController { get { return _playerCtx; } set { _playerCtx = value; } }
    public CompanionController CompanionCtx { get {  return _companionCtx; } set { _companionCtx = value; } }
    public GameCanvasHandler GameCanvasHandler { get { return _gameCanvasHandler; } set { _gameCanvasHandler = value; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

}
