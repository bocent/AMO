using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public Button signUpButton;

    private Login login;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(SignUp);
        login = GetComponent<Login>();
    }

    private void Login()
    {
        if (Validate())
        {
            login.CheckLogin(usernameInputField.text, passwordInputField.text, () => { 
            
            },
            (error) => { 
            
            });
        }
    }

    private bool Validate()
    {
        return !string.IsNullOrEmpty(usernameInputField.text) && !string.IsNullOrEmpty(passwordInputField.text);
    }

    private void SignUp()
    {
        login.ShowRegistrationPage(true);
    }
}
