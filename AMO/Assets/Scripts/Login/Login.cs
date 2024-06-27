using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class APIResponse
{
    public string status;
    public string msg;
}

[Serializable]
public class TokenResponse
{
    public string status;
    public string msg;
    public string email;
    public string token;
}

public class Login : MonoBehaviour
{
    private const string GET_TOKEN_URL = "get_token";

    public GameObject loginPage;
    public GameObject registrationPage;
    public GameObject forgetPasswordPage;
    public GameObject verificationPage;
    public TweenColor backgroundTweenColor;

    private const string GUEST = "guess";

    private IEnumerator Start()
    {
        yield return null;
        //CustomSceneManager.Instance.LoadSceneAsync("Home", null);
        //else
        //{
        //    StartCoroutine(GetToken("medibgr@gmail.com", "gE8YR0RKh3bqfdC1wXAT5fNNr4E3", (token) =>
        //    {
        //        CustomSceneManager.Instance.LoadSceneAsync("Home", null);
        //    }));
        //}

        if (SceneManager.GetActiveScene().name != "Home")
        {
            if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey(Consts.HAS_VERIFIED))
            {
                StartCoroutine(CheckLogin(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"), false, true, () =>
                {
                    CustomSceneManager.Instance.LoadScene("Home", null);
                }, (error) =>
                {
                    ShowLoginPage();
                    PopupManager.Instance.ShowPopupMessage("err", "Gagal Login Otomatis",
                        "Ulangi login secara manual", new ButtonInfo { content = "OK" });
                }));
            }
            else if (!PlayerPrefs.HasKey(Consts.HAS_ACCOUNT) && !PlayerPrefs.HasKey(Consts.HAS_ENTER))
            {
                StartCoroutine(GuestLogin());
            }
            else
            {
                if (PlayerPrefs.HasKey("guest_email"))
                {
                    StartCoroutine(CheckLogin(PlayerPrefs.GetString("guest_email"), PlayerPrefs.GetString("guest_password"), false, false, () =>
                    {
                        CustomSceneManager.Instance.LoadScene("Home", null);
                    }, (error) =>
                    {
                        ShowLoginPage();
                        PopupManager.Instance.ShowPopupMessage("err", "Gagal Login Otomatis",
                            "Ulangi login secara manual", new ButtonInfo { content = "OK" });
                    }));
                }
                else
                {
                    ShowLoginPage();
                }
            }
        }
    }

    public IEnumerator GuestLogin()
    {
        Debug.LogWarning("Guest Login");
        yield return (Register(GUEST, "12345678", (email) =>
        {
            StartCoroutine(CheckLogin(email, "12345678", true, true, () =>
            {
                CustomSceneManager.Instance.LoadScene("Home", null);
            }, null));
        }, (error) =>
        {
            //PopupManager.Instance.ShowPopupMessage("err", "Gagal Login Otomatis",
            //    "Ulangi login secara manual", new ButtonInfo { content = "OK" });
        }));
    }

    public void ShowLoginPage(bool value = true)
    {
        if(loginPage) loginPage.SetActive(value);
        if(registrationPage) registrationPage.SetActive(!value);
        if (value)
            backgroundTweenColor?.PlayBackward(null);
        else
            backgroundTweenColor?.Play();
    }

    public void ShowRegistrationPage(bool value = true)
    {
        if(loginPage) loginPage.SetActive(!value);
        if(registrationPage) registrationPage.SetActive(value);
        if (value)
            backgroundTweenColor?.Play();
        else
            backgroundTweenColor?.PlayBackward(null);
    }

    public void ShowForgetPassword(bool value = true)
    {
        forgetPasswordPage.SetActive(value);
        loginPage.SetActive(!value);
    }

    public void ShowVerification(bool value = true)
    {
        verificationPage.SetActive(value);
        registrationPage.SetActive(!value);
        if (value)
        {
            GetComponent<Verification>().ResetTime();
        }
    }

    /// <summary>
    /// Return string token onComplete
    /// </summary>
    /// <param name="email"></param>
    /// <param name="uid"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public IEnumerator GetToken(string email, string uid, Action<string> onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{ \"email\": \"" + email + "\", \"uid\": \"" + uid + "\" }");

        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + GET_TOKEN_URL, form))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string json = uwr.downloadHandler.text;
                TokenResponse response = JsonUtility.FromJson<TokenResponse>(json);
                Debug.LogWarning("success get token : " + response.token);
                UserData.token = response.token;
                onComplete?.Invoke(response.token);
            }
            else
            {
                Debug.LogError("err : " + uwr.error);
            }
        }
    }

    public IEnumerator Register(string email, string password, Action<string> onSuccess, Action<string> onFailed)
    {
        string json = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        WWWForm form = new WWWForm();
        form.AddField("data", json);

        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "registrasi", form))
        {
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    RegistrationResponse response = JsonUtility.FromJson<RegistrationResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onSuccess?.Invoke(response.email);
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    Debug.LogError("err : " + uwr.error);
                    throw new Exception("Terjadi Kesalahan");
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
            }
        }
    }

    public IEnumerator CheckLogin(string email, string password, bool isSaveToLocal, bool isGuest, Action onSuccess, Action<string> onFailed)
    {
        string json = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        WWWForm form = new WWWForm();
        form.AddField("data", json);
        Debug.LogWarning("CheckLogin : " + email + " " + password);
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "login", form))
        {
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(uwr.downloadHandler.text);
                    Debug.Log(uwr.downloadHandler.text);
                    if (response != null)
                    {
                        if (response.status.ToLower() == "ok")
                        {
                            UserData.token = response.token;
                            if (isSaveToLocal)
                            {
                                if (isGuest)
                                {
                                    PlayerPrefs.SetString("guest_email", email);
                                    PlayerPrefs.SetString("guest_password", password);
                                }
                                else
                                {
                                    PlayerPrefs.SetString("email", email);
                                    PlayerPrefs.SetString("password", password);
                                }
                                PlayerPrefs.Save();
                            }
                            if (SceneManager.GetActiveScene().name != "Home")
                            {
                                PlayerPrefs.SetInt(Consts.HAS_VERIFIED, 1);
                                PlayerPrefs.Save();
                            }
                            Debug.LogWarning("Login successfully : " + email);
                            onSuccess?.Invoke();
                        }
                        else
                        {
                            throw new Exception(response.msg);
                        }
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }

            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                   new ButtonInfo
                   {
                       content = "Ulangi",
                       onButtonClicked = () => StartCoroutine(CheckLogin(email, password, isSaveToLocal, isGuest, onSuccess, onFailed))
                   },
                   new ButtonInfo
                   {
                       content = "Batal"
                   });
            }
        }
    }

    public IEnumerator BindAccount(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        string json = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        WWWForm form = new WWWForm();
        form.AddField("data", json);
        Debug.LogWarning("BindAccount : " + email + " " + password);
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "request_link", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(uwr.downloadHandler.text);
                    Debug.Log(uwr.downloadHandler.text);
                    if (response != null)
                    {
                        if (response.status.ToLower() == "ok")
                        {
                            UserData.token = response.token;
                            //PlayerPrefs.SetString("email", email);
                            //PlayerPrefs.SetString("password", password);
                            //PlayerPrefs.Save();
                            PlayerPrefs.SetInt(Consts.HAS_ACCOUNT, 1);
                            PlayerPrefs.Save();
                            Debug.LogWarning("Login successfully : " + email);
                            onSuccess?.Invoke();
                        }
                        else
                        {
                            Debug.LogError("err : " + response.msg);
                            throw new Exception(response.msg);
                        }
                    }
                }
                else
                {
                    Debug.LogError("err : " + uwr.error);
                    throw new Exception(uwr.error);
                }
            }

            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                   new ButtonInfo
                   {
                       content = "Ulangi",
                       onButtonClicked = () => StartCoroutine(BindAccount(email, password, onSuccess, onFailed))
                   },
                   new ButtonInfo
                   {
                       content = "Batal"
                   });
            }
        }
    }
}
