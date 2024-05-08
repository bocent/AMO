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

[Serializable]
public class TutorialInfo
{
    public string tutorialId;
    public string message;
    public bool isMessageTriggerEvent;
    public bool isShowPointer;
    public UnityEvent targetEvent;
    public GameObject targetClone;
    public string nextTutorialId;
}

public class TutorialManager : MonoBehaviour
{
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
        yield return null;
        ShowTutorial(tutorialInfoList[0]);
    }

    private void NextTutorial()
    {
        if (string.IsNullOrEmpty(currentTutorialInfo.nextTutorialId))
        {
            HideTutorial();
        }
        else
        {
            HideMessage();
            ShowTutorial(GetTutorialInfo(currentTutorialInfo.nextTutorialId));
        }
    }

    private TutorialInfo GetTutorialInfo(string id)
    {
        return tutorialInfoList.Where(x => x.tutorialId == id).FirstOrDefault();
    }

    public void ShowTutorial(TutorialInfo info)
    {
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
        clone = Instantiate(info.targetClone);
        clone.transform.SetParent(targetCloneParent, false);
        clone.transform.position = info.targetClone.transform.position;
        Button button = clone.GetComponent<Button>();
        button?.onClick.AddListener(info.targetEvent.Invoke);
        button?.onClick.AddListener(NextTutorial);
        RectTransform target = ((RectTransform)clone.transform);
        Vector2 size = target.sizeDelta;
        Vector2 offset =  new Vector3(size.x * (1 - target.pivot.x - 0.5f), size.y * (1 - target.pivot.y - 0.5f));
        pointer.localPosition = clone.transform.localPosition + (Vector3)offset + new Vector3(pointer.sizeDelta.x / 2, -pointer.sizeDelta.y / 2);

        pointer.gameObject.SetActive(info.isShowPointer);
        SetText(info.message);
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
