using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    private const int baseExp = 100;
    private const string DEFAULT_AVATAR_NAME = "Aroha";
    public static double  Coins { get; private set; }

    public static void AddCoins(double value)
    {
        Coins += value;
    }

    public static int GetTotalExp(int level)
    {
        int exp = Mathf.RoundToInt(level * Mathf.Log10(level) * baseExp) + baseExp;
        return exp;
    }

    public static int[] GetLevel(int level, int totalExp)
    {
        int maxExp = totalExp;
        if (totalExp > 0)
        {
            int nextExp = GetTotalExp(level);
            Debug.Log("level : " + + level);
            Debug.Log("next exp : " + nextExp);
            if (totalExp - nextExp >= 0)
            {
                totalExp -= nextExp;
                Debug.Log("remaning exp : " + nextExp);
                return GetLevel(level + 1, totalExp);
            }
        }
        return new int[] { level, totalExp, GetTotalExp(level) };
    }

    public static string GetAvatarName()
    {
        return PlayerPrefs.HasKey("avatarName") ? PlayerPrefs.GetString("avatarName") : DEFAULT_AVATAR_NAME;
    }

    public static void SetAvatarName(AvatarInfo info)
    {
        PlayerPrefs.SetString("avatarName", info.avatarName);
        PlayerPrefs.Save();
    }
}
