using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData) {        
        OnHighlight();
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnExitHighlight();
    }

    private void OnHighlight() {
        //EventSystem.current.SetSelectedGameObject(null);
        //MenuController.Instance.CanvasHandler.SetHighlightedButton(this.gameObject);
    }

    private void OnExitHighlight() {
        //MenuController.Instance.SetHighlightedButton(null);
    }
}
