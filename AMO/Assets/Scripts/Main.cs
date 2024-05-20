using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class SpritePair
{
    public string id;
    public Sprite sprite;
}

[Serializable]
public class Limitation
{
    public float energyMax;
    public float energyMin;
    public int selfiePose;
    public bool canUseAskMe;
    public bool canUseAlarm;
    public bool canUseReminder;
    public string hygiene;
    public int mood;
    public int dessing;

    public List<int> requirementList;
}


public class Main : MonoBehaviour
{
    public List<SpritePair> spritePairList;
    public enum MoodStage
    {
        HAPPY = 0,
        SLIGHTLY_TIRED = 1,
        BAD_MOOD = 2,
        BROKEN = 3
    }

    public enum RequirementType
    {
        NEED_CLEAN_UP = 0,
        NEED_FOOD = 1,
        NEED_FIX_UP = 2
    }

    public List<Limitation> limitationList;

    public static Main Instance { get; private set; }

    private void Start()
    {
        if (Instance) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockCharacter(string avatarId)
    {
        StartCoroutine(Character.Instance.UnlockCharacter(avatarId));
    }

    public Sprite GetSprite(string id)
    {
        SpritePair sp = spritePairList.Where(x => x.id == id).FirstOrDefault();
        if (sp != null) return sp.sprite;
        return null;
    }

    public Limitation GetFeatureLimitation(float energy)
    {
        return limitationList.Where(x => x.energyMax >= energy && x.energyMin < energy).FirstOrDefault();
    }
}
