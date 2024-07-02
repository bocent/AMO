using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[Serializable]
public class AlarmInfo
{
    public string alarmId;
    public string alarmTitle;
    public string time;
    public List<DayOfWeek> dayList = new List<DayOfWeek>();
    public bool isOn;
}

[Serializable]
public class AlarmInfoList
{
    public List<AlarmInfo> alarmList;
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
        StartCoroutine(RequestGetAlarm("", null, null));
        //LoadAlarmList();
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
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson( new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();
        
    }

    public void EditAlarm(AlarmItem item, AlarmInfo newInfo)
    {
        int index = alarmInfoList.FindIndex(x => x.alarmId == newInfo.alarmId);
        Debug.LogWarning("edit alarm : " + index);
        alarmInfoList[index] = newInfo;
        ClearList();
        LoadList();
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();
        //StartCoroutine(RequestSetAlarm(newInfo.alarmId, null, null));
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
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //Debug.LogWarning("save alarm :  " + JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();
        //StartCoroutine(RequestSetAlarm(info.alarmId, null, null));
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
                string text = info.alarmTitle;
                Debug.LogWarning("text : " + text);
                StartCoroutine(HomeController.Instance.askMe.ProcessConversation("Alarm saya berbunyi. Ingatkan saya dengan singkat alarm yang berisi : ", text));
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

    //public void LoadAlarmList()
    //{
    //    Debug.LogWarning("LoadAlarmList");
    //    if (PlayerPrefs.HasKey(Consts.ALARM))
    //    {
    //        string json = PlayerPrefs.GetString(Consts.ALARM);
    //        Debug.LogWarning("alarm json : " + json);
    //        AlarmInfoList list = JsonUtility.FromJson<AlarmInfoList>(json);
    //        if (list != null)
    //        {
    //            LoadList();
    //        }
    //    }
    //}

    public IEnumerator RequestGetAlarm(string alarmId, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"alarm_id\" : \"" + alarmId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_alarm", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    AlarmResponse response = JsonUtility.FromJson<AlarmResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        foreach (Alarm alarm in response.alarm)
                        {
                            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
                            if (alarm.sun == 1) dayOfWeeks.Add(DayOfWeek.Sunday);
                            if (alarm.mon == 1) dayOfWeeks.Add(DayOfWeek.Monday);
                            if (alarm.tue == 1) dayOfWeeks.Add(DayOfWeek.Tuesday);
                            if (alarm.wed == 1) dayOfWeeks.Add(DayOfWeek.Wednesday);
                            if (alarm.thu == 1) dayOfWeeks.Add(DayOfWeek.Thursday);
                            if (alarm.fri == 1) dayOfWeeks.Add(DayOfWeek.Friday);
                            if (alarm.sat == 1) dayOfWeeks.Add(DayOfWeek.Saturday);
                            alarmInfoList.Add(new AlarmInfo { alarmId = alarm.alarm_id.ToString(), alarmTitle = alarm.title, isOn = alarm.active == 1, time = alarm.hour + ":" + alarm.minute, dayList = dayOfWeeks });
                        }
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
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestGetAlarm(alarmId, onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    public IEnumerator RequestSetAlarm(string alarmId, int hour, int minute, int sun, int mon, int tue, int wed, int thu, int fri, int sat, string title, int active, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"alarm_id\" : \"" + alarmId + "\",  \"hour\" : \"" + hour + "\", \"minute\" : \"" + minute + "\", \"sun\" : \"" + sun + "\", \"mon\" : \"" + mon + "\", \"tue\" : \"" + tue + "\", \"wed\" : \"" + wed + "\", \"thu\" : \"" + thu + "\", \"fri\" : \"" + fri + "\", \"sat\" : \"" + sat + "\", \"title\" : \"" + title + "\", \"active\" : \"" + active + "\"}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_alarm", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    BuyItemResponse response = JsonUtility.FromJson<BuyItemResponse>(uwr.downloadHandler.text);
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
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestSetAlarm(alarmId, hour, minute, sun, mon, tue, wed, thu, fri, sat, title, active, onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    private void FixedUpdate()
    {
        CheckAlarmTime();
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.LogWarning("show alarm");
        //    BuzzOnAlarm(new AlarmInfo { alarmTitle = "Pekerjaan rumah", time = "09:00" });
        //}
    }
}
