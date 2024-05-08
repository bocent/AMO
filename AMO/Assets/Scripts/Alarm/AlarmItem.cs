using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmItem : MonoBehaviour
{
    public AlarmInfo Info { get; private set; }

    public TMP_Text titleText;
    public TMP_Text timeText;
    public TMP_Text repeatText;
    public Toggle alarmOnToggle;

    private Button button;
    private AlarmController controller;

    private void Start()
    {
        alarmOnToggle.onValueChanged.AddListener(OnToggleValueChanged);
        button = GetComponent<Button>();
        button.onClick.AddListener(EditAlarm);
    }

    public void Init(AlarmController controller, AlarmInfo info)
    {
        this.controller = controller;
        Info = info;
        Debug.LogWarning("info : " + info);
        string repeatDay = "";

        titleText.text = info.alarmTitle;
        timeText.text = info.time;
        Debug.LogWarning("time : " + info.time);
        alarmOnToggle.isOn = info.isOn;

        if (info.dayList == null)
        {
            repeatText.text = "Never";
            return;
        }
        else if (info.dayList.Count == 0)
        {
            repeatText.text = "Never";
            return;
        }
        if (info.dayList.Count == 2)
        {
            if (info.dayList.Contains(DayOfWeek.Saturday) && info.dayList.Contains(DayOfWeek.Sunday))
            {
                repeatDay = "Weekends";
            }
            repeatText.text = repeatDay;
            return;
        }
        else if (info.dayList.Count == 5)
        {
            bool isWeekday = true;
            for (int i = 1; i < 6; i++)
            {
                if (!info.dayList.Contains((DayOfWeek)i))
                {
                    isWeekday = false;
                    break;
                }
            }
            if (isWeekday) repeatDay = "Weekdays";
            repeatText.text = repeatDay;
            return;
        }

        CultureInfo culture = new CultureInfo("id-ID");
        for (int i = 0; i < info.dayList.Count; i++)
        {
            string day = culture.DateTimeFormat.GetDayName(info.dayList[i]);
            repeatDay += day.Substring(0, 3);
            if (i < info.dayList.Count - 1)
            {
                repeatDay += ",";
            }
        }
        repeatText.text = repeatDay;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        alarmOnToggle.isOn = isOn;
    }

    public void EditAlarm()
    {
        controller.ShowAlarmCreator(this, Info);
    }
}
