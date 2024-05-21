using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmPopup : MonoBehaviour
{
    public Button stopButton;
    public TMP_Text titleText;
    public TMP_Text timeText;
    public GameObject container;

    private AlarmInfo info;

    private AlarmController controller;

    private void Start()
    {
        stopButton.onClick.AddListener(StopAlarm);
    }

    public void Init(AlarmController controller, AlarmInfo info)
    {
        this.controller = controller;
        this.info = info;
        titleText.text = info.alarmTitle;
        timeText.text = info.time;

    }

    private void StopAlarm()
    {
        Show(false);
        controller.BuzzOffAlarm();
    }

    public void Show(bool value)
    {
        container.SetActive(value);
        if (info != null)
        {
            string text = "ingantkan saya" + info.alarmTitle;
            StartCoroutine(HomeController.Instance.askMe.ProcessConversation(text));
        }
    }
}
