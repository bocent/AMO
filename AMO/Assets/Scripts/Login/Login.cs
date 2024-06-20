using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

        //if (PlayerPrefs.HasKey("email"))
        //{
        //    StartCoroutine(CheckLogin(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"), () => 
        //    {
        //        CustomSceneManager.Instance.LoadScene("Home", null);
        //    }, (error) => 
        //    {
        //        PopupManager.Instance.ShowPopupMessage("err", "Gagal Login Otomatis",
        //            "Ulangi login secara manual", new ButtonInfo { content = "OK" });
        //    }));
        //}
    }

    public void ShowLoginPage(bool value = true)
    {
        loginPage.SetActive(value);
        registrationPage.SetActive(!value);
        if (value)
            backgroundTweenColor.PlayBackward(null);
        else
            backgroundTweenColor.Play();
    }

    public void ShowRegistrationPage(bool value = true)
    {
        loginPage.SetActive(!value);
        registrationPage.SetActive(value);
        if (value)
            backgroundTweenColor.Play();
        else
            backgroundTweenColor.PlayBackward(null);
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

    public IEnumerator Register(string email, string password, Action onSuccess, Action<string> onFailed)
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
                    Response response = JsonUtility.FromJson<Response>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onSuccess?.Invoke();
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

    public IEnumerator CheckLogin(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        string json = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        WWWForm form = new WWWForm();
        form.AddField("data", json);

        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "login", form))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(uwr.downloadHandler.text);
                Debug.Log(uwr.downloadHandler.text);
                if (response != null)
                {
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.token = response.token;
                        PlayerPrefs.SetString("email", email);
                        PlayerPrefs.SetString("password", password);
                        PlayerPrefs.Save();
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        onFailed?.Invoke(response.msg);
                    }
                }
            }
            else
            {
                onFailed?.Invoke(uwr.error);
                Debug.LogError("err : " + uwr.error);
            }
        }
    }
}
