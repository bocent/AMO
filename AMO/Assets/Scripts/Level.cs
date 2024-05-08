using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public Image levelPanel;
    public Image expBar;
    public TMP_Text levelText;

    public void SetLevel(string level, int exp, int maxExp)
    {
        levelText.text = "LEVEL " + level;
        expBar.fillAmount = exp / (float)maxExp;
    }

    public void SetLevel(int level, int exp, int maxExp)
    {
        Debug.LogWarning("level : " + level + " exp : " + exp + "/" + maxExp);
        SetLevel(level.ToString(), exp, maxExp);
    }
}
