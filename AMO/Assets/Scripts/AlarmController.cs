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
    public AlarmPopup alarmPopup;
    public GameObject container;

    public List<AlarmItem> alarmList;
    private bool isAlarmOn = false;

    public static AlarmController Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        addAlarmButton.onClick.AddListener(ShowAlarmCreator);
        //StartCoroutine(RequestGetAlarm("", LoadList, null));
        //LoadAlarmList();
        //Debug.LogError("timespan now : " + DateTime.Now.TimeOfDay.ToString(@"hh\:mm"));
    }

    public AlarmItem AddAlarm(AlarmInfo info)
    {
        GameObject item = Instantiate(alarmPrefab, alarmItemParent, false);
        AlarmItem alarmItem = item.GetComponent<AlarmItem>();
        alarmList.Add(alarmItem);
        alarmItem.Init(this, info);
        //addAlarmButton.transform.SetAsLastSibling();
        ShowAddButton();

        return alarmItem;
    }

    public void RemoveAlarm(AlarmInfo info, AlarmItem item)
    {
     
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson( new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();

        StartCoroutine(RequestDeleteAlarm(info.alarmId, () =>
        {
            alarmInfoList.Remove(info);
            item.gameObject.SetActive(false);
            alarmCreator.gameObject.SetActive(false);
            ShowAddButton();
        }, null));
    }

    public void EditAlarm(AlarmItem item, AlarmInfo newInfo)
    {
        
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();
        StartCoroutine(RequestSetAlarm(newInfo.alarmId, newInfo, (response) => {
            int index = alarmInfoList.FindIndex(x => x.alarmId == response.alarm_id.ToString());
            Debug.LogWarning("edit alarm : " + index);
            alarmInfoList[index] = newInfo;
            ClearList();
            LoadList();
        }, null));
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
        for (int i = 0; i < alarmList.Count; i++)
        {
            if (!alarmList[i].gameObject.activeSelf)
            {
                alarmList[i].Init(this, info);

                //alarmItem.transform.SetAsLastSibling();
                //ShowAddButton();
                alarmList[i].gameObject.SetActive(true);
                return alarmList[i];
            }
        }

        return null;
        //return AddAlarm(info);
    }

    public void AddAlarmInfo(AlarmInfo info, Action onComplete)
    {
        //PlayerPrefs.SetString(Consts.ALARM, JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //Debug.LogWarning("save alarm :  " + JsonUtility.ToJson(new AlarmInfoList { alarmList = alarmInfoList }));
        //PlayerPrefs.Save();
        StartCoroutine(RequestSetAlarm("", info, (response) =>
        {
            onComplete?.Invoke();
            alarmInfoList.Add(info);
            ShowAddButton();
        }, null));
    }

    private void ShowAlarmCreator()
    {
        alarmCreator.gameObject.SetActive(true);
        alarmCreator.Init(this);
    }

    public void ShowAlarmCreator(AlarmItem alarmItem, AlarmInfo info)
    {
        alarmCreator.gameObject.SetActive(true);
        alarmCreator.Init(this, info, alarmItem);
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
        LoadingManager.Instance.ShowSpinLoading();
        Debug.Log("RequestGetAlarm");
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
                    Debug.Log("response : " + uwr.downloadHandler.text);
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
                        LoadList();
                        onComplete?.Invoke();
                        LoadingManager.Instance.HideSpinLoading();
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
                LoadingManager.Instance.HideSpinLoading();
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

    public IEnumerator RequestSetAlarm(string alarmId, AlarmInfo info, Action<SetAlarmResponse> onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        string[] times = info.time.Split(":");
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"alarm_id\" : \"" + alarmId + "\",  \"hour\" : \"" + times[0] + "\", \"min\" : \"" + times[1] + "\", \"sun\" : \"" + (info.dayList.Contains(DayOfWeek.Sunday) ? 1 : 0) + "\", \"mon\" : \"" + (info.dayList.Contains(DayOfWeek.Monday) ? 1 : 0) + "\", \"tue\" : \"" + (info.dayList.Contains(DayOfWeek.Tuesday) ? 1 : 0) + "\", \"wed\" : \"" + (info.dayList.Contains(DayOfWeek.Wednesday) ? 1 : 0) + "\", \"thu\" : \"" + (info.dayList.Contains(DayOfWeek.Thursday) ? 1 : 0) + "\", \"fri\" : \"" + (info.dayList.Contains(DayOfWeek.Friday) ? 1 : 0) + "\", \"sat\" : \"" + (info.dayList.Contains(DayOfWeek.Saturday) ? 1 : 0) + "\", \"title\" : \"" + info.alarmTitle + "\", \"active\" : \"" + (info.isOn ? 1 : 0) + "\"}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_alarm", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    SetAlarmResponse response = JsonUtility.FromJson<SetAlarmResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke(response);
                        LoadingManager.Instance.HideSpinLoading();
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
                LoadingManager.Instance.HideSpinLoading();
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Membuat Alarm", e.Message,
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    public IEnumerator RequestDeleteAlarm(string alarmId, Action onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"alarm_id\" : \"" + alarmId + "\"}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "delete_alarm", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Response response = JsonUtility.FromJson<Response>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke();
                        LoadingManager.Instance.HideSpinLoading();
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
                LoadingManager.Instance.HideSpinLoading();
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestDeleteAlarm(alarmId, onComplete, onFailed))
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
