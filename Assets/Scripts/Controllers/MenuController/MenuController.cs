using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour { 
    public static MenuController Instance { get; private set; }

    [SerializeField] GameObject[] slots;
    [SerializeField] GameObject recordsPod;
    [Space]
    [SerializeField] DataInfo[] slotsInfo;
    [SerializeField] RecordInfo[] recordsInfo;
    [SerializeField] PlayerInfo _playerInfo;
    [SerializeField] UI_Info _UIinfo;
    [Space]
    [SerializeField] CanvasHandler _canvasHandler;    

    private int currentSlot = 0;
    private int selectedSlot = 0;
    private int direction;

    private InputAction _navigateAction;
    private InputAction _submitAction;
    private InputAction _saveAction;
    private InputAction _quitAction;
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
        StartCoroutine("DisplayRecords");
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

        _pauseCameraAction.started += OnPauseCamera;

        _resumeCameraAction.started += OnResumeCamera;

        _anyButtonAction.started += OnAnyButton;

    }

    private void UnsubscribeCallbacks() {
        _navigateAction.started -= OnNavigate;
        //_navigateAction.performed -= OnNavigate;

        //_submitAction.started -= OnSubmit;

        _saveAction.started -= OnSave;

        _quitAction.started -= OnQuit;

        _pauseCameraAction.started -= OnPauseCamera;

        _resumeCameraAction.started -= OnResumeCamera;

        _anyButtonAction.started -= OnAnyButton;

    }

    private void InitializeActions() {
        //UI Actions
        _navigateAction = InputManager.Instance.GetPlayerInput().actions["Navigate"];
        _submitAction = InputManager.Instance.GetPlayerInput().actions["Submit"];
        _saveAction = InputManager.Instance.GetPlayerInput().actions["Save"];
        _quitAction = InputManager.Instance.GetPlayerInput().actions["Quit"];
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
                        //NavigateSlot(input.ReadValue<Vector2>());                        
                        break;
                    }
                case UIMode.Pause: {
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
                        SelectSlot();
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

    public void ContinueGame() {
        DataManager.Instance.ResumeData();
        ScenesManager.Instance.LoadGame();

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

    public void NewGame() {
        UIMode mode = UIMode.Slots;
        _canvasHandler.SwitchUIMode(mode);        

        CameraManager.Instance.MenuToSlot();
    }

    public void MainMenu() {
        UIMode mode = UIMode.MainMenu;
        _canvasHandler.SwitchUIMode(mode);

        CameraManager.Instance.MenuToSlot();
    }

    public void QuitGame() {
        ScenesManager.Instance.QuitGame();
    }

    public void ResumeGameplay() {
        CameraManager.Instance.PauseIn();
        Cursor.lockState = CursorLockMode.Locked;
    }    

    public void DeleteSlot() {
        if (slotsInfo[currentSlot].Runtime == 0) {
            int slotID = slotsInfo[currentSlot].SlotID;
            DataManager.Instance.DeleteData(slotID);
            DataManager.Instance.RefreshData();

            StartCoroutine("DisplaySlots");
        } else {
            Debug.Log("Current slot is active!");
        }
    }

    public void SetNewRecord() {
        DataManager.Instance.SetRecord();
        DataManager.Instance.RefreshRecords();

        StartCoroutine("DisplayRecords");
    }

    public void DeleteRecords() {
        DataManager.Instance.ResetRecords();
        DataManager.Instance.RefreshRecords();

        StartCoroutine("DisplayRecords");
    }    

    private void SelectSlot() {
        if (slotsInfo[currentSlot].Checkpoint.x == 0) {            
            DataManager.Instance.AssignSlotInfo(currentSlot);
            ScenesManager.Instance.StartGame();
            DataManager.Instance.OverwriteData(currentSlot);
            DataManager.Instance.RefreshData();
            CameraManager.Instance.SetCameraMode(VCameraMode.GameVCameras);
            InputManager.Instance.SetActionMap("Player");            

        } else {
            DataManager.Instance.AssignSlotInfo(currentSlot);
            ScenesManager.Instance.LoadGame();
            DataManager.Instance.OverwriteData(currentSlot);
            DataManager.Instance.RefreshData();
            CameraManager.Instance.SetCameraMode(VCameraMode.GameVCameras);
            InputManager.Instance.SetActionMap("Player");
        }

    }

    private void NavigateSlot(Vector2 input) {
        if (input == new Vector2(1f, 0f)) {
            direction = 1;
            SwitchSlot(direction);
        }

        if (input == new Vector2(-1f, 0f)) {
            direction = 0;
            SwitchSlot(direction);
        }
    }

    private void SwitchSlot(int direction) {
        DeselectLights();
        
        if (direction == 1) {

            if (selectedSlot < (int)SaveSlot.Three) {
                selectedSlot++;
            } else {
                selectedSlot = (int)SaveSlot.One;
            }

        } else {

            if (selectedSlot > (int)SaveSlot.One) {
                selectedSlot--;
            }
            else {
                selectedSlot = (int)SaveSlot.Three;
            }

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

}
