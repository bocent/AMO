using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DayPair
{
    public int day;
    public string dayName;
    public bool isActive;
}

public class AlarmCreator : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text repeatText;
    public TMP_InputField titleInputField;
    public Button editRepeatButton;
    public Button saveButton;
    public Button deleteButton;
    public Button closeDayPanelButton;
    public int hourIndex;
    public int minuteIndex;
    public SimpleScrollSnap hourScrollSnap;
    public SimpleScrollSnap minuteScrollSnap;
    public List<DayItem> dayItemList;

    public GameObject alarmDayPanel;

    private AlarmInfo alarmInfo;
    private AlarmController controller;
    private bool isCreateNew;
    private int index;
    private AlarmItem selectedAlarmItem;

    private IEnumerator Start()
    {
        editRepeatButton.onClick.AddListener(OpenRepeatDayPanel);
        titleInputField.onSubmit.AddListener(OnTitleSubmitted);
        saveButton.onClick.AddListener(Save);
        closeDayPanelButton.onClick.AddListener(HideRepeatDayPanel);
        hourScrollSnap.OnPanelCentered.AddListener(OnHourSelected);
        minuteScrollSnap.OnPanelCentered.AddListener(OnMinuteSelected);
        deleteButton.onClick.AddListener(DeleteAlarm);
        yield return null;
        gameObject.SetActive(false);
    }

    public void Init(AlarmController controller)
    {
        index = -1;
        isCreateNew = true;
        titleText.text = "Alarm Baru".ToUpper();
        alarmInfo = new AlarmInfo { alarmId = Guid.NewGuid().ToString() };
        this.controller = controller;
        Load(null);
        deleteButton.gameObject.SetActive(false);
        OpenRepeatDayPanel();
    }

    public void Init(AlarmController controller, AlarmInfo alarmInfo, AlarmItem selectedAlarmItem)
    {
        isCreateNew = false;
        titleText.text = "Ubah Alarm".ToUpper();
        this.selectedAlarmItem = selectedAlarmItem;
        this.alarmInfo = alarmInfo;
        this.controller = controller;
        Load(alarmInfo);
        deleteButton.gameObject.SetActive(true);
        OpenRepeatDayPanel();
    }

    public void Load(AlarmInfo info)
    {
        Debug.LogWarning("load alarm creator");
        if (info != null)
        {
            string repeatDay = "";

            string time = info.time;
            Debug.LogWarning("time : " + time);
            if (!string.IsNullOrEmpty(time))
            {
                string[] times = time.Split(":");
                string hour = times[0];
                string minute = times[1];
                Debug.LogWarning("hourScrollSnap : " + hourScrollSnap);
                for (int i = 0; i < hourScrollSnap.Panels.Length; i++)
                {
                    if (hourScrollSnap.Panels[i].GetComponent<TMP_Text>().text == hour)
                    {
                        hourScrollSnap.GoToPanel(i);
                        break;
                    }
                }

                for (int i = 0; i < minuteScrollSnap.Panels.Length; i++)
                {
                    if (minuteScrollSnap.Panels[i].GetComponent<TMP_Text>().text == minute)
                    {
                        minuteScrollSnap.GoToPanel(i);
                        break;
                    }
                }
            }
            if (info.dayList == null)
            {
                repeatDay = "Sekali";
                repeatText.text = repeatDay;
                return;
            }
            else if (info.dayList.Count == 0)
            {
                repeatDay = "Sekali";
                repeatText.text = repeatDay;
                return;
            }
            else if (info.dayList.Count == 2)
            {
                if (info.dayList.Contains(DayOfWeek.Saturday) && info.dayList.Contains(DayOfWeek.Sunday))
                {
                    repeatDay = "Sabtu dan Minggu";
                    repeatText.text = repeatDay;
                    return;
                }
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
                if (isWeekday)
                {
                    repeatDay = "Senin Sampai Jumat";
                    repeatText.text = repeatDay;
                    return;
                }
            }
            else if (info.dayList.Count == 7)
            {
                repeatDay = "Setiap Hari";
                repeatText.text = repeatDay;
                return;
            }

            CultureInfo culture = new CultureInfo("id-ID");
            for (int i = 0; i < info.dayList.Count; i++)
            {
                string day = culture.DateTimeFormat.GetDayName(info.dayList[i]);
                Debug.LogError("new day : " + info.dayList[i]);
                repeatDay += day.Substring(0, 3);
                if (i < info.dayList.Count - 1)
                {
                    repeatDay += ", ";
                }
            }
            repeatText.text = repeatDay;
        }
        else
        {
            hourScrollSnap.GoToPanel(0);
            minuteScrollSnap.GoToPanel(0);
            repeatText.text = "Never";
            titleInputField.text = "";
        }
    }

    private void OnTitleSubmitted(string content)
    {
        titleInputField.text = content;
    }

    private void OpenRepeatDayPanel()
    {
        //alarmDayPanel.SetActive(true);
        if (alarmInfo != null)
        {
            if (alarmInfo.dayList != null)
            {
                for (int i = 0; i < dayItemList.Count; i++)
                {
                    if (alarmInfo.dayList.Contains((DayOfWeek)dayItemList[i].dayPair.day))
                    {
                        dayItemList[i].Init(true);
                    }
                    else
                    {
                        dayItemList[i].Init(false);
                    }
                }
            }
        }
        else
        {
            foreach (DayItem dayItem in dayItemList)
            {
                dayItem.Init(false);
            }
        }
    }

    public void HideRepeatDayPanel()
    {
        alarmInfo.dayList = new List<DayOfWeek>();
        //alarmDayPanel.SetActive(false);
        foreach (DayItem dayItem in dayItemList)
        {
            if (dayItem.dayPair.isActive)
            {
                alarmInfo.dayList.Add((DayOfWeek)dayItem.dayPair.day);
                Debug.LogError("day : " + (DayOfWeek)dayItem.dayPair.day);
            }
        }
        Load(alarmInfo);
    }

    private void Save()
    {
        alarmInfo.alarmTitle = titleInputField.text == "" ? "Alarm" : titleInputField.text;
        alarmInfo.time =
            hourScrollSnap.Content.GetChild(hourIndex).GetComponent<TMP_Text>().text + ":" +
            minuteScrollSnap.Content.GetChild(minuteIndex).GetComponent<TMP_Text>().text;
        alarmInfo.isOn = true;
        if (isCreateNew)
        {
            controller.AddAlarmInfo(alarmInfo, () =>
            {
                controller.GetAlarm(alarmInfo);
            });
        }
        else
            controller.EditAlarm(selectedAlarmItem, alarmInfo);
        controller.HideAlarmCreator();
        HideRepeatDayPanel();
    }

    private void Load()
    {
        
    }

    private void OnHourSelected(int to, int from)
    {
        hourIndex = to;
        alarmInfo.time =
           hourScrollSnap.Content.GetChild(hourIndex).GetComponent<TMP_Text>().text + ":" +
           minuteScrollSnap.Content.GetChild(minuteIndex).GetComponent<TMP_Text>().text;
    }

    private void OnMinuteSelected(int to, int from)
    {
        minuteIndex = to;
        alarmInfo.time =
           hourScrollSnap.Content.GetChild(hourIndex).GetComponent<TMP_Text>().text + ":" +
           minuteScrollSnap.Content.GetChild(minuteIndex).GetComponent<TMP_Text>().text;
    }

    private void DeleteAlarm()
    {
        controller.RemoveAlarm(alarmInfo, selectedAlarmItem);
    }
}
