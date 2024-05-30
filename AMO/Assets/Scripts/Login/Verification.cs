using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Verification : MonoBehaviour
{
    private const float sendCooldown = 60;

    private float time;

    public Button sendButton;
    public Button backButton;
    public TMP_Text cooldownText;

    public CanvasGroup buttonCanvasGroup;

    private Login login;

    private void Start()
    {
        sendButton.onClick.AddListener(ResetTime);
        backButton.onClick.AddListener(Back);
        login = GetComponent<Login>();
    }


    private void Back()
    {
        login.ShowForgetPassword(false);
    }

    public void ResetTime()
    {
        time = sendCooldown;
    }

    private void Update()
    {
        if (time > 0)
        {
            cooldownText.gameObject.SetActive(true);
            TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.RoundToInt(time));
            Debug.Log("timespan : " + timeSpan.ToString());
            cooldownText.text = timeSpan.ToString(@"mm\:ss");
            buttonCanvasGroup.alpha = 0.5f;
            sendButton.interactable = false;
            time -= Time.deltaTime;
        }
        else
        {
            cooldownText.gameObject.SetActive(false);
            buttonCanvasGroup.alpha = 1f;
            sendButton.interactable = true;
        }
    }

    private IEnumerator RequestVerificationEmail(string email, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"email\" : \"" + email +"\"}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "request_email_verification", form))
        {
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Response response = JsonUtility.FromJson<Response>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke();
                    }
                    else
                    {
                        throw new Exception(response.msg);
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
            }
        }
    }
}
