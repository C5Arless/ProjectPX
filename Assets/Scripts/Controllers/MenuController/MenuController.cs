using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour { 
    public static MenuController Instance { get; private set; }

    [SerializeField] GameObject[] slots;
    //[SerializeField] GameObject recordsPod;
    [Space]
    [SerializeField] DataInfo[] slotsInfo;
    [SerializeField] RecordInfo[] recordsInfo;
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] UI_Info _UIinfo;
    [Space]
    [SerializeField] CanvasHandler _canvasHandler;    

    private int currentSlot = 0;
    private int selectedSlot = 0;    

    private InputAction _navigateAction;
    private InputAction _submitAction;
    private InputAction _saveAction;
    private InputAction _quitAction;
    private InputAction _cancelAction;
    private InputAction _pauseCameraAction;
    private InputAction _resumeCameraAction;
    private InputAction _anyButtonAction;

    public CanvasHandler CanvasHandler { get { return _canvasHandler; } set { _canvasHandler = value; } }
    public int CurrentSlot { get {  return currentSlot; } set {  currentSlot = value; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        _UIinfo.UIMode = UIMode.MainScreen;
    }

    private void Start() {
        DataManager.Instance.RefreshData();
        StartCoroutine("DisplaySlots");
        //StartCoroutine("DisplayRecords");
        SelectLights();
        
        CameraManager.Instance.InitializeVCameras();

        InitializeActions();
        SubscribeCallbacks();
        
    }

    private void OnDestroy() {
        UnsubscribeCallbacks();
    }

    private void SubscribeCallbacks() {
        _navigateAction.started += OnNavigate;
        //_navigateAction.performed += OnNavigate;

        //_submitAction.started += OnSubmit;

        _saveAction.started += OnSave;

        _quitAction.started += OnQuit;

        _cancelAction.started += OnCancel;

        _pauseCameraAction.started += OnPauseCamera;

        _resumeCameraAction.started += OnResumeCamera;

        _anyButtonAction.started += OnAnyButton;

        _canvasHandler._onSaveSlotSelectButton += SelectSlots;
        _canvasHandler._onSaveSlotHighlightButton += HighlightSlots;

    }

    private void UnsubscribeCallbacks() {
        _navigateAction.started -= OnNavigate;
        //_navigateAction.performed -= OnNavigate;

        //_submitAction.started -= OnSubmit;

        _saveAction.started -= OnSave;

        _quitAction.started -= OnQuit;

        _cancelAction.started -= OnCancel;

        _pauseCameraAction.started -= OnPauseCamera;

        _resumeCameraAction.started -= OnResumeCamera;

        _anyButtonAction.started -= OnAnyButton;

        _canvasHandler._onSaveSlotSelectButton -= SelectSlots;
        _canvasHandler._onSaveSlotHighlightButton -= HighlightSlots;

    }

    private void InitializeActions() {
        //UI Actions
        _navigateAction = InputManager.Instance.GetPlayerInput().actions["Navigate"];
        _submitAction = InputManager.Instance.GetPlayerInput().actions["Submit"];
        _saveAction = InputManager.Instance.GetPlayerInput().actions["Save"];
        _quitAction = InputManager.Instance.GetPlayerInput().actions["Quit"];
        _cancelAction = InputManager.Instance.GetPlayerInput().actions["Cancel"];
        _resumeCameraAction = InputManager.Instance.GetPlayerInput().actions["Resume"];

        //Player Actions
        _pauseCameraAction = InputManager.Instance.GetPlayerInput().actions["Pause"];

        //MainScreen Actions
        _anyButtonAction = InputManager.Instance.GetPlayerInput().actions["AnyButton"];
    }

    public void OnNavigate(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {
            switch (_UIinfo.UIMode) {
                case UIMode.MainMenu: {
                        _canvasHandler.NavigateMenu(input.ReadValue<Vector2>());
                        break;
                    }
                case UIMode.Slots: {
                        _canvasHandler.NavigateMenu(input.ReadValue<Vector2>());
                        //NavigateSlot(input.ReadValue<Vector2>());                        
                        break;
                    }
                case UIMode.Pause: {
                        _canvasHandler.NavigateMenu(input.ReadValue<Vector2>());
                        //NavigatePause(input.ReadValue<Vector2>());
                        break;
                    }
            }        
        }        
    }

    public void OnSubmit(InputAction.CallbackContext input) {
        if (InputManager.Instance.GetPlayerInput().currentActionMap.ToString() != "UI") { return; }

        if (input.phase == InputActionPhase.Started) { 
            switch (_UIinfo.UIMode) {
                case UIMode.MainScreen: {
                        //SubmitMainScreen();
                        break;
                    }
                case UIMode.MainMenu: {
                        //SubmitMenu();
                        break;
                    }
                case UIMode.Slots: {
                        //SelectSlot();
                        break;
                    }
                case UIMode.Pause: {
                        //SubmitPause()
                        break;
                    }
            }        
        }        
    }

    public void OnSave(InputAction.CallbackContext input) {
        if (_UIinfo.UIMode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            SaveGame();
        }
    }

    public void OnQuit(InputAction.CallbackContext input) {
        if (_UIinfo.UIMode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            QuitGame();
        }
    }

    public void OnCancel(InputAction.CallbackContext input) {
        if (_UIinfo.UIMode != UIMode.Records && _UIinfo.UIMode != UIMode.Slots) { return; }

        if (input.phase == InputActionPhase.Started) {
            MainMenu();
        }
    }

    public void OnPauseCamera(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {
            UIMode mode = UIMode.Pause;
            _canvasHandler.SwitchUIMode(mode);

            CameraManager.Instance.PauseOut();
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnResumeCamera(InputAction.CallbackContext input) {
        if (_UIinfo.UIMode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            ResumeGameplay();
        }
    }

    public void OnAnyButton(InputAction.CallbackContext input) {
        if (_UIinfo.UIMode != UIMode.MainScreen) { return; }

        if (input.phase == InputActionPhase.Started) {
            StartCoroutine(ResolveAnyButtonAction());
        }
    }

    
    public void NewGame() {
        ScenesManager.Instance.StartGame();

        CameraManager.Instance.StartGame();
        InputManager.Instance.SetActionMap("Player");
    }
    

    public void SaveGame() {
        DataManager.Instance.OverwriteData(currentSlot);
        DataManager.Instance.RefreshData();

        StartCoroutine("DisplaySlots");
    }

    public void ReturnToMainMenu() {
        ScenesManager.Instance.MainMenu();
        DataManager.Instance.RefreshData();

        UIMode mode = UIMode.MainMenu;
        _canvasHandler.SwitchUIMode(mode);

        CameraManager.Instance.SwitchMenuVCamera(MenuVCameras.Menu);

        StartCoroutine("DisplaySlots");
    }

    public void LoadGame() {
        UIMode mode = UIMode.Slots;
        _canvasHandler.SwitchUIMode(mode);        

        CameraManager.Instance.MenuToSlot();
    }

    public void MainMenu() {
        UIMode mode = UIMode.MainMenu;
        _canvasHandler.SwitchUIMode(mode);

        CameraManager.Instance.MenuToSlot();
    }

    public void PauseToSlots() {
        if (_UIinfo.UIMode == UIMode.Pause) {
            UIMode mode = UIMode.Slots;
            _canvasHandler.SwitchUIMode(mode);

            CameraManager.Instance.SwitchMenuVCamera(MenuVCameras.Slots);
        } else {
            UIMode mode = UIMode.Pause;
            _canvasHandler.SwitchUIMode(mode);

            CameraManager.Instance.SwitchMenuVCamera(MenuVCameras.PauseEnd);
        }
    }

    public void Records() {
        UIMode mode = UIMode.Records;
        _canvasHandler.SwitchUIMode(mode);

        CameraManager.Instance.SwitchMenuVCamera(MenuVCameras.Record);
    }

    public void QuitGame() {
        ScenesManager.Instance.QuitGame();
    }

    public void ResumeGameplay() {
        CameraManager.Instance.PauseIn();
        Cursor.lockState = CursorLockMode.Locked;
    }    

    public void DeleteSlot(int saveSlot) {
        if (saveSlot < 0 && saveSlot > 2) { return; }

        if (slotsInfo[saveSlot].Runtime == 0) {
            int slotID = slotsInfo[saveSlot].SlotID;
            DataManager.Instance.DeleteData(slotID);
            DataManager.Instance.RefreshData();

            StartCoroutine("DisplaySlots");
        } else {
            Debug.Log("Current slot is active!");
        }
    }

    public void DeleteAllSlots() {                        
        DataManager.Instance.DeleteData(1);
        DataManager.Instance.DeleteData(2);
        DataManager.Instance.DeleteData(3);
        DataManager.Instance.RefreshData();

        StartCoroutine("DisplaySlots");
       
    }

    public void SetNewRecord() {
        DataManager.Instance.SetRecord();
        DataManager.Instance.RefreshRecords();

        //Update on all canvas through canvasHandler
        //StartCoroutine("DisplayRecords");
    }

    public void DeleteRecords() {
        DataManager.Instance.ResetRecords();
        DataManager.Instance.RefreshRecords();

        //Update on all canvas through canvasHandler
        //StartCoroutine("DisplayRecords");
    }

    public void SubmitSlot() {
        if (slotsInfo[currentSlot].Checkpoint.x != 0) {                        
            DataManager.Instance.AssignSlotInfo(currentSlot);
            ScenesManager.Instance.LoadGame();

            DataManager.Instance.OverwriteData(currentSlot);
            DataManager.Instance.RefreshData();

            CameraManager.Instance.SetCameraMode(VCameraMode.GameVCameras);
            CameraManager.Instance.StartGame();

            InputManager.Instance.SetActionMap("Player");
        }
    }

    private void SelectSlots() {
        if (_canvasHandler.SaveSlots[(int)SaveSlot.One] == _canvasHandler.SelectedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.One] == _canvasHandler.SelectedButton) {
            SwitchSlot(SaveSlot.One);
        }
        else if (_canvasHandler.SaveSlots[(int)SaveSlot.Two] == _canvasHandler.SelectedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.Two] == _canvasHandler.SelectedButton) {
            SwitchSlot(SaveSlot.Two);
        }
        else if (_canvasHandler.SaveSlots[(int)SaveSlot.Three] == _canvasHandler.SelectedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.Three] == _canvasHandler.SelectedButton) {
            SwitchSlot(SaveSlot.Three);
        }
        else {
            DeselectLights();
        }
    }

    private void HighlightSlots() {
        if (_canvasHandler.SaveSlots[(int)SaveSlot.One] == _canvasHandler.HighlightedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.One] == _canvasHandler.HighlightedButton) {
            SwitchSlot(SaveSlot.One);
        }
        else if (_canvasHandler.SaveSlots[(int)SaveSlot.Two] == _canvasHandler.HighlightedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.Two] == _canvasHandler.HighlightedButton) {
            SwitchSlot(SaveSlot.Two);
        }
        else if (_canvasHandler.SaveSlots[(int)SaveSlot.Three] == _canvasHandler.HighlightedButton ||
            _canvasHandler.OverwriteSlots[(int)SaveSlot.Three] == _canvasHandler.HighlightedButton) {
            SwitchSlot(SaveSlot.Three);
        }
        else {
            DeselectLights();
        }
    }

    private void SwitchSlot(SaveSlot target) {
        DeselectLights();

        switch (target) {
            case SaveSlot.One: {
                    selectedSlot = (int)SaveSlot.One;
                    break;
                }
            case SaveSlot.Two: {
                    selectedSlot = (int)SaveSlot.Two;
                    break;
                }
            case SaveSlot.Three: {
                    selectedSlot = (int)SaveSlot.Three;
                    break;
                }
            default: break;
        }

        currentSlot = selectedSlot;
        SelectLights();
    }   
    
    private void SelectLights() {        
        foreach (Light light in slots[selectedSlot].GetComponentsInChildren<Light>()) {
            if (light.tag == "Light") {
                light.enabled = true;
            } else {
                light.enabled = false;
            }
        }        
    }

    private void DeselectLights() {
        foreach (Light light in slots[currentSlot].GetComponentsInChildren<Light>()) {
            if (light.tag == "Light") {
                light.enabled = false;
            }
            else {
                light.enabled = true;
            }
        }
    }

    private IEnumerator ResolveAnyButtonAction() {
        yield return null;

        UIMode mode = UIMode.MainMenu;
        _canvasHandler.SwitchUIMode(mode);                

        yield return null;
        CameraManager.Instance.SwitchMenuVCamera(MenuVCameras.Menu);

        yield return new WaitForSeconds(.4f);
        InputManager.Instance.SetActionMap("UI");

        yield break;
    }

    private IEnumerator DisplaySlots() {
        int i = 0;

        foreach (GameObject slot in slots) {

            if ((int)slotsInfo[i].Checkpoint.x != 0) {

                TMP_Text[] fields = slot.GetComponentsInChildren<TMP_Text>();

                fields[0].text = slotsInfo[i].Name;
                fields[1].text = slotsInfo[i].CurrentHp + " Hp";
                fields[2].text = "CP: " + slotsInfo[i].Checkpoint.x + "-" + slotsInfo[i].Checkpoint.y;

                i++;

            } else {

                TMP_Text[] fields = slot.GetComponentsInChildren<TMP_Text>();

                fields[0].text = "";
                fields[1].text = "X";
                fields[2].text = "";

                i++;

            }
            
            yield return null;

        }        

        yield break;
    }
    
    /*
    private IEnumerator DisplayRecords() {
        int i = 0;
        TMP_Text[] fields = recordsPod.GetComponentsInChildren<TMP_Text>();

        foreach (TMP_Text field in fields) {

            if (recordsInfo[i].Name == "Default") {
                field.text = "";
            } else {
                int position = i + 1;
                field.text = position.ToString() + ". " + recordsInfo[i].Name + " - " + recordsInfo[i].Score;
                i++;
            }
            

            yield return null;

        }

        yield break;
    }
    */

}
