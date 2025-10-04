using UnityEngine;
using System.Collections.Generic;

public class GameBucket : MonoBehaviour {
    public static GameBucket Instance { get; private set; }

    private PXController _playerCtx;
    private CompanionController _companionCtx;
    private MenuController _menuCtx;
    private EventsOrchestrator _eventsOrchestrator;
    private SpawnHandler spawnHandler;

    private GameCanvasHandler _gameCanvasHandler;
    private DialogObject[] _dialogues;
    private List<DialogData> _dialogData = new List<DialogData>();

    private VoiceMood[] _moodMask = new VoiceMood[5];
    private VoiceName[] _nameMask = new VoiceName[2];

    public PXController PXController { get { return _playerCtx; } set { _playerCtx = value; } }
    public CompanionController CompanionCtx { get {  return _companionCtx; } set { _companionCtx = value; } }
    public MenuController MenuCtx { get { return _menuCtx; } set { _menuCtx = value; } }
    public EventsOrchestrator EventsOrchestrator { get { return _eventsOrchestrator; } set { _eventsOrchestrator = value; } }
    public SpawnHandler SpawnHandler { get { return spawnHandler; } set { spawnHandler = value; } }
    public GameCanvasHandler GameCanvasHandler { get { return _gameCanvasHandler; } set { _gameCanvasHandler = value; } }  

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        InitializeMasks();
    }

    private void Start() {
        RetrieveDialogObject();
        
    }

    public DialogObject GetDialogObject(int IDX) {
        return _dialogues[IDX];
    }

    public DialogData GetDialogData(int IDX) {
        return _dialogData[IDX];
    }

    private void RetrieveDialogData(DialogObject[] dialogues) {
        foreach (DialogObject dialog in dialogues) {
            DialogData target = new DialogData();
            
            VoiceMood targetMood = RetrieveMood(dialog);
            VoiceName targetName = RetrieveName(dialog);
            target.Mood = targetMood;
            target.Name = targetName;            
            target.IDX = dialog.IDX;

            _dialogData.Add(target);
        }
    }

    private void RetrieveDialogObject() {        
        DialogWrapper dialogWrapper = DataManager.Instance.GetDialogObj();
        _dialogues = dialogWrapper.Dialogues;
        RetrieveDialogData(_dialogues);
    }

    private VoiceMood RetrieveMood(DialogObject dialog) {
        foreach (VoiceMood _mask in _moodMask) {
            if (_mask.ToString() == dialog.Mood) {
                return _mask;
            }
        }
        return VoiceMood.Default;
    }

    private VoiceName RetrieveName(DialogObject dialog) {
        foreach (VoiceName _mask in _moodMask) {
            if (_mask.ToString() == dialog.Tag) {
                return _mask;
            }
        }
        return VoiceName.Default;
    }

    private void InitializeMasks() {
        for (int i = 0; i <= _moodMask.Length - 1; i++) {
            _moodMask[i] = (VoiceMood)i;
        }

        for (int i = 0; i <= _nameMask.Length - 1; i++) {
            _nameMask[i] = (VoiceName)i;
        }
    }
}
