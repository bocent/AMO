using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UserData
{
    private const int baseExp = 100;
    private const string DEFAULT_AVATAR_NAME = "Aroha";
    
    public static string token;

    private static List<int> requirementTypeList;

    public static string username;
    public static double Coins { get; private set; }
    public static float Energy { get; private set; }

    public readonly string[] moodString = { "Happy", "Slightly Tired", "Bad Mood", "Broken" };

    public static Main.MoodStage Mood { get; private set; }

    public static void SetCoin(double value)
    {
        Coins = value;
    }

    public static void AddCoins(double value)
    {
        Coins += value;
    }

    public static void AddCoins(double value, Action onComplete)
    {
        Coins += value;
        onComplete?.Invoke();
    }

    public static void SetEnergy(int value)
    {
        Energy = value;
    }

    public static void AddEnergy(int value)
    {
        Energy += value;
        if (Energy > 100) Energy = 100;
    }

    public static void UseEnergy(float value)
    {
        Energy -= value;
        if(Energy < 0) Energy = 0;
    }

    public static void SetMood(Main.MoodStage mood)
    {
        Mood = mood;
    }

    public static void Init()
    {
        requirementTypeList = new List<int>();
    }

    public static void SetRequirementList(List<int> newRequirements)
    {
        if (newRequirements != null)
        {
            for (int i = 0; i < newRequirements.Count; i++)
            {
                if (!requirementTypeList.Contains(newRequirements[i]))
                {
                    requirementTypeList.Add(newRequirements[i]);
                }
            }
        }
        //requirementTypeList = new List<int>(newRequirements);
        Debug.LogError("requirement count : " + requirementTypeList.Count);
    }

    public static void RemoveRequirement(int requirement)
    {
        if (requirementTypeList != null)
        {
            if (requirementTypeList.Contains(requirement)) requirementTypeList.Remove(requirement);
            Debug.LogError("req count : " + requirementTypeList.Count);
        }
    }

    public static List<int> GetRequirementList()
    {
        return requirementTypeList;
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
