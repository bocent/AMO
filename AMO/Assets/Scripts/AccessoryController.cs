using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AccessoryInfo
{
    public string accessoryId;
    public string accessoryName;
    public string maskId;
    public string avatarName;
    public GameObject accessoryPrefab;
    public Sprite accessorySprite;
    public SelectedCharacter.AccessoryType accessoryType;
    public bool hasOwned;
}

[Serializable]
public class SkinInfo
{
    public string skinId;
    public Material material;
}

public class AccessoryController : MonoBehaviour
{
    public List<AccessoryInfo> accessoryList;
    public List<SkinInfo> skinInfoList;

    public static AccessoryController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public AccessoryInfo GetAccessoryInfo(string id)
    {
        AccessoryInfo info = accessoryList.Where(x => x.accessoryId == id).FirstOrDefault();
        return info;
    }

    public SkinInfo GetSkinInfo(string id)
    {
        SkinInfo info = skinInfoList.Where(x => x.skinId == id).FirstOrDefault();
        return info;
    }
}
