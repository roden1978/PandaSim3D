using TMPro;
using UnityEngine;

public class InputFieldActivator : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;

    private void OnEnable()
    {
        ActivateInput();
    }

    private void ActivateInput()
    {
        _inputField.Select();
        _inputField.ActivateInputField();
    }
}