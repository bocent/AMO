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
        if (UserData.GetRequirementList() != null)
        {
            if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_CLEAN_UP))
            {
                soap.Show(this);
                ActionProgress.Instance.Show(this);
            }
        }
    }

    private void HideSoap()
    {
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
            HideSoap();
            ActionProgress.Instance.Hide();
            if (UserData.GetRequirementList() != null)
            {
                UserData.RemoveRequirement((int)Main.RequirementType.NEED_CLEAN_UP);
            }
        }
        return elapsedTime / maxTime;
    }

    public override void Close()
    {
        HideSoap();
    }
}
