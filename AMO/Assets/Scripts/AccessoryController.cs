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
    [SerializeField] private List<AccessoryInfo> accessoryList;

    public const string DEFAULT_AROHA_HELMET = "5";
    public const string DEFAULT_GILMO_HELMET = "11";
    public const string DEFAULT_LORRY_HELMET = "14";
    public const string DEFAULT_MOCHI_HELMET = "17";
    public const string DEFAULT_OLGA_HELMET = "20";

    public const string DEFAULT_AROHA_OUTFIT = "6";
    public const string DEFAULT_GILMO_OUTFIT = "gilmo_default_outfit";
    public const string DEFAULT_LORRY_OUTFIT = "lorry_default_outfit";
    public const string DEFAULT_MOCHI_OUTFIT = "mochi_default_outfit";
    public const string DEFAULT_OLGA_OUTFIT = "olga_default_outfit";

    public static AccessoryController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<AccessoryInfo> GetAccessoryList()
    {
        return accessoryList;
    }

    public AccessoryInfo GetAccessoryInfo(string id)
    {
        AccessoryInfo info = accessoryList.Where(x => x.accessoryId == id).FirstOrDefault();
        return info;
    }

    public GameObject GetAccessoryPrefab(string id, AvatarInfo.StageType stageType)
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
            return info.accessoryPrefabs.Where(x => x.stageType == stageType).Select(x => x.prefab).FirstOrDefault();
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
