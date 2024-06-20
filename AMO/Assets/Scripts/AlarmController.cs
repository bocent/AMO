using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AlarmInfo
{
    public string alarmId;
    public string alarmTitle;
    public string time;
    public List<DayOfWeek> dayList = new List<DayOfWeek>();
    public bool isOn;
}

public class AlarmController : MonoBehaviour
{
    public List<AlarmInfo> alarmInfoList = new List<AlarmInfo>();

    public AlarmCreator alarmCreator;
    public GameObject alarmPrefab;
    public Transform alarmItemParent;
    public Button addAlarmButton;
    public Button cancelCreateButton;
    public AlarmPopup alarmPopup;
    public GameObject container;

    public List<AlarmItem> alarmList = new List<AlarmItem>();
    private bool isAlarmOn = false;


    private void Start()
    {
        addAlarmButton.onClick.AddListener(ShowAlarmCreator);
        cancelCreateButton.onClick.AddListener(HideAlarmCreator);
        //Debug.LogError("timespan now : " + DateTime.Now.TimeOfDay.ToString(@"hh\:mm"));
    }

    public AlarmItem AddAlarm(AlarmInfo info)
    {
        GameObject item = Instantiate(alarmPrefab, alarmItemParent, false);
        AlarmItem alarmItem = item.GetComponent<AlarmItem>();
        alarmList.Add(alarmItem);
        alarmItem.Init(this, info);
        addAlarmButton.transform.SetAsLastSibling();
        ShowAddButton();
        return alarmItem;
    }

    public void RemoveAlarm(AlarmInfo info, AlarmItem item)
    {
        alarmInfoList.Remove(info);
        item.gameObject.SetActive(false);
        alarmCreator.gameObject.SetActive(false);
        ShowAddButton();
    }

    public void EditAlarm(AlarmItem item, AlarmInfo newInfo)
    {
        int index = alarmInfoList.FindIndex(x => x.alarmId == newInfo.alarmId);
        Debug.LogWarning("edit alarm : " + index);
        alarmInfoList[index] = newInfo;
        ClearList();
        LoadList();
    }

    private void ClearList()
    {
        foreach (AlarmItem alarmItem in alarmList)
        {
            alarmItem.gameObject.SetActive(false);
        }
    }

    public void LoadList()
    {
        foreach (AlarmInfo info in alarmInfoList)
        {
            Debug.LogError("info : " + info.alarmId);
            GetAlarm(info);
        }
        addAlarmButton.transform.SetAsLastSibling();
        ShowAddButton();
    }

    public AlarmItem GetAlarm(AlarmInfo info)
    {
        foreach (AlarmItem alarmItem in alarmList)
        {
            if (!alarmItem.gameObject.activeSelf)
            {
                alarmItem.Init(this, info);
                
                //alarmItem.transform.SetAsLastSibling();
                //ShowAddButton();
                alarmItem.gameObject.SetActive(true);
                return alarmItem;
            }
        }

        return AddAlarm(info);
    }

    public void AddAlarmInfo(AlarmInfo info)
    {
        alarmInfoList.Add(info);
        ShowAddButton();
    }

    private void ShowAlarmCreator()
    {
        alarmCreator.Init(this);
        alarmCreator.gameObject.SetActive(true);
    }

    public void ShowAlarmCreator(AlarmItem alarmItem, AlarmInfo info)
    {
        alarmCreator.Init(this, info, alarmItem);
        alarmCreator.gameObject.SetActive(true);
    }

    public void HideAlarmCreator()
    {
        alarmCreator.gameObject.SetActive(false);
    }

    private void CheckAlarmTime()
    {
        int hourNow = DateTime.Now.Hour;
        int minuteNow = DateTime.Now.Minute;
        int secondNow = DateTime.Now.Second;
        int currentTimeInSeconds = hourNow * 3600 + minuteNow * 60 + secondNow;
        DayOfWeek today = DateTime.Now.DayOfWeek;

        foreach (AlarmInfo info in alarmInfoList)
        {
            if (info.dayList != null)
            {
                if ((info.dayList.Contains(today) && info.isOn) || (info.dayList.Count == 0 && info.isOn))
                {
                    string[] times = info.time.Split(":");
                    int hour = int.Parse(times[0]);
                    int minute = int.Parse(times[1]);
                    int totalInSeconds = hour * 3600 + minute * 60;

                    if (currentTimeInSeconds >= totalInSeconds && currentTimeInSeconds < totalInSeconds + 1)
                    {
                        BuzzOnAlarm(info);
                        if (info.dayList.Count == 0 && info.isOn)
                        {
                            info.isOn = false;
                        }
                    }
                }
            }
        }
    }

    private void BuzzOnAlarm(AlarmInfo info)
    {
        if (!isAlarmOn)
        {
            isAlarmOn = true;
            alarmPopup.Init(this, info);
            alarmPopup.Show(true);

            if (info != null)
            {
                string[] intro = { UserData.username };
                string text = "ingatkan saya" + info.alarmTitle;
                //StartCoroutine(HomeController.Instance.askMe.ProcessTextToSpeech(text));
            }
        }
    }

    public void BuzzOffAlarm()
    {
        isAlarmOn = false;
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    private void ShowAddButton()
    {
        addAlarmButton.gameObject.SetActive(alarmInfoList.Count <= 3);
    }

    private void FixedUpdate()
    {
        CheckAlarmTime();
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.LogWarning("show alarm");
        //    BuzzOnAlarm(new AlarmInfo { alarmTitle = "Bangun", time = "09:00" });
        //}
    }
}
