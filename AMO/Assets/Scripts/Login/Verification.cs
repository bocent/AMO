using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Verification : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public Button sendCodeButton;
    public Button confirmCodeButton;
    public TMP_Text cooldownText;

    private void Start()
    {
        sendCodeButton.onClick.AddListener(SendCode);
        confirmCodeButton.onClick.AddListener(Verify);
    }

    private void SendCode()
    {
        
    }

    private void Verify()
    {
        
    }
}
