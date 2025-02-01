using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct canvasButtons {
    public GameObject NewGameButton;
    public GameObject ContinueButton;    
    public GameObject ResumeButton;
    public GameObject mAudioButton;
    public GameObject pAudioButton;
    public GameObject rBackButton;
}

[System.Serializable]
public struct menuScreen {
    public GameObject mainScreen;
    public GameObject slotsScreen;
    public GameObject pauseScreen;
    public GameObject recordScreen;
}

[System.Serializable]
public struct menuCanvas {
    public GameObject[] mainCanvas;
    public GameObject slotsCanvas;
    public GameObject[] pauseCanvas;
    public GameObject recordCanvas;
}

public class CanvasHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] UI_Info _UIinfo;
    [SerializeField] menuScreen _menuScreen;
    [SerializeField] canvasButtons _buttons;
    [SerializeField] GameObject[] _saveSlots;
    [SerializeField] menuCanvas _menuCanvas;
    //[SerializeField] GameObject _firstSideWindow;

    public delegate void OnSelectButton();
    public OnSelectButton _onSelectButton;

    public delegate void OnDeselectButton();
    public OnDeselectButton _onDeselectButton;

    public delegate void OnHighlightButton();
    public OnHighlightButton _onHighlightButton;

    public delegate void OnExitHighlightButton();
    public OnExitHighlightButton _onExitHighlightButton;

    public delegate void OnSaveSlotSelectButton();
    public OnSaveSlotSelectButton _onSaveSlotSelectButton;

    public delegate void OnSaveSlotHighlightButton();
    public OnSaveSlotHighlightButton _onSaveSlotHighlightButton;

    private GameObject _highlightedButton;
    private GameObject _selectedButton;

    private GameObject _currentSideWindow;

    public GameObject HighlightedButton { get { return _highlightedButton; } }
    public GameObject SelectedButton { get { return _selectedButton; } }
    public GameObject[] SaveSlots { get { return _saveSlots; } }

    private void Awake() {
        _onHighlightButton += () => Debug.Log("OnHighlightEvent!");
        _onExitHighlightButton += () => Debug.Log("OnExitHighlightEvent!");
        _onSelectButton += () => Debug.Log("OnSelectEvent!");
        _onDeselectButton += () => Debug.Log("OnDeselectEvent!");
    }

    private void Start() {
        SubscribeCallbacks();

        StartCoroutine("SetEventCamera");
    }

    private void OnDestroy() {
        UnsubscribeCallbacks();
    }

    private void SubscribeCallbacks() {        
        InputManager.Instance._controlsChangedResolver += ControlsChanged;
    }

    private void UnsubscribeCallbacks() {
        InputManager.Instance._controlsChangedResolver -= ControlsChanged;
    }

    private void ControlsChanged() {
        if (InputManager.Instance.GetPlayerInput().currentControlScheme == "Gamepad") {

            if (_selectedButton != null) {
                EventSystem.current.SetSelectedGameObject(_selectedButton);
            }
            else if (_highlightedButton != null) {
                EventSystem.current.SetSelectedGameObject(_highlightedButton);
            }
            else {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            }

            Debug.Log("Controls changed to Gamepad!");
        }
        else if (InputManager.Instance.GetPlayerInput().currentControlScheme == "Keyboard&Mouse") {
            if (_selectedButton != null) {
                EventSystem.current.SetSelectedGameObject(_selectedButton);
            } else {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            }

            Debug.Log("Controls changed to Keyboard&Mouse!");
        }
    }

    public void SwitchOptions() {
        if (_UIinfo.UIMode == UIMode.MainMenu) {

            if (_menuCanvas.mainCanvas[0].activeSelf) {
                _menuCanvas.mainCanvas[0].SetActive(false);
                _menuCanvas.mainCanvas[1].SetActive(true);

                EventSystem.current.SetSelectedGameObject(_buttons.mAudioButton);
                EventSystem.current.firstSelectedGameObject = _buttons.mAudioButton;

            } else {
                _menuCanvas.mainCanvas[1].SetActive(false);
                _menuCanvas.mainCanvas[0].SetActive(true);

                EventSystem.current.SetSelectedGameObject(_buttons.NewGameButton);
                EventSystem.current.firstSelectedGameObject = _buttons.NewGameButton;
            }
        }
        else if (_UIinfo.UIMode == UIMode.Pause) {

            if (_menuCanvas.pauseCanvas[0].activeSelf) {
                _menuCanvas.pauseCanvas[0].SetActive(false);
                _menuCanvas.pauseCanvas[1].SetActive(true);

                EventSystem.current.SetSelectedGameObject(_buttons.pAudioButton);
                EventSystem.current.firstSelectedGameObject = _buttons.pAudioButton;
            }
            else {
                _menuCanvas.pauseCanvas[1].SetActive(false);
                _menuCanvas.pauseCanvas[0].SetActive(true);

                EventSystem.current.SetSelectedGameObject(_buttons.ResumeButton);
                EventSystem.current.firstSelectedGameObject = _buttons.ResumeButton;
            }            
        }
    }

    public void SwitchUIMode(UIMode _target) {
        if (_UIinfo.UIMode != _target) {
            _UIinfo.UIMode = _target;

            switch (_target) {
                case UIMode.MainMenu: {
                        _menuScreen.mainScreen.SetActive(true);
                        _menuScreen.slotsScreen.SetActive(false);
                        _menuScreen.pauseScreen.SetActive(false);
                        _menuScreen.recordScreen.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(_buttons.NewGameButton);
                        EventSystem.current.firstSelectedGameObject = _buttons.NewGameButton;
                        break;
                    }
                case UIMode.Slots: {
                        _menuScreen.mainScreen.SetActive(false);
                        _menuScreen.slotsScreen.SetActive(true);
                        _menuScreen.pauseScreen.SetActive(false);
                        _menuScreen.recordScreen.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(_saveSlots[(int)SaveSlot.One]);
                        EventSystem.current.firstSelectedGameObject = _saveSlots[(int)SaveSlot.One];
                        break;
                    }
                case UIMode.Pause: {
                        _menuScreen.mainScreen.SetActive(false);
                        _menuScreen.slotsScreen.SetActive(false);
                        _menuScreen.pauseScreen.SetActive(true);
                        _menuScreen.recordScreen.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(_buttons.ResumeButton);
                        EventSystem.current.firstSelectedGameObject = _buttons.ResumeButton;
                        break;
                    }
                case UIMode.Records: {
                        _menuScreen.mainScreen.SetActive(false);
                        _menuScreen.slotsScreen.SetActive(false);
                        _menuScreen.pauseScreen.SetActive(false);
                        _menuScreen.recordScreen.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(_buttons.rBackButton);
                        EventSystem.current.firstSelectedGameObject = _buttons.rBackButton;
                        break;
                    }
                default: break;
            }
        }
    }

    public void NavigateMenu(Vector2 input) {
        if (EventSystem.current.currentSelectedGameObject != null) { return; }

        if (_selectedButton != null) {
            EventSystem.current.SetSelectedGameObject(_selectedButton);
        }
        else if (_highlightedButton != null) {
            EventSystem.current.SetSelectedGameObject(_highlightedButton);
        }
        else {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerEnter.name == "Blocker") { return; }

        if (HighlightCondition(eventData)) { 
            return; 
        } else {
            RectTransform target = eventData.pointerEnter.GetComponent<RectTransform>();

            OnHighlight(target.gameObject);
            _onHighlightButton();

            if (_UIinfo.UIMode == UIMode.Slots) {
                _onSaveSlotHighlightButton();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerEnter == null) { return; }

        if (HighlightCondition(eventData)) {
            return;
        } else {
            _onExitHighlightButton();
        }
    }
    
    public void OnDeselect(BaseEventData eventData) {
        _onDeselectButton();
    }
    
    public void OnSelect(BaseEventData eventData) {        
        RectTransform target = eventData.selectedObject.GetComponent<RectTransform>();
        _selectedButton = target.gameObject;

        _onSelectButton();

        if (_UIinfo.UIMode == UIMode.Slots) {
            _onSaveSlotSelectButton();
        }
    }

    public void SetSelected(GameObject target) {
        EventSystem.current.SetSelectedGameObject(target);
        _selectedButton = target;
    }

    public void SwitchSideWindow(GameObject target) {
        if (_currentSideWindow != null) {
            _currentSideWindow.SetActive(false);
        }

        _currentSideWindow = target;
        _currentSideWindow.SetActive(true);
    }

    private void OnHighlight(GameObject target) {
        EventSystem.current.SetSelectedGameObject(null);
        //_selectedButton = null;

        _highlightedButton = target;        
    }    

    private bool HighlightCondition(PointerEventData eventData) {
        if (eventData.pointerEnter.GetComponent<Button>() == null &&
            eventData.pointerEnter.GetComponent<TMP_Dropdown>() == null &&
            eventData.pointerEnter.GetComponent<Slider>() == null &&
            eventData.pointerEnter.GetComponent<Toggle>() == null) {
            return true;
        } else return false;
    }

    private IEnumerator SetEventCamera() {
        _menuScreen.mainScreen.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();
        yield return null;

        _menuScreen.slotsScreen.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();
        yield return null;

        _menuScreen.pauseScreen.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();
        yield return null;

        _menuScreen.recordScreen.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();

        yield break;
    }

}
