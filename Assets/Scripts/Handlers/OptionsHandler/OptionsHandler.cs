using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsHandler : MonoBehaviour {
    [SerializeField] TMP_InputField inputField;

    public void AddText(TMP_Text text) {
        if (inputField.text.Length < inputField.characterLimit) {
            inputField.text += text.text;

        }
    }

    public void BackspaceText() {
        if (inputField != null && inputField.text.Length > 0) {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
