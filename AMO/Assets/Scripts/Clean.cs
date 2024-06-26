using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Clean : ActivityTask
{
    public Soap soap;

    private float elapsedTime = 0;
    private float maxTime = 5f;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowSoap);
    }

    private void ShowSoap()
    {
        Debug.LogWarning("Show Soap");
        if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_FIX_UP))
        {
            PopupManager.Instance.ShowPopupMessage("err", "Tidak Dapat Memberi Makan AMO", "AMO perlu diperbaiki terlebih dahulu", new ButtonInfo { content = "OK" });
        }
        else
        {
            if (UserData.GetRequirementList() != null)
            {
                if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_CLEAN_UP))
                {
                    soap.Show(this);
                    ActionProgress.Instance.Show(this);
                }
            }
        }
    }

    private void HideSoap()
    {
        Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
        soap.Hide();
        ActionProgress.Instance.SetFillBar(0);
    }

    public float Cleaning()
    {
        if (elapsedTime < maxTime)
        {
            elapsedTime += Time.deltaTime;
            ActionProgress.Instance.SetFillBar(elapsedTime / maxTime);
        }
        else
        {
            Reset();
            HideSoap();
            ActionProgress.Instance.Hide();
            ActionProgress.Instance.SetFillBar(0);
            if (UserData.GetRequirementList() != null)
            {
                //UserData.RemoveRequirement((int)Main.RequirementType.NEED_CLEAN_UP);
                //NeedsController.Instance.Pop(Main.RequirementType.NEED_CLEAN_UP);

                //Character.Instance.RequestAddExperience(Character.Instance.GetCurrentAvatarInfo().avatarId, 10, null, null);

                StartCoroutine(RequestClean(() =>
                {
                    UserData.RemoveRequirement((int)Main.RequirementType.NEED_CLEAN_UP);
                    NeedsController.Instance.Pop(Main.RequirementType.NEED_CLEAN_UP);
                    StartCoroutine(Character.Instance.RequestAddExperience(
                        Character.Instance.GetCurrentAvatarInfo().avatarId, 10, null, null));
                },
                (error) =>
                {

                }));
            }
        }
        return elapsedTime / maxTime;
    }

    private void Reset()
    {
        elapsedTime = 0;
    }

    public override void Close()
    {
        HideSoap();
    }

    private IEnumerator RequestClean(Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + Character.Instance.GetCurrentAvatarInfo().avatarId + "\", \"status\" : { \"need_clean\" : \"" + 0 + "\" }}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_status_karakter", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    CharacterStatusResponse response = JsonUtility.FromJson<CharacterStatusResponse>(uwr.downloadHandler.text);
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
                        onButtonClicked = () => StartCoroutine(RequestClean(onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }
}
