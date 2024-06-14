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
    [SerializeField] private List<AvatarInfo> liveAvatarInfoList;
    [SerializeField] private Transform characterParent;
    [SerializeField] private List<AvatarAsset> avatarAssetList;

    private List<SelectedCharacter> characterList = new List<SelectedCharacter>();

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
                   

                },
                (error) =>
                {

                }));
            },
            (error) =>
            {

            }));
        },
        (error) =>
        {
            Debug.LogError("err : " + error);
        }));

        

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
                   
            }, (error) => 
            {

            }));
        }, (error) =>
        { 
        
        }));
    }

    private void LoadAllCharacter()
    {
        foreach (AvatarInfo info in avatarInfoList)
        {
            GameObject charObj = Instantiate(info.characterPrefab, characterParent, false);
            SelectedCharacter character = charObj.GetComponent<SelectedCharacter>();
            character.Init(info);
            charObj.SetActive(false);
            characterList.Add(character);
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
                HomeController.Instance.SelectCharacter(character.info, false);
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
            if (character.Info.avatarId == avatarId)
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

    public IEnumerator UnlockCharacter(int avatarId)
    {
        foreach (AvatarInfo info in avatarInfoList)
        {
            if (info.avatarId == avatarId)
            {
                info.isUnlocked = true;
            }
        }

        yield return new WaitForSeconds(0.5f);
        unlockCharacterPopup.SetActive(true);
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
                        liveAvatarInfoList = new List<AvatarInfo>();
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
                            int index = avatarInfoList.FindIndex(x =>x.avatarId == response.karakter[i].karakter_id);
                            avatarInfoList[index].evolutionList = new List<EvolutionResponse>();
                            foreach (EvolutionData evolutionData in response.karakter[i].evolution)
                            {
                                avatarInfoList[index].evolutionList.Add(new EvolutionResponse { evolutionId = evolutionData.next_evolution_id, evolutionName = evolutionData.next_evolution_name, experienceToEvolve = evolutionData.experience_to_evolution });
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
            //try
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
            //catch (Exception e)
            //{
            //    onFailed?.Invoke(e.Message);
            //    Debug.LogError("err : " + e.Message);
            //    PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
            //        new ButtonInfo
            //        {
            //            content = "Ulangi",
            //            onButtonClicked = () => StartCoroutine(RequestItemList(onComplete, onFailed))
            //        },
            //        new ButtonInfo
            //        {
            //            content = "Batal"
            //        });
            //}
        }
    }

    public IEnumerator RequestUserData(Action<int> onComplete, Action<string> onFailed)
    {
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
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.SetCoin(response.user_coin);
                        foreach (UserCharacter ownedCharacter in response.karakter_user)
                        {
                            int index = avatarInfoList.FindIndex(x => x.avatarId == ownedCharacter.karakter_id);
                            if (index >= 0)
                            {
                                avatarInfoList[index].isUnlocked = true;
                                avatarInfoList[index].exp = ownedCharacter.experience;
                               
                            }
                            if (ownedCharacter.is_used == 1)
                            {
                                activeCharacterId = ownedCharacter.karakter_id;
                                DateTime.TryParse(ownedCharacter.status.last_fed, out DateTime lastFed);
                                DateTime now = DateTime.Now;

                                TimeSpan gapTime = now - lastFed;
                                double gapInSeconds = gapTime.TotalSeconds;
                                Debug.LogWarning("gap Time : " + gapInSeconds);

                                int energy = ownedCharacter.status.energy;
                                int energyWasted = HomeController.Instance.ConvertSecondsToEnergy(Mathf.FloorToInt((float)gapInSeconds));
                                int energyRemaining = Mathf.Clamp(energy - energyWasted, 0, 100);

                                Debug.LogWarning("energy : " + energyRemaining + " " + ownedCharacter.status.energy);

                                UserData.SetMood((Main.MoodStage)(4 - Mathf.RoundToInt(ownedCharacter.status.happiness / 25)));
                                UserData.SetEnergy(energyRemaining);
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
                        onComplete?.Invoke(response.evolution_up, response.new_evolution_id);
                        if (response.evolution_up)
                        {
                            if (int.TryParse(response.new_evolution_id, out int targetId))
                            {
                                Evolution(targetId);
                            }
                        }
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

    public IEnumerator RequestUpdateCharacter(int characterId, Action onComplete, Action<string> onFailed)
    {
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
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
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
        EvolutionResponse evolution = info.evolutionList.Where(x => x.evolutionName == GetNextStageEvolution(info).ToUpper()).FirstOrDefault();
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
        if (targetId != 0)
        {
            UpdateSelectedAvatar(targetId);
        }
    }
}
