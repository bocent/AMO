using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignupUI : MonoBehaviour
{
    private const int MIN_USERNAME_LENGTH = 4;
    private const int MAX_USERNAME_LENGTH = 24;
    private const int MIN_PASSWORD_LENGTH = 7;
    private const int MAX_PASSWORD_LENGTH = 32;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confPasswordInputField;

    public Button signUpButton;
    public Button loginButton;
    
    private Login login;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(SignUp);
        login = GetComponent<Login>();
    }

    private void Login()
    {
        login.ShowLoginPage();
    }

    private bool Validate()
    {
        if (CheckEmpty())
        {
            if (CheckLength())
            {
                if (CheckEmail())
                {
                    if (CheckPassword())
                    {
                        if (CheckConfPassword())
                        {
                            return true;
                        }
                        else
                        {
                            PopupManager.Instance.ShowPopupMessage("err", "Register Failed", "Confirmation password is not same", new ButtonInfo { content = "OK" });
                        }
                    }
                    else
                    {
                        PopupManager.Instance.ShowPopupMessage("err", "Register Failed", "Password must be contain an uppercase letter, a lowercase letter, a number and a special character.", new ButtonInfo { content = "OK" });
                    }
                }
                else
                {
                    PopupManager.Instance.ShowPopupMessage("err", "Register Failed", "Email is not valid", new ButtonInfo { content = "OK" });
                }
            }
            else
            {
                PopupManager.Instance.ShowPopupMessage("err", "Register Failed",
                    $"password length must be more than {MIN_PASSWORD_LENGTH} and less than {MAX_PASSWORD_LENGTH}.",
                    new ButtonInfo { content = "OK" });
            }
        }
        else
        {
            PopupManager.Instance.ShowPopupMessage("err", "Register Failed", "Fields cannot be empty", new ButtonInfo { content = "OK" });
        }
        return false;
    }

    private bool CheckEmpty()
    {
        return !string.IsNullOrEmpty(emailInputField.text) &&
            !string.IsNullOrEmpty(passwordInputField.text) &&
            !string.IsNullOrEmpty(confPasswordInputField.text);
    }

    private bool CheckLength()
    {
        return emailInputField.text.Length > 0 
            && passwordInputField.text.Length <= MAX_PASSWORD_LENGTH
            && passwordInputField.text.Length > MIN_PASSWORD_LENGTH; 
    }

    private bool CheckPassword()
    {
        Match match = Regex.Match(passwordInputField.text, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        return match.Success;
    }

    private bool CheckConfPassword()
    {
        return passwordInputField.text == confPasswordInputField.text;
    }

    private bool CheckEmail()
    {
        Match match = Regex.Match(emailInputField.text, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
        return match.Success;
    }

    private void SignUp()
    {
        if (Validate())
        {
            StartCoroutine(login.Register(emailInputField.text, passwordInputField.text, () => {
                login.ShowVerification(true);
            }, (error) => { 
            
            }));
        }
    }
}
