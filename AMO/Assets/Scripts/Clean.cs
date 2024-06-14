using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_FIX_UP))
        {
            PopupManager.Instance.ShowPopupMessage("err", "UNABLE TO FEED AMO", "AMO need to be fixed first", new ButtonInfo { content = "OK" });
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
            if (UserData.GetRequirementList() != null)
            {
                UserData.RemoveRequirement((int)Main.RequirementType.NEED_CLEAN_UP);
                NeedsController.Instance.Pop(Main.RequirementType.NEED_CLEAN_UP);

                Character.Instance.RequestAddExperience(Character.Instance.GetCurrentAvatarInfo().avatarId, 10, null, null);
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
}
