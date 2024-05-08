using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public enum CanvasRenderMode
    {
        screenSpace,
        worldSpace
    }
    public CanvasRenderMode canvasRenderMode;
    public Transform popupParent;
    public GameObject popupMessagePrefab;

    public ButtonInfo defaultButton1;
    public ButtonInfo defaultButton2;

    public List<string> popupIdList;
    public List<PopupMessage> popupList;
    public static PopupManager Instance { get; protected set; }

    [HideInInspector]
    public PopupMessage popupMessage;

    protected virtual void OnEnable()
    {
        //EventManager.onShowPopupMessageConfirmation += ShowPopupMessage;
        //EventManager.onShowPopupMessage += ShowPopupMessage;
        //EventManager.onBackButtonPressed += RemovePopup;

        //StateController.RegisterState(new CustomEvent { action = (obj) => ShowPopupMessage(obj[0].ToString(), obj[1].ToString(), obj[2].ToString(), obj[3] as Action) }, new CustomEvent { action = (obj) => RemovePopup() }, State.POPUP_EXIT);
        //StateController.RegisterState(() => ShowPopupMessage("exit", "EXIT", "Are you sure", Application.Quit, StateController.GoToPrevState), RemovePopup, State.POPUP_EXIT);
        //StateController.RegisterState(() => ShowPopupMessage("test", "test", "test", null), RemovePopup, State.POPUP_GENERAL);
    }

    private void OnDisable()
    {
        //EventManager.onShowPopupMessageConfirmation -= ShowPopupMessage;
        //EventManager.onShowPopupMessage -= ShowPopupMessage;
        //EventManager.onBackButtonPressed -= RemovePopup;
    }

    private void Awake()
    {
        popupList = new List<PopupMessage>();
        popupIdList = new List<string>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Instance = this;
        Debug.LogError("pop up manager : " + this, this);
    }

    public void ShowPopupMessage(string id, string title, string content, ButtonInfo buttonInfo)
    {
        Debug.LogError("ShowPopupMessage : " + content);
        if (popupIdList.Count > 0)
        {
            Debug.LogError("popup id : " + id + " " + popupIdList.Count);
            if (popupIdList.Contains(id))
            {
                return;
            }
        }
        GameObject go = Instantiate(popupMessagePrefab, popupParent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        popupMessage = go.GetComponent<PopupMessage>();
        popupMessage.Show(id, title, content, buttonInfo);
        popupList.Add(popupMessage);
        popupIdList.Add(id);
        Debug.LogError("popup add : " + id + " " + popupIdList.Count);
    }

    public void ShowPopupMessage(string id, string title, string content, ButtonInfo buttonInfo1, ButtonInfo buttonInfo2)
    {
        Debug.LogError("ShowPopupMessage : " + content);
        if (popupIdList.Count > 0)
        {
            if (popupIdList.Contains(id))
            {
                //popupMessage.noButton.onClick.Invoke();
                return;
            }
        }
        GameObject go = Instantiate(popupMessagePrefab, popupParent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        popupMessage = go.GetComponent<PopupMessage>();
        popupMessage.Show(id, title, content, buttonInfo1, buttonInfo2);
        popupList.Add(popupMessage);
        popupIdList.Add(id);
        Debug.LogError("popup add : " + id + " " + popupIdList.Count);
    }

    //public bool RemovePopup(string id, bool invokeAction = true)
    //{
    //    Debug.LogError("remove popup : " + id);
    //    if (popupIdList.Count > 0)
    //    {
    //        for (int i = 0; i < popupIdList.Count; i++)
    //        {
    //            Debug.LogWarning("popup id : " + popupIdList[i]);
    //        }

    //        if (popupIdList.Contains(id))
    //        {
    //            if (invokeAction && popupMessage != null)
    //            {
    //                popupMessage.button1.button.onClick?.Invoke();
    //                popupMessage.button2.button.onClick?.Invoke();
    //            }
    //            int index = popupIdList.FindIndex( (x) => { return x == id; });
    //            popupIdList.RemoveAt(index);
    //            Destroy(popupMessage.gameObject);
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    public void RemovePopup(string id, PopupMessage popupMessage)
    {
        popupIdList.Remove(id);
        popupList.Remove(popupMessage);
        Destroy(popupMessage.gameObject);
    }

    public void RemovePopup()
    {
        Debug.LogError("RemovePopup");
        if (popupIdList.Count > 0)
        {
            Destroy(popupList[popupList.Count - 1].gameObject);
            popupIdList.RemoveAt(popupIdList.Count - 1);
            popupList.RemoveAt(popupList.Count - 1);
        }
    }

    public bool IsContainId(string id)
    {
        return popupIdList.Contains(id);
    }
}
