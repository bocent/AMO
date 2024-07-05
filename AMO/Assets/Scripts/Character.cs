using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Character : MonoBehaviour
{
    [SerializeField] private List<AvatarInfo> avatarInfoList;
    //[SerializeField] private List<AvatarInfo> liveAvatarInfoList;
    [SerializeField] private Transform characterParent;
    [SerializeField] private List<AvatarAsset> avatarAssetList;

    public List<SelectedCharacter> characterList = new List<SelectedCharacter>();

    public GameObject unlockCharacterPopup;

    public SelectedCharacter currentCharacter;
    public int SelectedAvatarId { get; private set; }

    public static Character Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < avatarInfoList.Count; i++)
        {
            avatarInfoList[i].isUnlocked = false;
        }

        yield return (RequestCharacters(() =>
        {
            StartCoroutine(RequestItemList(() =>
            {
                StartCoroutine(RequestUserData((id) =>
                {
                    LoadAllCharacter();
                    LoadCharacter(id);
                    SelectedAvatarId = id;
                    //UpdateSelectedAvatar();
                    HomeController.Instance.characterSelection.Init();

                    StartCoroutine(RequestSetCharacterStatus("energy", Mathf.RoundToInt(Mathf.Clamp(UserData.Energy, 0, 100)).ToString(), () =>
                    {
                        StartCoroutine(RequestSetCharacterStatus("energy", "+0", () =>
                        {
                            StartCoroutine(AlarmController.Instance.RequestGetAlarm("", LoadingManager.Instance.HideSpinLoading, null));
                        }, null));
                    }, null));
                }, null));
            }, null));
        },
        (error) =>
        {
            Debug.LogError("err : " + error);
        }));
    }

    public void Reset()
    {
        //LoadAllCharacter();
        HomeController.Instance.characterSelection.Init();
    }

    public void UpdateSelectedAvatar(int avatarId)
    {
        //AvatarInfo info = GetLastStageAvatar(UserData.GetAvatarName());
        //SelectedAvatarId = info.avatarId;

        StartCoroutine(RequestSelectCharacter(avatarId, (id) => {
            SelectedAvatarId = id;
            NeedsController.Instance.ClearAll();
            StartCoroutine(RequestUserData((id) => 
            {
                StartCoroutine(RequestSetCharacterStatus("energy", Mathf.RoundToInt(Mathf.Clamp(UserData.Energy, 0, 100)).ToString(), () =>
                {
                    StartCoroutine(RequestSetCharacterStatus("energy", "+0", () =>
                    {
                        LoadingManager.Instance.HideSpinLoading();
                    }, null));
                }, null));
            }, (error) => 
            {

            }));
        }, (error) =>
        { 
        
        }));
    }

    private void LoadAllCharacter()
    {
        foreach (SelectedCharacter character in characterList)
        {
            Destroy(character.gameObject);
        }
        characterList = new List<SelectedCharacter>();

        foreach (AvatarInfo info in avatarInfoList)
        {
            foreach(EvolutionResponse data in info.evolutionList)
            {
                GameObject charObj = Instantiate(data.characterPrefab, characterParent, false);
                SelectedCharacter character = charObj.GetComponent<SelectedCharacter>();
                AvatarInfo newInfo = info.Clone();
                newInfo.stageType = GetStageType(data.evolutionName);
                StartCoroutine(character.Init(newInfo));
                charObj.SetActive(false);
                characterList.Add(character);
            }
        }
    }

    private void LoadCharacter(int avatarId)
    {
        foreach (SelectedCharacter character in characterList)
        {
            if (character.Info.avatarId == avatarId)
            {
                currentCharacter = character;
                //HomeController.Instance.selectedCharacter = character;
                //character.gameObject.SetActive(true);
                HomeController.Instance.SelectCharacter(character.info, false, false);
                break;
            }
        }
    }

    private AvatarInfo GetLastStageAvatar(string avatarName)
    {
        return avatarInfoList.Where(x => x.avatarName == avatarName && x.isUnlocked).FirstOrDefault();
    }

    public SelectedCharacter SwitchCharacter(int avatarId)
    {
        foreach (SelectedCharacter character in characterList)
        {
            if (character.Info.avatarId == avatarId && GetAvatarInfo(avatarId).stageType.ToString() == character.info.stageType.ToString())
            {
                currentCharacter = character;
                character.gameObject.SetActive(true);
            }
            else
            {
                character.gameObject.SetActive(false);
            }
        }        
        return currentCharacter;
    }

    public AvatarInfo GetCurrentAvatarInfo()
    {
        var result = avatarInfoList.Where(x => x.avatarId == SelectedAvatarId).FirstOrDefault();
        return result;
    }

    public AvatarInfo GetAvatarInfo(int avatarId)
    {
        var result = avatarInfoList.Where(x => x.avatarId == avatarId).FirstOrDefault();
        return result;
    }

    public List<AvatarInfo> GetAvatarInfoList()
    {
        return avatarInfoList;
    }

    public IEnumerator RequestCharacters(Action onComplete, Action<string> onFailed)
    {
        Debug.LogError("request character");
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_karakter_data", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    CharacterResponse response = JsonUtility.FromJson<CharacterResponse>(uwr.downloadHandler.text);
                    Debug.LogError("response : " + uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        for (int i = 0; i < response.karakter.Length; i++)
                        {
                            Debug.LogWarning("karakter : " + response.karakter.Length);
                            //AvatarAsset asset = avatarAssetList.Where(x => x.id == response.karakter[i].evolution.Where(y => y.evolution_id == x.id).FirstOrDefault().evolution_id).FirstOrDefault();
                            //liveAvatarInfoList.Add(new AvatarInfo
                            //{
                            //    avatarId = response.karakter[i].karakter_id,
                            //    avatarName = response.karakter[i].nama_karakter,
                            //    characterPrefab = asset.prefab, //Resources.Load<GameObject>("Prefabs/Characters/" + response.karakter[i].karakter_id)
                            //    avatarSprite = asset.sprite
                            //});
                            int index = avatarInfoList.FindIndex(x => x.avatarId == response.karakter[i].karakter_id);
                            if (index >= 0)
                            {
                                avatarInfoList[index].price = response.karakter[i].price;
                                foreach (EvolutionData evolutionData in response.karakter[i].evolution)
                                {
                                    int evoIndex = avatarInfoList[index].evolutionList.FindIndex(x => x.evolutionId == evolutionData.evolution_id);
                                    Debug.LogWarning("evo index : " + evoIndex);
                                    if (index >= 0)
                                    {
                                        avatarInfoList[index].evolutionList[evoIndex].evolutionName = evolutionData.evolution_name;
                                        avatarInfoList[index].evolutionList[evoIndex].experienceToEvolve = evolutionData.experience_to_evolution;
                                    }
                                }
                            }
                        }
                        onComplete?.Invoke();
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
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestCharacters(onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    public IEnumerator RequestItemList(Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"items_id\" : \"" + string.Empty + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_items", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    ItemResponse response = JsonUtility.FromJson<ItemResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        foreach (ItemData item in response.items.accessories.helmet)
                        {
                            Debug.LogWarning("acc Id : " + item.items_id);
                            int index = AccessoryController.Instance.accessoryList.FindIndex(x => x.accessoryId == item.items_id);
                            AccessoryController.Instance.accessoryList[index].accessoryName = item.item_name;
                            AccessoryController.Instance.accessoryList[index].accessoryType = SelectedCharacter.AccessoryType.Helmet;
                            AccessoryController.Instance.accessoryList[index].hasOwned = false;
                        }
                        foreach (ItemData item in response.items.accessories.outfit)
                        {
                            int index = AccessoryController.Instance.accessoryList.FindIndex(x => x.accessoryId == item.items_id);
                            AccessoryController.Instance.accessoryList[index].accessoryName = item.item_name;
                            AccessoryController.Instance.accessoryList[index].accessoryType = SelectedCharacter.AccessoryType.Outfit;
                            AccessoryController.Instance.accessoryList[index].hasOwned = false;
                        }
                        foreach (ItemData item in response.items.charge)
                        {
                            int index = Inventory.Instance.itemInfoList.FindIndex(x => x.itemId == item.items_id);
                            Inventory.Instance.itemInfoList[index].itemName = item.item_name;
                        }
                        onComplete?.Invoke();
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
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestItemList(onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    private AvatarInfo.StageType GetStageType(string evolutionName)
    {
        switch (evolutionName)
        {
            case Consts.BABY:
                return AvatarInfo.StageType.Baby;
            case Consts.TODDLER:
                return AvatarInfo.StageType.Toddler;
            case Consts.TEEN:
                return AvatarInfo.StageType.Teen;
            case Consts.ANDROID:
                return AvatarInfo.StageType.Android;
            case Consts.HUMANOID:
                return AvatarInfo.StageType.Humanoid;
        }

        return AvatarInfo.StageType.Baby;
    }

    public IEnumerator RequestUserData(Action<int> onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance?.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_user_karakter", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    UserResponse response = JsonUtility.FromJson<UserResponse>(uwr.downloadHandler.text);
                    int activeCharacterId = -1;
                    Debug.LogWarning("user data : " + uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.SetCoin(response.user_coin);
                        UserData.username = response.name;
                        HomeController.Instance.RefreshCoins();
                        foreach (UserCharacter ownedCharacter in response.karakter_user)
                        {
                            int index = avatarInfoList.FindIndex(x => x.avatarId == ownedCharacter.karakter_id);
                            if (index >= 0)
                            {
                                avatarInfoList[index].isUnlocked = true;
                                avatarInfoList[index].exp = ownedCharacter.experience;
                                avatarInfoList[index].stageType = GetStageType(ownedCharacter.evolution_name);
                                if (ownedCharacter.accessories_used.helmet_items_id != 0)
                                {
                                    avatarInfoList[index].helmetId = (int)ownedCharacter.accessories_used.helmet_items_id;
                                }
                                if (ownedCharacter.accessories_used.outfit_items_id != 0)
                                {
                                    avatarInfoList[index].outfitId = (int)ownedCharacter.accessories_used.outfit_items_id;
                                }
                            }
                            if (ownedCharacter.is_used == 1)
                            {
                                activeCharacterId = ownedCharacter.karakter_id;

                                DateTime.TryParse(ownedCharacter.status.last_fed, out DateTime lastFed);
                                DateTime.TryParse(response.waktu_server, out DateTime now);

                                TimeSpan gapTime = now - lastFed;
                                double gapInSeconds = gapTime.TotalSeconds;
                                Debug.LogWarning("gap Time : " + gapInSeconds);

                                int energy = ownedCharacter.status.energy;
                                int energyWasted = HomeController.Instance.ConvertSecondsToEnergy(Mathf.FloorToInt((float)gapInSeconds));
                                int energyRemaining = Mathf.Clamp(energy - energyWasted, 0, 100);
                                UserData.SetEnergy(energyRemaining);

                                List<int> requirementList = new List<int>();
                                if (ownedCharacter.need_action.need_clean == 1)
                                {
                                    requirementList.Add((int)Main.RequirementType.NEED_CLEAN_UP);
                                }
                                if (ownedCharacter.need_action.need_charge == 1)
                                {
                                    requirementList.Add((int)Main.RequirementType.NEED_FOOD);
                                }
                                if (ownedCharacter.need_action.need_repair == 1)
                                {
                                    requirementList.Add((int)Main.RequirementType.NEED_FIX_UP);
                                }
                                UserData.SetRequirementList(requirementList);
                                StartCoroutine(NeedsController.Instance.Init());
                            }
                        }
                        foreach (ItemData item in response.inventory.helmet)
                        {
                            int index = AccessoryController.Instance.accessoryList.FindIndex(x => x.accessoryId == item.items_id);
                            AccessoryController.Instance.accessoryList[index].hasOwned = true;
                        }
                        foreach (ItemData item in response.inventory.outfit)
                        {
                            int index = AccessoryController.Instance.accessoryList.FindIndex(x => x.accessoryId == item.items_id);
                            AccessoryController.Instance.accessoryList[index].hasOwned = true;
                        }
                        int energyItemIndex = Inventory.Instance.itemInfoList.FindIndex(x => x.itemId == 1);
                        int hyperItemIndex = Inventory.Instance.itemInfoList.FindIndex(x => x.itemId == 2);
                        int fixItemIndex = Inventory.Instance.itemInfoList.FindIndex(x => x.itemId == 3);
                        Inventory.Instance.itemInfoList[energyItemIndex].itemCount = response.charge_items.energy_charge_stock;
                        Inventory.Instance.itemInfoList[hyperItemIndex].itemCount = response.charge_items.energy_super_charge_stock;
                        Inventory.Instance.itemInfoList[fixItemIndex].itemCount = response.charge_items.fix_charge_stock;
                        onComplete?.Invoke(activeCharacterId);
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
                Debug.LogError("err : " + e.Message);
                onFailed?.Invoke(e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                  new ButtonInfo
                  {
                      content = "Ulangi",
                      onButtonClicked = () => StartCoroutine(RequestUserData(onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }

    public IEnumerator RequestAddExperience(int id, int experience, Action<bool, string> onComplete, Action<string> onFailed)
    {
        Debug.LogWarning("RequestAddExperience");
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + id + "\", \"experience\" : \"" + experience + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "add_experience", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    ExperienceResponse response = JsonUtility.FromJson<ExperienceResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        GetCurrentAvatarInfo().exp = response.new_experience;
                        UserData.GetLevel(1, response.new_experience);
                        HomeController.Instance.RefreshLevel(GetCurrentAvatarInfo());
                        if (response.evolution_up)
                        {
                            if (int.TryParse(response.new_evolution_id, out int targetId))
                            {
                                Evolution(targetId);
                            }
                        }
                        onComplete?.Invoke(response.evolution_up, response.new_evolution_id);
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
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                  new ButtonInfo
                  {
                      content = "Ulangi",
                      onButtonClicked = () => StartCoroutine(RequestAddExperience(id, experience, onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }

    public IEnumerator RequestSetCharacterStatus(string status, string value, Action onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + GetCurrentAvatarInfo().avatarId + "\", \"status\" : { \"" + status + "\" : \"" + value + "\" }}");
        Debug.LogWarning("set value : " + value);
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_status_karakter", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            //try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    CharacterStatusResponse response = JsonUtility.FromJson<CharacterStatusResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.SetEnergy(response.status_karakter.energy);
                        UserData.SetMood((Main.MoodStage)(4 - Mathf.CeilToInt(response.status_karakter.energy / 25)));


                        List<int> requirementList = new List<int>();
                        if (response.need_action.need_clean == 1)
                        {
                            requirementList.Add((int)Main.RequirementType.NEED_CLEAN_UP);
                        }
                        if (response.need_action.need_charge == 1)
                        {
                            requirementList.Add((int)Main.RequirementType.NEED_FOOD);
                        }
                        if (response.need_action.need_repair == 1)
                        {
                            requirementList.Add((int)Main.RequirementType.NEED_FIX_UP);
                        }
                        UserData.SetRequirementList(requirementList);
                        StartCoroutine(NeedsController.Instance.Init());
                        onComplete?.Invoke();
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
            //catch (Exception e)
            //{
            //    LoadingManager.Instance.HideSpinLoading();
            //    onFailed?.Invoke(e.Message);
            //    Debug.LogError("err : " + e.Message);
            //    PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
            //        new ButtonInfo
            //        {
            //            content = "Ulangi",
            //            onButtonClicked = () => StartCoroutine(RequestSetCharacterStatus(status, value, onComplete, onFailed))
            //        },
            //        new ButtonInfo
            //        {
            //            content = "Batal"
            //        });
            //}
        }
    }

    public IEnumerator RequestSetCharacterStatus(string json, Action onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + GetCurrentAvatarInfo().avatarId + "\", \"status\" : {" + json + "}}");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "set_status_karakter", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    CharacterStatusResponse response = JsonUtility.FromJson<CharacterStatusResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.SetEnergy(response.status_karakter.energy);
                        UserData.SetMood((Main.MoodStage)(4 - Mathf.CeilToInt(response.status_karakter.energy / 25)));
                        onComplete?.Invoke();
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
                LoadingManager.Instance.HideSpinLoading();
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestSetCharacterStatus(json, onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    public IEnumerator RequestUpdateCharacter(int characterId, Action onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + characterId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "add_experience", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    ExperienceResponse response = JsonUtility.FromJson<ExperienceResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        LoadingManager.Instance.HideSpinLoading();
                        onComplete?.Invoke();
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
                      onButtonClicked = () => StartCoroutine(RequestUpdateCharacter(characterId, onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }

    public IEnumerator RequestSelectCharacter(int characterId, Action<int> onComplete, Action<string> onFailed)
    {
        LoadingManager.Instance.ShowSpinLoading();
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + characterId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "use_karakter", form))
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
                        onComplete?.Invoke(characterId);
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
                      onButtonClicked = () => StartCoroutine(RequestSelectCharacter(characterId, onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }

    public string GetNextStageEvolution(AvatarInfo info)
    {
        switch (info.stageType)
        {
            case AvatarInfo.StageType.Baby:
                return AvatarInfo.StageType.Toddler.ToString();
            case AvatarInfo.StageType.Toddler:
                return AvatarInfo.StageType.Teen.ToString();
            case AvatarInfo.StageType.Teen:
                return AvatarInfo.StageType.Android.ToString();
            case AvatarInfo.StageType.Android:
                return AvatarInfo.StageType.Humanoid.ToString();
        }
        return "";
    }

    public void CheckEvolution()
    {
        AvatarInfo info = GetCurrentAvatarInfo();
        EvolutionResponse evolution = info.evolutionList.Where(x => x.evolutionName == info.stageType.ToString().ToUpper()).FirstOrDefault();
        if (evolution != null)
        {
            if (info.exp >= evolution.experienceToEvolve)
            {
                Evolution(evolution.evolutionId);
            }
        }
    }

    public void Evolution(int targetId)
    {
        Debug.LogWarning("evolution!!!");
        if (targetId != 0)
        {
            string nextStage = GetNextStageEvolution(GetCurrentAvatarInfo());
            AvatarInfo newInfo = GetCurrentAvatarInfo().Clone();
            newInfo.stageType = GetStageType(nextStage);

            HomeController.Instance.SelectCharacter(newInfo, true);

            //UpdateSelectedAvatar(targetId);
            //EvolutionResponse evolution = GetCurrentAvatarInfo().evolutionList.Where(x => x.evolutionId == targetId).FirstOrDefault();
            currentCharacter.PlayChoosenAnimation();
        }
    }
}
