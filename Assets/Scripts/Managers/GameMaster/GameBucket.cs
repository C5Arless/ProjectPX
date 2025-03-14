using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class GameBucket : MonoBehaviour {
    public static GameBucket Instance { get; private set; }

    private PXController _playerCtx;
    private CompanionController _companionCtx;

    private GameCanvasHandler _gameCanvasHandler;
    private DialogObject[] _dialogues;

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

    private void Start() {
        RetrieveDialogObjects();
    }

    public DialogObject GetDialogObject(int IDX) {
        return _dialogues[IDX];
    }

    private void RetrieveDialogObjects() {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues.json");

        if (File.Exists(path)) {
            string jsonContent = File.ReadAllText(path);            

            DialogWrapper dialogWrapper = JsonConvert.DeserializeObject<DialogWrapper>(jsonContent);
            _dialogues = dialogWrapper.Dialogues;
        }
        else {
            Debug.LogError("File JSON non trovato!");
        }
    }
}
