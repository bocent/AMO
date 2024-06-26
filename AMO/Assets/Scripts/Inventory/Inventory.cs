using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

[Serializable]
public class ItemInfo
{
    public int itemId;
    public string itemName;
    public int energy;
    public int itemCount;
    public Sprite sprite;
}

public class Inventory : MonoBehaviour
{
    public List<ItemInfo> itemInfoList;
    public Transform itemParent;
    public GameObject itemPrefab;
    public GameObject container;
    public Button backButton;

    private List<GameObject> itemList = new List<GameObject>();

    public static Inventory Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        backButton.onClick.AddListener(Hide);
    }

    public void Show(bool value)
    {
        if (value)
            Show();
        else
            Hide();
    }

    public void Show()
    {
        container.SetActive(true);
        LoadInventory();
        HomeController.Instance.ShowChargingRoom(true);
        HomeController.Instance.ShowHome(false);
        HomeController.Instance.ShowHUD(false);
    }

    public void Hide()
    {
        container?.SetActive(false);
        HomeController.Instance.ShowChargingRoom(false);
        HomeController.Instance.ShowHome(true);
        HomeController.Instance.ShowHUD(true);
    }

    public void LoadInventory()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].SetActive(false);
        }

        List<ItemInfo> filteredItemInfoList = itemInfoList.Where(x => x.itemCount > 0).ToList();
        Debug.LogError("count : " + filteredItemInfoList.Count);
        for (int i = 0; i < filteredItemInfoList.Count; i++)
        {
            GameObject item = GetItem();
            item.GetComponent<ChargeItem>().Init(this, filteredItemInfoList[i]);
        }
    }

    private GameObject SpawnItem()
    {
        GameObject item = Instantiate(itemPrefab, itemParent);
        itemList.Add(item);
        Debug.LogError("spawn item : " + item);
        return item;
    }

    private GameObject GetItem()
    {
        Debug.LogError("item count : " + itemList.Count);
        for (int i = 0; i < itemList.Count; i++)
        {
            if (!itemList[i].activeSelf)
            {
                itemList[i].SetActive(true);
                return itemList[i];
            }
        }
        return SpawnItem();
    }

    public void AddItem(int itemId, int quantity)
    {
        ItemInfo info = itemInfoList.Where(x => x.itemId == itemId).FirstOrDefault();
        if (info != null)
        {
            info.itemCount += quantity;
        }
    }

    public void UseItem(ItemInfo info)
    {
        Debug.LogError("item used : " + info.energy);
        int index = itemInfoList.FindIndex(x => x == info);
        if (index != -1)
        {
            if (info.itemCount > 0)
            {
                //Request API
                if (info.itemId == 3)
                {
                    StartCoroutine(RequestUseChargeItem(info.itemId, (itemCount) =>
                    {
                        info.itemCount = itemCount;
                        LoadInventory();
                        StartCoroutine(Character.Instance.RequestSetCharacterStatus("energy", Mathf.RoundToInt(Mathf.Clamp(UserData.Energy, 0, 100)).ToString(), () =>
                        {
                            string json = "\"energy\" : " + "\"+" + info.energy + "\", \"need_repair\" : \"" + 0 + "\" ";
                            StartCoroutine(Character.Instance.RequestSetCharacterStatus(json, () =>
                            {
                                UserData.RemoveRequirement((int)Main.RequirementType.NEED_FOOD);
                                UserData.RemoveRequirement((int)Main.RequirementType.NEED_FIX_UP);
                                NeedsController.Instance.Pop(Main.RequirementType.NEED_FOOD);
                                NeedsController.Instance.Pop(Main.RequirementType.NEED_FIX_UP);

                                HomeController.Instance.ShowEatBatteryEffect();
                                Character.Instance.currentCharacter.PlayEatAnimation();
                                LoadingManager.Instance.HideSpinLoading();
                            }, null));

                        }, null));
                       
                    }, (error) =>
                    {
                       
                    }));
                }
                else
                {
                    Debug.LogError("req : " + UserData.GetRequirementList());
                    if (UserData.GetRequirementList() != null)
                    {
                        if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_FIX_UP))
                        {
                            PopupManager.Instance.ShowPopupMessage("err", "Tidak Dapat Memberi Makan AMO", "AMO perlu diperbaiki terlebih dahulu", new ButtonInfo { content = "OK" });
                        }
                        else
                        {
                            StartCoroutine(RequestUseChargeItem(info.itemId, (itemCount) => 
                            {
                                info.itemCount = itemCount;
                                LoadInventory();
                                StartCoroutine(Character.Instance.RequestSetCharacterStatus("energy", Mathf.RoundToInt(Mathf.Clamp(UserData.Energy, 0, 100)).ToString(), () =>
                                {
                                    StartCoroutine(Character.Instance.RequestSetCharacterStatus("energy", "+" + info.energy, () =>
                                    {
                                        UserData.RemoveRequirement((int)Main.RequirementType.NEED_FOOD);
                                        NeedsController.Instance.Pop(Main.RequirementType.NEED_FOOD);
                                        HomeController.Instance.ShowEatBatteryEffect();
                                        Character.Instance.currentCharacter.PlayEatAnimation();
                                        LoadingManager.Instance.HideSpinLoading();

                                    }, null));
                                }, null));
                                //UserData.AddEnergy(info.energy);
                            }, (error) =>
                            {
                                
                            }));
                        }
                    }
                    else
                    {
                        StartCoroutine(RequestUseChargeItem(info.itemId, (itemCount) =>
                        {
                            info.itemCount = itemCount;
                            LoadInventory();
                            StartCoroutine(Character.Instance.RequestSetCharacterStatus("energy", "+" + info.energy, () =>
                            {
                                StartCoroutine(Character.Instance.RequestSetCharacterStatus("energy", Mathf.RoundToInt(Mathf.Clamp(UserData.Energy, 0, 100)).ToString(), () =>
                                {
                                    UserData.RemoveRequirement((int)Main.RequirementType.NEED_FOOD);
                                    NeedsController.Instance.Pop(Main.RequirementType.NEED_FOOD);
                                
                                    HomeController.Instance.ShowEatBatteryEffect();
                                    Character.Instance.currentCharacter.PlayEatAnimation();
                                    LoadingManager.Instance.HideSpinLoading();
                                }, null));
                            }, null));
                        }, (error) =>
                        {
                            
                        }));
                       
                    }
                }
            }
        }
    }

    public IEnumerator RequestUseChargeItem(int itemId, Action<int> onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"charge_item\" : \"" + itemId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "use_charge_item", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    UseItemResponse response = JsonUtility.FromJson<UseItemResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke(response.sisa_stok);
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
            }
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    AddItem("h_battery", 1);
        //}
    }

    public ItemInfo GetItemInfo(int itemId)
    {
        return itemInfoList.Where(x => x.itemId == itemId).FirstOrDefault();
    }
}
