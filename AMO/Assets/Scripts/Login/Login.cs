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
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onFailed?.Invoke(uwr.error);
                Debug.LogError("err : " + uwr.error);
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
                if (response != null)
                {
                    UserData.token = response.token;
                    onSuccess?.Invoke();
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
