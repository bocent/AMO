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
    public Material material;
    public Sprite accessorySprite;
    public SelectedCharacter.AccessoryType accessoryType;
    public bool hasOwned;
}

public class AccessoryController : MonoBehaviour
{
    [SerializeField] private List<AccessoryInfo> accessoryList;

    public const string DEFAULT_AROHA_HELMET = "aroha_default_helm";
    public const string DEFAULT_GILMO_HELMET = "gilmo_default_outfit";
    public const string DEFAULT_LORRY_HELMET = "lorry_default_outfit";
    public const string DEFAULT_MOCHI_HELMET = "mochi_default_outfit";
    public const string DEFAULT_OLGA_HELMET = "olga_default_outfit";

    public const string DEFAULT_AROHA_OUTFIT = "aroha_default_outfit";
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
}
