using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderTimer : MonoBehaviour
{
    public int hourIndex;
    public int minuteIndex;
    public SimpleScrollSnap hourScrollSnap;
    public SimpleScrollSnap minuteScrollSnap;
    public Button saveButton;
    public Button deleteButton;

    public List<DayItem> dayItemList;

    private bool isCreateNew;

    private NoteInfo noteInfo;
    private ToDoController controller;
    private ReminderNote selectedNote;

    public void Init(ToDoController controller)
    {
        isCreateNew = true;
        noteInfo = new NoteInfo {  };
        this.controller = controller;
        deleteButton.gameObject.SetActive(false);
        OpenRepeatDayPanel();
    }

    public void Init(ToDoController controller, NoteInfo noteInfo, ReminderNote selectedNote)
    {
        isCreateNew = false;
        this.selectedNote = selectedNote;
        this.noteInfo = noteInfo;
        this.controller = controller;
        deleteButton.gameObject.SetActive(true);
        OpenRepeatDayPanel();
    }

    //public void Load(NoteInfo info)
    //{
    //    Debug.LogWarning("load reminder creator");
    //    if (info != null)
    //    {
    //        string repeatDay = "";

    //            string hour = info.hour;
    //            string minute = info.minute;
    //            Debug.LogWarning("hourScrollSnap : " + hourScrollSnap);
    //            for (int i = 0; i < hourScrollSnap.Panels.Length; i++)
    //            {
    //                if (hourScrollSnap.Panels[i].GetComponent<TMP_Text>().text == hour)
    //                {
    //                    hourScrollSnap.GoToPanel(i);
    //                    break;
    //                }
    //            }

    //            for (int i = 0; i < minuteScrollSnap.Panels.Length; i++)
    //            {
    //                if (minuteScrollSnap.Panels[i].GetComponent<TMP_Text>().text == minute)
    //                {
    //                    minuteScrollSnap.GoToPanel(i);
    //                    break;
    //                }
    //            }

    //        if (info.dayList == null)
    //        {
    //            repeatDay = "Sekali";
    //            repeatText.text = repeatDay;
    //            return;
    //        }
    //        else if (info.dayList.Count == 0)
    //        {
    //            repeatDay = "Sekali";
    //            repeatText.text = repeatDay;
    //            return;
    //        }
    //        else if (info.dayList.Count == 2)
    //        {
    //            if (info.dayList.Contains(DayOfWeek.Saturday) && info.dayList.Contains(DayOfWeek.Sunday))
    //            {
    //                repeatDay = "Sabtu dan Minggu";
    //                repeatText.text = repeatDay;
    //                return;
    //            }
    //        }
    //        else if (info.dayList.Count == 5)
    //        {
    //            bool isWeekday = true;
    //            for (int i = 1; i < 6; i++)
    //            {
    //                if (!info.dayList.Contains((DayOfWeek)i))
    //                {
    //                    isWeekday = false;
    //                    break;
    //                }
    //            }
    //            if (isWeekday)
    //            {
    //                repeatDay = "Senin Sampai Jumat";
    //                repeatText.text = repeatDay;
    //                return;
    //            }
    //        }
    //        else if (info.dayList.Count == 7)
    //        {
    //            repeatDay = "Setiap Hari";
    //            repeatText.text = repeatDay;
    //            return;
    //        }

    //        CultureInfo culture = new CultureInfo("id-ID");
    //        for (int i = 0; i < info.dayList.Count; i++)
    //        {
    //            string day = culture.DateTimeFormat.GetDayName(info.dayList[i]);
    //            Debug.LogError("new day : " + info.dayList[i]);
    //            repeatDay += day.Substring(0, 3);
    //            if (i < info.dayList.Count - 1)
    //            {
    //                repeatDay += ", ";
    //            }
    //        }
    //        repeatText.text = repeatDay;
    //    }
    //    else
    //    {
    //        hourScrollSnap.GoToPanel(0);
    //        minuteScrollSnap.GoToPanel(0);
    //        repeatText.text = "Never";
    //        titleInputField.text = "";
    //    }
    //}

    private void OpenRepeatDayPanel()
    {
        //alarmDayPanel.SetActive(true);
        if (noteInfo != null)
        {
            if (noteInfo.dayList != null)
            {
                for (int i = 0; i < dayItemList.Count; i++)
                {
                    if (noteInfo.dayList.Contains((DayOfWeek)dayItemList[i].dayPair.day))
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
        noteInfo.dayList = new List<DayOfWeek>();
        //alarmDayPanel.SetActive(false);
        foreach (DayItem dayItem in dayItemList)
        {
            if (dayItem.dayPair.isActive)
            {
                noteInfo.dayList.Add((DayOfWeek)dayItem.dayPair.day);
                Debug.LogError("day : " + (DayOfWeek)dayItem.dayPair.day);
            }
        }
    }

    private void Save()
    {
        noteInfo.title = selectedNote.GetTitleText() == "" ? "Catatan Baru" : selectedNote.GetTitleText();
        noteInfo.hour = hourScrollSnap.Content.GetChild(hourIndex).GetComponent<TMP_Text>().text;
        noteInfo.minute = minuteScrollSnap.Content.GetChild(minuteIndex).GetComponent<TMP_Text>().text;

        

        //if (isCreateNew)
        //{
        //    controller.AddAlarmInfo(alarmInfo, () =>
        //    {
        //        controller.GetAlarm(alarmInfo);
        //    });
        //}
        //else
        //    controller.EditAlarm(selectedAlarmItem, alarmInfo);
        //controller.HideAlarmCreator();
        HideRepeatDayPanel();
    }
}
