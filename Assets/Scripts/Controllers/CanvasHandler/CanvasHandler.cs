using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct canvasButtons {
    public GameObject NewGameButton;
    public GameObject ContinueButton;    
    public GameObject ResumeButton;
    public GameObject AudioButton;
}

[System.Serializable]
public struct menuCanvas {
    public GameObject mainCanvas;
    public GameObject slotsCanvas;
    public GameObject pauseCanvas;
}

public class CanvasHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] UI_Info _UIinfo;
    [SerializeField] menuCanvas _menuCanvas;
    [SerializeField] canvasButtons _buttons;
    [SerializeField] GameObject[] _saveSlots;

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

            if (_highlightedButton != null) {
                EventSystem.current.SetSelectedGameObject(_highlightedButton);
            }
            else if (_selectedButton != null) {
                EventSystem.current.SetSelectedGameObject(_selectedButton);
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

    public void SwitchUIMode(UIMode _target) {
        if (_UIinfo.UIMode != _target) {
            _UIinfo.UIMode = _target;

            switch (_target) {
                case UIMode.MainMenu: {
                        _menuCanvas.mainCanvas.SetActive(true);
                        _menuCanvas.slotsCanvas.SetActive(false);
                        _menuCanvas.pauseCanvas.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(_buttons.NewGameButton);
                        EventSystem.current.firstSelectedGameObject = _buttons.NewGameButton;
                        break;
                    }
                case UIMode.Slots: {
                        _menuCanvas.mainCanvas.SetActive(false);
                        _menuCanvas.slotsCanvas.SetActive(true);
                        _menuCanvas.pauseCanvas.SetActive(false);

                        EventSystem.current.SetSelectedGameObject(_saveSlots[(int)SaveSlot.One]);
                        EventSystem.current.firstSelectedGameObject = _saveSlots[(int)SaveSlot.One];
                        break;
                    }
                case UIMode.Pause: {
                        _menuCanvas.mainCanvas.SetActive(false);
                        _menuCanvas.slotsCanvas.SetActive(false);
                        _menuCanvas.pauseCanvas.SetActive(true);

                        EventSystem.current.SetSelectedGameObject(_buttons.ResumeButton);
                        EventSystem.current.firstSelectedGameObject = _buttons.ResumeButton;
                        break;
                    }
                default: break;
            }
        }
    }

    public void NavigateMenu(Vector2 input) {
        if (EventSystem.current.currentSelectedGameObject != null) { return; }

        if (_highlightedButton != null) {
            EventSystem.current.SetSelectedGameObject(_highlightedButton);
        }
        else if (_selectedButton != null) {
            EventSystem.current.SetSelectedGameObject(_selectedButton);
        }
        else {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerEnter.GetComponent<Button>() != null) {
            Button target = eventData.pointerEnter.GetComponent<Button>();

            OnHighlight(target.gameObject);
            _onHighlightButton();

            if (_UIinfo.UIMode == UIMode.Slots) {
                _onSaveSlotHighlightButton();
            }

        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerEnter.GetComponent<Button>() != null) {
            _onExitHighlightButton();
        }
    }
    
    public void OnDeselect(BaseEventData eventData) {
        _onDeselectButton();
    }
    
    public void OnSelect(BaseEventData eventData) {        
        Button target = eventData.selectedObject.GetComponent<Button>();
        _selectedButton = target.gameObject;

        _onSelectButton();

        if (_UIinfo.UIMode == UIMode.Slots) {
            _onSaveSlotSelectButton();
        }
    }

    private void OnHighlight(GameObject target) {
        EventSystem.current.SetSelectedGameObject(null);
        _highlightedButton = target;        
    }

    private void OnExitHighlight() {
        //
    }    

    private IEnumerator SetEventCamera() {
        _menuCanvas.mainCanvas.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();
        yield return null;

        _menuCanvas.slotsCanvas.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();
        yield return null;

        _menuCanvas.pauseCanvas.GetComponent<Canvas>().worldCamera = CameraManager.Instance.MenuBrain.GetComponent<Camera>();

        yield break;
    }

}
