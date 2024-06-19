using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUI : MonoBehaviour
{
    public Button backButton;
    public AlarmController alarmController;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowAlarm);
        backButton.onClick.AddListener(HideAlarm);
    }

    private void ShowAlarm()
    {
        alarmController.Show();
    }

    private void HideAlarm()
    {
        alarmController.Hide();
    }
}
