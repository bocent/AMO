using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemLibrary : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform itemParent;
    public GameObject container;

    private List<GameObject> itemList = new List<GameObject>();

    public Toggle outfitToggle;
    public Toggle helmetToggle;
    public Button backButton;

    public Sprite emptyItemSprite;
    public ToggleGroup toggleGroup;
    private Button button;


    private IEnumerator Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Show(true));
        backButton.onClick.AddListener(() => Show(false));
        outfitToggle.onValueChanged.AddListener(ShowOutfitList);
        helmetToggle.onValueChanged.AddListener(ShowHelmetList);
        outfitToggle.SetIsOnWithoutNotify(true);

        yield return null;
        SetEquippedAccesories();
    }

    private void Update()
    {
        Limitation limitation = HomeController.Instance.currentLimitation;
        button.interactable = limitation.dressing != 2;
        
    }

    private void SetEquippedAccesories()
    {
        foreach (GameObject go in itemList)
        {
            Item item = go.GetComponent<Item>();
            if (item.Info != null)
            {
                int helmetId = HomeController.Instance.selectedCharacter.Info.helmetId;
                int outfitId = HomeController.Instance.selectedCharacter.Info.outfitId;
                bool isEquipped =
                    (item.Info.accessoryId == helmetId && helmetId != 0) || 
                    (item.Info.accessoryId == outfitId && outfitId != 0);

                //item.SelectItem(isEquipped);
            }
        }
    }

    public void Init(SelectedCharacter.AccessoryType accessoryType)
    {
        toggleGroup.allowSwitchOff = true;
        foreach (GameObject go in itemList)
        {
            go.SetActive(false);
        }

        int itemCount = 0;
        foreach (AccessoryInfo info in AccessoryController.Instance.GetAccessoryList())
        {
            if (info.hasOwned && info.avatarName == HomeController.Instance.selectedCharacter.Info.avatarName && info.accessoryType == accessoryType)
            {
                GameObject go = GetItem(info);
                //if (accessoryType == SelectedCharacter.AccessoryType.Outfit)
                //{
                //    go.GetComponent<Item>().SetToggle(info.accessoryId == Character.Instance.GetCurrentAvatarInfo().outfitId);
                //    Debug.LogWarning("acc id : " + info.accessoryId + " " + Character.Instance.GetCurrentAvatarInfo().helmetId + " " + (info.accessoryId == Character.Instance.GetCurrentAvatarInfo().outfitId));
                //}
                //else
                //{
                //    go.GetComponent<Item>().SetToggle(info.accessoryId == Character.Instance.GetCurrentAvatarInfo().helmetId);
                //    Debug.LogWarning("acc id : " + info.accessoryId + " " + Character.Instance.GetCurrentAvatarInfo().helmetId + " " + (info.accessoryId == Character.Instance.GetCurrentAvatarInfo().helmetId));
                //}
                itemCount += 1;
            }
        }
        if (itemCount < 9)
        {
            for (int i = itemCount; i < 9; i++)
            {
                GetItem(null);
            }
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            Item item = itemList[i].GetComponent<Item>();
            if (item.Info == null)
            {
                item.SetToggle(false);
            }
            else
            {
                if (accessoryType == SelectedCharacter.AccessoryType.Outfit)
                {
                    item.SetToggle(item.Info.accessoryId == Character.Instance.GetCurrentAvatarInfo().outfitId);
                }
                else
                {
                    item.SetToggle(item.Info.accessoryId == Character.Instance.GetCurrentAvatarInfo().helmetId);
                }
            }
        }
        toggleGroup.allowSwitchOff = false;
    }

    private GameObject SpawnItem(AccessoryInfo info)
    {
        GameObject obj = Instantiate(itemPrefab, itemParent, false);
        Item item = obj.GetComponent<Item>();
        item.Init(this, info, toggleGroup);
        itemList.Add(obj);
        return obj;
    }

    private GameObject GetItem(AccessoryInfo info)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (!itemList[i].activeSelf)
            {
                itemList[i].GetComponent<Item>().Init(this, info, toggleGroup);
                itemList[i].SetActive(true);
                return itemList[i];
            }
        }
        return SpawnItem(info);
    }

    public void Show(bool value)
    {
        container.SetActive(value);
        HomeController.Instance.ShowHUD(!value);
        HomeController.Instance.ShowFittingRoom(value);
        HomeController.Instance.ShowHome(!value);
        outfitToggle.SetIsOnWithoutNotify(true);
        ShowOutfitList(true);
    }

    private void ShowOutfitList(bool value)
    {
        if (value)
        {
            Init(SelectedCharacter.AccessoryType.Outfit);
        }
    }

    private void ShowHelmetList(bool value)
    {
        if (value)
        {
            Init(SelectedCharacter.AccessoryType.Helmet);
        }
    }

    public IEnumerator RequestEquipItem(int itemId, Action<int> onComplete, Action<string> onFailed)
    {
        Debug.LogWarning("RequestEquipItem : " + itemId);
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"items_id\" : \"" + itemId + "\", \"is_used\" : \"" + 1 + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_used_item", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Response response = JsonUtility.FromJson<Response>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        LoadingManager.Instance.HideSpinLoading();
                        onComplete?.Invoke(itemId);
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
                LoadingManager.Instance.HideSpinLoading();
                onFailed?.Invoke(e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                  new ButtonInfo
                  {
                      content = "Ulangi",
                      onButtonClicked = () => StartCoroutine(RequestEquipItem(itemId, onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }
}
