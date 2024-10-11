using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour { 
    public static MenuController Instance { get; private set; }

    [SerializeField] GameObject[] slots;
    [SerializeField] GameObject recordsPod;
    [Space]
    [SerializeField] DataInfo[] slotsInfo;
    [SerializeField] RecordInfo[] recordsInfo;
    [SerializeField] PlayerInfo _playerInfo;

    private UIMode mode = UIMode.MainMenu;

    private int currentSlot = 0;
    private int selectedSlot = 0;
    private int direction;

    private InputAction _navigateAction;
    private InputAction _submitAction;
    private InputAction _saveAction;
    private InputAction _quitAction;

    public UIMode Mode { get { return mode; } set { mode = value; } }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
                
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

    public void OnNavigate(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) {
            switch (mode) {
                case UIMode.MainMenu: {
                        //NavigateMenu()
                        break;
                    }
                case UIMode.Slots: {
                        NavigateSlot(input.ReadValue<Vector2>());                        
                        break;
                    }
                case UIMode.Pause: {
                        //NavigatePause()
                        break;
                    }
            }        
        }        
    }

    public void OnSubmit(InputAction.CallbackContext input) {
        if (input.phase == InputActionPhase.Started) { 
            switch (mode) {
                case UIMode.MainMenu: {
                        SubmitMenu();
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
        if (mode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            SaveGame();
        }
    }

    public void OnQuit(InputAction.CallbackContext input) {
        if (mode != UIMode.Pause) { return; }

        if (input.phase == InputActionPhase.Started) {
            ScenesManager.Instance.QuitGame();
        }
    }

    private void SubscribeCallbacks() {
        _navigateAction.started += OnNavigate;
        _navigateAction.performed += OnNavigate;
        _navigateAction.canceled += OnNavigate;

        _submitAction.started += OnSubmit;
        _submitAction.performed += OnSubmit;
        _submitAction.canceled += OnSubmit;

        _saveAction.started += OnSave;
        _saveAction.performed += OnSave;
        _saveAction.canceled += OnSave;

        _quitAction.started += OnQuit;
        _quitAction.performed += OnQuit;
        _quitAction.canceled += OnQuit;
    }

    private void UnsubscribeCallbacks() {
        _navigateAction.started -= OnNavigate;
        _navigateAction.performed -= OnNavigate;
        _navigateAction.canceled -= OnNavigate;

        _submitAction.started -= OnSubmit;
        _submitAction.performed -= OnSubmit;
        _submitAction.canceled -= OnSubmit;

        _saveAction.started -= OnSave;
        _saveAction.performed -= OnSave;
        _saveAction.canceled -= OnSave;

        _quitAction.started -= OnQuit;
        _quitAction.performed -= OnQuit;
        _quitAction.canceled -= OnQuit;
    }

    private void InitializeActions() {
        _navigateAction = InputManager.Instance.GetPlayerInput().actions["Navigate"];
        _submitAction = InputManager.Instance.GetPlayerInput().actions["Submit"];
        _saveAction = InputManager.Instance.GetPlayerInput().actions["Save"];
        _quitAction = InputManager.Instance.GetPlayerInput().actions["Quit"];
    }

    public void ContinueGame() {
        DataManager.Instance.ResumeData();
        ScenesManager.Instance.LoadGame();
    }

    public void SaveGame() {
        DataManager.Instance.QuickSaveData();
        DataManager.Instance.RefreshData();

        StartCoroutine("DisplaySlots");
    }

    public void OverwriteGame() {
        int slotID = slotsInfo[currentSlot].SlotID;
        DataManager.Instance.OverwriteData(slotID);
        DataManager.Instance.RefreshData();

        StartCoroutine("DisplaySlots");
    }

    public void ReturnToMainMenu() {
        ScenesManager.Instance.MainMenu();
        DataManager.Instance.RefreshData();

        mode = UIMode.MainMenu;

        StartCoroutine("DisplaySlots");
    }

    public void SubmitMenu() {
        mode = UIMode.Slots;
        CameraManager.Instance.MenuToSlot();
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

    public void SelectSlot() {
        if (slotsInfo[currentSlot].Checkpoint.x == 0) {
            try {
                DataManager.Instance.AssignSlotInfo(currentSlot);
                ScenesManager.Instance.StartGame();
                DataManager.Instance.QuickSaveData();
                DataManager.Instance.RefreshData();
                CameraManager.Instance.SetCameraMode(VCameraMode.GameVCameras);
                InputManager.Instance.SetActionMap("Player");
            } catch {
                Debug.Log("Failed to assign slot info");
            }

        } else {
            DataManager.Instance.AssignSlotInfo(currentSlot);
            ScenesManager.Instance.LoadGame();
            DataManager.Instance.QuickSaveData();
            DataManager.Instance.RefreshData();
            CameraManager.Instance.SetCameraMode(VCameraMode.GameVCameras);
            InputManager.Instance.SetActionMap("Player");
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

    public void NavigateSlot(Vector2 input) {
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
