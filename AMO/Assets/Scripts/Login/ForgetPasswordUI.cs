using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ForgetPasswordUI : MonoBehaviour
{
    private const float sendCooldown = 60;

    private float time;

    public TMP_InputField emailInputField;
    public TMP_Text cooldownText;
    public Button sendButton;
    public Button backButton;
    public CanvasGroup buttonCanvasGroup;

    private Login login;

    private void Start()
    {
        time = 0;
        sendButton.onClick.AddListener(SendForgetPassword);
        backButton.onClick.AddListener(Back);
        login = GetComponent<Login>();
    }

    private void Back()
    {
        login.ShowForgetPassword(false);
    }

    private void ResetTime()
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

    private void SendForgetPassword()
    {
        string email = emailInputField.text;
        StartCoroutine(RequestForgetPassword(email, ResetTime, (error) => {
            Debug.LogError("err : " + error);
            PopupManager.Instance.ShowPopupMessage("err", "Gagal Reset Password",
                "Silahkan ulangi lagi", new ButtonInfo { content = "Retry", onButtonClicked = SendForgetPassword },
                new ButtonInfo { content = "Batal" });
        }));
    }

    private IEnumerator RequestForgetPassword(string email, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"email\" : \"" + email + "\"}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "forget_password", form))
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
