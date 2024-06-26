using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Button backButton;
    
    private Login login;

    private void Start()
    {
        loginButton?.onClick.AddListener(Login);
        signUpButton?.onClick.AddListener(SignUp);
        backButton?.onClick.AddListener(Back);
        login = GetComponent<Login>();
    }

    private void Login()
    {
        login.ShowLoginPage();
    }

    private void Back()
    {
        login.ShowRegistrationPage(false);
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
                            PopupManager.Instance.ShowPopupMessage("err", "Format Tidak Valid", "Konfirmasi password tidak sama", new ButtonInfo { content = "OK" });
                        }
                    }
                    else
                    {
                        PopupManager.Instance.ShowPopupMessage("err", "Format Tidak Valid", "Password harus mengandung huruf dan angka", new ButtonInfo { content = "OK" });
                    }
                }
                else
                {
                    PopupManager.Instance.ShowPopupMessage("err", "Format Tidak Valid", "Email tidak sesuai", new ButtonInfo { content = "OK" });
                }
            }
            else
            {
                PopupManager.Instance.ShowPopupMessage("err", "Format Tidak Valid",
                    $"password harus lebih dari {MIN_PASSWORD_LENGTH} dan kurang dari {MAX_PASSWORD_LENGTH}.",
                    new ButtonInfo { content = "OK" });
            }
        }
        else
        {
            PopupManager.Instance.ShowPopupMessage("err", "Format Tidak Valid", "Fields harus terisi", new ButtonInfo { content = "OK" });
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
        Match match = Regex.Match(passwordInputField.text, @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");
        return match.Success;

        //return passwordInputField.text.All(char.IsLetter) && passwordInputField.text.All(char.IsNumber);
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
            StartCoroutine(login.BindAccount(emailInputField.text, passwordInputField.text, () => {
                login.ShowVerification(true);
            }, (error) => { 
            
            }));
        }
    }
}
