using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public Button signUpButton;
    public Button forgetButton;
    public Button guestLoginButton;

    private Login login;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(SignUp);
        forgetButton.onClick.AddListener(ForgetPassword);
        guestLoginButton.onClick.AddListener(GuestLogin);
        login = GetComponent<Login>();
    }

    public void GuestLogin()
    {
        StartCoroutine(login.GuestLogin());
    }

    private void Login()
    {
        if (Validate())
        {
            StartCoroutine(login.CheckLogin(emailInputField.text, passwordInputField.text, true, false, () =>
            {
                CustomSceneManager.Instance.LoadScene(Consts.HOME_SCENE, null);
            },
            (error) =>
            {
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Login", error,
                    new ButtonInfo { content = "OK" });
            }));
        }
        else
        {
            PopupManager.Instance.ShowPopupMessage("err", "Data Tidak Valid", "Email dan Password harus diisi",
                    new ButtonInfo { content = "OK" });
        }

        //CustomSceneManager.Instance.LoadScene("Home", null);
    }

    private bool Validate()
    {
        return !string.IsNullOrEmpty(emailInputField.text) && !string.IsNullOrEmpty(passwordInputField.text);
    }

    private void SignUp()
    {
        login.ShowRegistrationPage(true);
    }

    private void ForgetPassword()
    {
        login.ShowForgetPassword();
    }
}
