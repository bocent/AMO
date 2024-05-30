using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        sendButton.onClick.AddListener(ResetTime);
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
}
