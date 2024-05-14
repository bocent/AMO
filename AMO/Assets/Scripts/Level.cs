using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Level : MonoBehaviour
{
    public Image levelPanel;
    public Image expBar;
    public TMP_Text levelText;

    public Ease ease = Ease.InOutQuad;
    public float animDuration = 0.5f;

    public void UpdateLevel(int level, int exp, int maxExp, int addExp)
    {
        //Debug.LogWarning("level : " + level + " add exp : " + addExp + " " + exp + "/" + maxExp);
        levelText.text = "LEVEL " + level;
        float from = expBar.fillAmount;
        if (maxExp > exp + addExp)
        {
            float to = (exp + addExp) / (float)maxExp;
            float gap = to - expBar.fillAmount;
            expBar.DOFillAmount(to, animDuration * gap).SetEase(ease).Play();
        }
        else if (maxExp == exp + addExp)
        {
            float gap = 1f - expBar.fillAmount;
            expBar.DOFillAmount(1f, animDuration * gap).SetEase(ease).OnComplete(() => {
                expBar.fillAmount = 0;
                int nextMaxExp = UserData.GetTotalExp(level + 1);
                UpdateLevel(level + 1, 0, nextMaxExp, 0);
            }).Play();
        }
        else
        {

            float gap = 1f - expBar.fillAmount;
            expBar.DOFillAmount(1f, animDuration * gap).SetEase(ease).OnComplete(() =>
            {
                int nextMaxExp = UserData.GetTotalExp(level + 1);
                addExp -= (maxExp - exp);
                expBar.fillAmount = 0;
                UpdateLevel(level + 1, 0, nextMaxExp, addExp);
            }).Play();
        }
    }

    public void SetLevel(string level, int exp, int maxExp)
    {
        levelText.text = "LEVEL " + level;
        float to = exp / (float)maxExp;
        expBar.fillAmount = to;
    }

    public void SetLevel(int level, int exp, int maxExp)
    {
        Debug.LogWarning("level : " + level + " exp : " + exp + "/" + maxExp);
        SetLevel(level.ToString(), exp, maxExp);
    }
}
