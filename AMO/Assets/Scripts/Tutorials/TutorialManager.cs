using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.Android.Types;
using DG.Tweening.Plugins;
using static UnityEngine.UI.CanvasScaler;
using System.Linq.Expressions;

[Serializable]
public class TutorialInfo
{
    public string tutorialId;
    public string message;
    public bool isMessageTriggerEvent;
    public bool isShowPointer;
    public UnityEvent targetEvent;
    public UnityEvent currentEvent;
    public GameObject targetClone;
    public string nextTutorialId;
    public bool isCompleted;
}

public class TutorialManager : MonoBehaviour
{
    private const string TUTORIAL_KEY = "tutorial";

    public List<TutorialInfo> tutorialInfoList;

    public GameObject container;
    public Transform targetCloneParent;
    public RectTransform pointer;
    public GameObject topMessageContainer;
    public GameObject bottomMessageContainer;
    public TMP_Text topText;
    public TMP_Text bottomText;

    private GameObject clone;

    private TutorialInfo currentTutorialInfo;

    public static TutorialManager Instance { get; private set; }

    private IEnumerator Start()
    {
        Instance = this;
        //topMessageContainer.GetComponent<Button>().onClick.AddListener(NextTutorial);
        //bottomMessageContainer.GetComponent<Button>().onClick.AddListener(NextTutorial);
        
        if (!PlayerPrefs.HasKey(TUTORIAL_KEY))
        {
            yield return ShowTutorial(tutorialInfoList[0]);
        }
        else
        {
            if (PlayerPrefs.GetString(TUTORIAL_KEY) != "Completed")
            {
                Init();
                string id = PlayerPrefs.GetString(TUTORIAL_KEY);
                if (CheckTutorial(id))
                {
                    TutorialInfo info = GetTutorialInfo(id);
                    if (info != null)
                    {
                        OpenCurrentState(info);
                        yield return ShowTutorial(info);
                    }
                }
            }
        }
    }

    private void Init()
    {
        string id = PlayerPrefs.GetString(TUTORIAL_KEY);
        int index = tutorialInfoList.FindIndex(x => x.tutorialId == id);
        for (int i = 0; i < index; i++)
        {
            tutorialInfoList[i].isCompleted = true;
        }
    }

    private void SaveTutorial()
    {
        PlayerPrefs.SetString(TUTORIAL_KEY, currentTutorialInfo.tutorialId);
        PlayerPrefs.Save();
    }

    private void NextTutorial()
    {
        currentTutorialInfo.isCompleted = true;     
        if (string.IsNullOrEmpty(currentTutorialInfo.nextTutorialId))
        {
            HideTutorial();
            PlayerPrefs.SetString(TUTORIAL_KEY, "Completed");
        }
        else
        {
            HideMessage();
            StartCoroutine(ShowTutorial(GetTutorialInfo(currentTutorialInfo.nextTutorialId)));
        }
    }

    private TutorialInfo GetTutorialInfo(string id)
    {
        return tutorialInfoList.Where(x => x.tutorialId == id).FirstOrDefault();
    }

    public bool CheckTutorial(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            TutorialInfo info = GetTutorialInfo(id);
            if (info == null)
            {
                return false;
            }
            else
            {
                if (info.isCompleted)
                    return false;
            }
        }
        return true;
    }

    private void OpenCurrentState(TutorialInfo info)
    {
        info.currentEvent?.Invoke();
    }

    public IEnumerator ShowTutorial(TutorialInfo info)
    {
        if (info == null) yield break;
        if (info.isCompleted) yield break;
        
        currentTutorialInfo = info;
        container.SetActive(true);
        if (clone) Destroy(clone);
        if (info.isMessageTriggerEvent)
        {
            topMessageContainer.GetComponent<Button>().onClick.AddListener(NextTutorial);
            bottomMessageContainer.GetComponent<Button>().onClick.AddListener(NextTutorial);
        }
        else
        {
            topMessageContainer.GetComponent<Button>().onClick.RemoveAllListeners();
            bottomMessageContainer.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        SaveTutorial();
        HideMessage();
        ShowPointer(false);
        yield return new WaitForSeconds(0.5f);
        clone = Instantiate(info.targetClone);
        clone.transform.SetParent(targetCloneParent, false);
        clone.transform.position = info.targetClone.transform.position;
        Tweening[] tweens = clone.GetComponents<Tweening>();
        if (tweens != null)
        {
            foreach (Tweening tween in tweens)
            {
                tween.enabled = false;
            }
        }
        Button button = clone.GetComponent<Button>();
        button?.onClick.AddListener(info.targetEvent.Invoke);
        button?.onClick.AddListener(NextTutorial);
        RectTransform target = ((RectTransform)clone.transform);
        Vector2 size = target.sizeDelta;
        Vector2 offset =  new Vector3(size.x * (1 - target.pivot.x - 0.5f), size.y * (1 - target.pivot.y - 0.5f));
        pointer.localPosition = clone.transform.localPosition + (Vector3)offset + new Vector3(pointer.sizeDelta.x / 2, -pointer.sizeDelta.y / 2);

        pointer.gameObject.SetActive(info.isShowPointer);
        SetText(info.message);
        ShowPointer(true);
    }

    public void HideTutorial()
    {
        if (clone) Destroy(clone);
        container.SetActive(false);
    }

    private void HideMessage()
    {
        topMessageContainer.SetActive(false);
        bottomMessageContainer.SetActive(false);
    }

    private void ShowPointer(bool value)
    {
        pointer.gameObject.SetActive(value);
    }

    private void SetText(string content)
    {
        Debug.LogWarning("clone pos : " + ((RectTransform)clone.transform).anchoredPosition);
        if (((RectTransform)clone.transform).localPosition.y > 0)
        {
            topMessageContainer.SetActive(false);
            bottomMessageContainer.SetActive(true);
            bottomText.text = content;
        }
        else
        {
            topMessageContainer.SetActive(true);
            bottomMessageContainer.SetActive(false);
            topText.text = content;
        }
    }
}
