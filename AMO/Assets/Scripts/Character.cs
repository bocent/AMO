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
    public string SelectedAvatarId { get; private set; }

    public static Character Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadAllCharacter();
        UpdateSelectedAvatar();
        //LoadCharacter(SelectedAvatarId);

        StartCoroutine(RequestCharacters(() => StartCoroutine(RequestUserData()), (error) => {
            Debug.LogError("err : " + error);
        }));
        
    }

    public void UpdateSelectedAvatar()
    {
        AvatarInfo info = GetLastStageAvatar(UserData.GetAvatarName());
        SelectedAvatarId = info.avatarId;
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

    private void LoadCharacter(string avatarId)
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

    public SelectedCharacter SwitchCharacter(string avatarId)
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

    public AvatarInfo GetAvatarInfo(string avatarId)
    {
        var result = avatarInfoList.Where(x => x.avatarId == avatarId).FirstOrDefault();
        return result;
    }

    public List<AvatarInfo> GetAvatarInfoList()
    {
        return avatarInfoList;
    }

    public IEnumerator UnlockCharacter(string avatarId)
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
                            AvatarAsset asset = avatarAssetList.Where(x => x.id == response.karakter[i].evolution.Where(y => y.evolution_id == x.id).FirstOrDefault().evolution_id).FirstOrDefault();
                            liveAvatarInfoList.Add(new AvatarInfo
                            {
                                avatarId = response.karakter[i].karakter_id.ToString(),
                                avatarName = response.karakter[i].nama_karakter,
                                characterPrefab = asset.prefab, //Resources.Load<GameObject>("Prefabs/Characters/" + response.karakter[i].karakter_id)
                                avatarSprite = asset.sprite
                            });
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
                    onFailed?.Invoke(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
            }
        }
    }

    public IEnumerator RequestUserData()
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
                    if (response.status.ToLower() == "ok")
                    {
                        UserData.SetCoin(response.user_coin);
                        foreach (UserCharacter ownedCharacter in response.karakter_user)
                        {
                            AvatarInfo info = avatarInfoList.Where(x => x.avatarId == ownedCharacter.karakter_id.ToString()).FirstOrDefault();
                            if (info != null)
                            {
                                info.isUnlocked = true;
                                info.exp = ownedCharacter.experience;
                                UserData.SetMood((Main.MoodStage)(4 - Mathf.RoundToInt(ownedCharacter.status.happiness / 25)));
                                UserData.SetEnergy(ownedCharacter.status.energy);
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
                Debug.LogError("err : " + e.Message);
            }
        }
    }

    public IEnumerator RequestAddExperience(int id, int experience, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"karakter_id\" : \"" + id + "\", \"experience\" : \"" + experience + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "add_experience", form))
        {
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
            }
        }
    }
}
