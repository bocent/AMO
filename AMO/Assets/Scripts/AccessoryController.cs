using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AccessoryInfo
{
    public int accessoryId;
    public string accessoryName;
    public string maskId;
    public string avatarName;
    public AccessoryPrefab[] accessoryPrefabs;
    public AccessoryMaterial[] materials;
    public Sprite accessorySprite;
    public SelectedCharacter.AccessoryType accessoryType;
    public bool hasOwned;
}

[Serializable]
public class AccessoryPrefab
{
    public AvatarInfo.StageType stageType;
    public GameObject prefab;
}

[Serializable]
public class AccessoryMaterial
{
    public AvatarInfo.StageType stageType;
    public Material material;
}

public class AccessoryController : MonoBehaviour
{
    public List<AccessoryInfo> accessoryList;

    public const int DEFAULT_AROHA_HELMET = 5;
    public const int DEFAULT_GILMO_HELMET = 23;
    public const int DEFAULT_LORRY_HELMET = 29;
    public const int DEFAULT_MOCHI_HELMET = 11;
    public const int DEFAULT_OLGA_HELMET = 17;

    public const int DEFAULT_AROHA_OUTFIT = 6;
    public const int DEFAULT_GILMO_OUTFIT = 24;
    public const int DEFAULT_LORRY_OUTFIT = 30;
    public const int DEFAULT_MOCHI_OUTFIT = 12;
    public const int DEFAULT_OLGA_OUTFIT = 18;

    public static AccessoryController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<AccessoryInfo> GetAccessoryList()
    {
        return accessoryList;
    }

    public AccessoryInfo GetAccessoryInfo(int id)
    {
        AccessoryInfo info = accessoryList.Where(x => x.accessoryId == id).FirstOrDefault();
        return info;
    }

    public GameObject GetAccessoryPrefab(int id, AvatarInfo.StageType stageType)
    {
        AccessoryInfo info = GetAccessoryInfo(id);
        if (info != null)
        {
            return info.accessoryPrefabs.Where(x => x.stageType == stageType).Select(x => x.prefab).FirstOrDefault();
        }
        return null;
    }

    public GameObject GetAccessoryPrefab(AccessoryInfo info, AvatarInfo.StageType stageType)
    {
        if (info != null)
        {
            GameObject prefab = info.accessoryPrefabs.Where(x => x.stageType == stageType).Select(x => x.prefab).FirstOrDefault();
            Debug.LogWarning("prefab : " + info.accessoryId + " " + prefab + " " + stageType.ToString());
            return prefab;
        }
        return null;
    }

    public Material GetAccessoryMaterial(AccessoryInfo info, AvatarInfo.StageType stageType)
    {
        if (info != null)
        {
            return info.materials.Where(x => x.stageType == stageType).Select(x => x.material).FirstOrDefault();
        }
        return null;
    }

   
}
