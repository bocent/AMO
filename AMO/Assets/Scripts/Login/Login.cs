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
    private const string BASE_URL = "https://dev.amoevo.my.id/";
    private const string GET_TOKEN_URL = "get_token";

    private void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CustomSceneManager.Instance.LoadSceneAsync("Home", null);
        }
        else
        {
            StartCoroutine(GetToken("medibgr@gmail.com", "gE8YR0RKh3bqfdC1wXAT5fNNr4E3", (token) =>
            {
                CustomSceneManager.Instance.LoadSceneAsync("Home", null);
            }));
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

        using (UnityWebRequest uwr = UnityWebRequest.Post(BASE_URL + GET_TOKEN_URL, form))
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

    public IEnumerator Register(string email, string password)
    {
        yield return null;
    }
}
