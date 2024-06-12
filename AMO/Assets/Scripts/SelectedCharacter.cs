using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedCharacter : MonoBehaviour
{
    public AvatarInfo Info { get; private set; }
    public AvatarInfo info;

    public enum AccessoryType
    {
        Helmet,
        Outfit
    }

    public Accessory helmetAccessory;
    public Accessory outfitAccessory;
    //public Accessory bodyAccessory;
    //public Accessory shoesAccessory;

    public SkinnedMeshRenderer bodyMeshRenderer;
    private AudioSource voiceSource;

    private List<GameObject> equippedAccessories = new List<GameObject>();

    private CharacterAnimation characterAnimation;
    private const string HELMET_KEY = "helmet";
    private const string OUTFIT_KEY = "outfit";


    private void Awake()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
        voiceSource = GetComponent<AudioSource>();
    }

    public void Init(AvatarInfo info)
    {
        Info = info;
        this.info = info;
        string helmetId = LoadAccessory(AccessoryType.Helmet);
        string outfitId = LoadAccessory(AccessoryType.Outfit);
        AddAccessory(helmetId, false);
        AddAccessory(outfitId, false);
    }

    public int GetMood()
    {
        return Info.mood;
    }

    public float GetEnergy()
    {
        return Info.energy;
    }

    public void AddMood(int value)
    {
        Info.mood += value;
        characterAnimation.SetAnimationCondition("mood", Info.mood);
    }

    public void AddEnergy(int value)
    {
        Info.energy += value;
        characterAnimation.SetAnimationCondition("energy", Info.mood);
    }

    public void AddExp(int value)
    {
        Info.exp += value;
    }

    public void ConsumeMood(int value)
    {
        Info.mood -= value;
        characterAnimation.SetAnimationCondition("mood", Info.mood);
    }

    public void ConsumeEnergy(int value)
    {
        Info.energy -= value;
        characterAnimation.SetAnimationCondition("energy", Info.mood);
    }

    public void SetMood(int value)
    {
        Info.mood = value;
        characterAnimation.SetAnimationCondition("mood", Info.mood);
    }

    public void SetEnergy(int value)
    {
        Info.energy = value;
        characterAnimation.SetAnimationCondition("energy", Info.mood);
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.clip = clip;
        voiceSource.Play();    
    }

    public void PlayIdleAnimation()
    {
        string conditionName = "Idle";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName);
                }
            }
        }
    }

    public void PlayObtainCharacter()
    {
        string conditionName = "Obtained";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName);
                }
            }
        }
    }

    public void PlayChoosenAnimation()
    {
        Debug.LogError("play choose animation ", this);
        string conditionName = "Choose";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName);
                }
            }
        }
    }

    public void PlayEatAnimation()
    {
        string conditionName = "Eat";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName);
                }
            }
        }
    }

    public void PlayDressUpAnimation()
    {
        string conditionName = "FinishDressUp";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName);
                }
            }
        }
    }

    public void PlayCleanUpAnimation(bool value)
    {
        string conditionName = "CleanUp";
        if (characterAnimation)
        {
            characterAnimation.SetAnimationCondition(conditionName, value);
            foreach (GameObject equippedAccessory in equippedAccessories)
            {
                CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
                if (characterAnim) characterAnim.SetAnimationCondition(conditionName, value);
                CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
                foreach (CharacterAnimation animation in animations)
                {
                    animation.SetAnimationCondition(conditionName, value);
                }
            }
        }
    }

    public GameObject AddAccessory(string accessoryId, bool useAnimation = true)
    {
        Debug.LogWarning("acc id : " + accessoryId);
        if (!string.IsNullOrEmpty(accessoryId))
        {
            AccessoryInfo info = AccessoryController.Instance.GetAccessoryInfo(accessoryId);
            if (info != null)
            {
                Debug.LogWarning("acc info : " + info);
                switch (info.accessoryType)
                {
                    case AccessoryType.Helmet:
                        return AddHelmetAccessory(info);
                    case AccessoryType.Outfit:
                        return AddOutfitAccessory(info);
                }
                if (useAnimation) PlayDressUpAnimation();
            }
        }
        return null;
    }

    public void RemoveAccessory(AccessoryType bodyPart)
    {
        switch (bodyPart)
        {
            case AccessoryType.Helmet:
                if (helmetAccessory)
                {
                    Destroy(helmetAccessory);
                    helmetAccessory = null;
                }
                Info.helmetId = null;
                break;
            case AccessoryType.Outfit:
                if (outfitAccessory)
                {
                    Destroy(outfitAccessory);
                    outfitAccessory = null;
                }
                Info.outfitId = null;
                break;
        }
    }

    private GameObject AddHelmetAccessory(AccessoryInfo info)
    {
        Debug.LogError("helmet acc : " + helmetAccessory);
        if (helmetAccessory != null)
        {
            equippedAccessories.Remove(helmetAccessory.gameObject);
            Destroy(helmetAccessory.gameObject);
        }
        if (info.accessoryPrefabs != null)
        {
            GameObject prefab = AccessoryController.Instance.GetAccessoryPrefab(info, Info.stageType);
            if (prefab != null)
            {
                GameObject head = Instantiate(prefab, transform, false);
                head.transform.localEulerAngles = Vector3.zero;
                helmetAccessory = head.GetComponent<Accessory>();
                helmetAccessory.Init(info);
                Info.helmetId = info.accessoryId;
                equippedAccessories.Add(head);

                SaveAccessory(AccessoryType.Helmet, info.accessoryId);
                PlayIdleAnimation();
                return head;
            }
            else
            {
                Info.helmetId = info.accessoryId;
                SaveAccessory(AccessoryType.Helmet, info.accessoryId);
            }
        }
        else
        {
            Info.helmetId = info.accessoryId;
            SaveAccessory(AccessoryType.Helmet, info.accessoryId);
        }
        return null;
    }

    private GameObject AddOutfitAccessory(AccessoryInfo info)
    {
        Debug.LogWarning("outfit id : " + Info.outfitId);
        
        Debug.LogWarning("outfit : " + outfitAccessory);
        if (outfitAccessory != null)
        {
            equippedAccessories.Remove(outfitAccessory.gameObject);
            Destroy(outfitAccessory.gameObject);
            outfitAccessory = null;
        }
        if (info.accessoryPrefabs != null)
        {
            GameObject prefab = AccessoryController.Instance.GetAccessoryPrefab(info, Info.stageType);
            if (prefab != null)
            {
                GameObject body = Instantiate(prefab, transform, false);
                body.transform.localEulerAngles = Vector3.zero;
                outfitAccessory = body.GetComponent<Accessory>();
                outfitAccessory.Init(info);
                Info.outfitId = info.accessoryId;
                equippedAccessories.Add(body);
                SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                PlayIdleAnimation();
                return body;
            }
            else if (info.materials != null)
            {
                Material material = AccessoryController.Instance.GetAccessoryMaterial(info, Info.stageType);
                if (material)
                {
                    Info.outfitId = info.accessoryId;
                    bodyMeshRenderer.material = material;
                    SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                    PlayIdleAnimation();
                }
                else
                {
                    Info.outfitId = info.accessoryId;
                    SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                }
            }
            else
            {
                Info.outfitId = info.accessoryId;
                SaveAccessory(AccessoryType.Outfit, info.accessoryId);
            }
        }
        else if (info.materials != null)
        {
            Material material = AccessoryController.Instance.GetAccessoryMaterial(info, Info.stageType);
            if (material)
            {
                Info.outfitId = info.accessoryId;
                bodyMeshRenderer.material = material;
                SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                PlayIdleAnimation();
            }
            else
            {
                Info.outfitId = info.accessoryId;
                SaveAccessory(AccessoryType.Outfit, info.accessoryId);
            }
        }
        else
        {
            Info.outfitId = info.accessoryId;
            SaveAccessory(AccessoryType.Outfit, info.accessoryId);
        }
        return null;
    }

    private void SaveAccessory(AccessoryType accessoryType, string accesoryId)
    {
        if (accessoryType == AccessoryType.Helmet)
        {
            PlayerPrefs.SetString(HELMET_KEY + info.avatarName, accesoryId);
        }
        else
        {
            PlayerPrefs.SetString(OUTFIT_KEY + info.avatarName, accesoryId);
        }
        PlayerPrefs.Save();
    }

    private string LoadAccessory(AccessoryType accessoryType)
    {
        if (accessoryType == AccessoryType.Helmet)
        {
            string helmetId = info.helmetId == "" ? GetDefaultAccessory(accessoryType) : info.helmetId;
            return PlayerPrefs.HasKey(HELMET_KEY + info.avatarName) ? PlayerPrefs.GetString(HELMET_KEY + info.avatarName) : helmetId;
        }
        else
        {
            string outfitId = info.outfitId == "" ? GetDefaultAccessory(accessoryType) : info.outfitId;
            return PlayerPrefs.HasKey(OUTFIT_KEY + info.avatarName) ? PlayerPrefs.GetString(OUTFIT_KEY + info.avatarName) : outfitId;
        }
    }

    private string GetDefaultAccessory(AccessoryType accessoryType)
    {
        if (accessoryType == AccessoryType.Outfit)
        {
            switch (Info.avatarName)
            {
                case Consts.AROHA:
                    return AccessoryController.DEFAULT_AROHA_OUTFIT;
                case Consts.MOCHI:
                    return AccessoryController.DEFAULT_MOCHI_OUTFIT;
                case Consts.GILMO:
                    return AccessoryController.DEFAULT_GILMO_OUTFIT;
                case Consts.OLGA:
                    return AccessoryController.DEFAULT_OLGA_OUTFIT;
                case Consts.LORRY:
                    return AccessoryController.DEFAULT_LORRY_OUTFIT;
            }
        }
        else if (accessoryType == AccessoryType.Helmet)
        {
            switch (Info.avatarName)
            {
                case Consts.AROHA:
                    return AccessoryController.DEFAULT_AROHA_HELMET;
                case Consts.MOCHI:
                    return AccessoryController.DEFAULT_MOCHI_HELMET;
                case Consts.GILMO:
                    return AccessoryController.DEFAULT_GILMO_HELMET;
                case Consts.OLGA:
                    return AccessoryController.DEFAULT_OLGA_HELMET;
                case Consts.LORRY:
                    return AccessoryController.DEFAULT_LORRY_HELMET;
            }
        }
        return "";
    }

    //private GameObject AddHandAccessory(AccessoryInfo info)
    //{
    //    if (handAccessory != null)
    //    {
    //        Destroy(handAccessory);
    //    }
    //    if (info.accessoryPrefab != null)
    //    {
    //        GameObject hand = Instantiate(info.accessoryPrefab, transform, false);
    //        hand.transform.localEulerAngles = Vector3.zero;
    //        return hand;
    //    }
    //    return null;
    //}

    //private GameObject AddShoesAccessory(AccessoryInfo info)
    //{
    //    if (shoesAccessory != null)
    //    {
    //        Destroy(shoesAccessory);
    //    }
    //    if (info.accessoryPrefab != null)
    //    {
    //        GameObject shoes = Instantiate(info.accessoryPrefab, transform, false);
    //        shoes.transform.localEulerAngles = Vector3.zero;
    //        return shoes;
    //    }
    //    return null;
    //}

    private string GetBodyMaskId()
    {
        if (outfitAccessory)
        {
            if (!string.IsNullOrEmpty(outfitAccessory.Info.maskId))
            {
                return outfitAccessory.Info.maskId;
            }
        }
        return null;
    }

    public void Evolution()
    {
        int currentAvatarId = Info.avatarId;
        {
            Info.isUnlocked = false;
            Evolution evolution = Info.evolutionList.Where(x => x.evolutionId == Info.nextEvolutionId).FirstOrDefault();
            if (evolution != null)
            {
                TransferStats(Info.nextEvolutionId);
            }
        }
    }

    private void TransferStats(int avatarId)
    {
        AvatarInfo targetAvatarInfo = Character.Instance.GetAvatarInfo(avatarId);
        Debug.LogError("avatar id : " + targetAvatarInfo.avatarId + " " + targetAvatarInfo.stageType.ToString());
        //targetAvatarInfo.mood = Info.mood;
        //targetAvatarInfo.energy = Info.energy;
        targetAvatarInfo.level = Info.level;
        targetAvatarInfo.isUnlocked = true;
        HomeController.Instance.SelectCharacter(targetAvatarInfo);
        Info = targetAvatarInfo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayChoosenAnimation();
        }

        string MOOD = "Mood";
        string ENERGY = "Energy";
        float moodValue = ((int)UserData.Mood + 1) / 4f;
        characterAnimation.SetAnimationCondition(MOOD, moodValue);
        characterAnimation.SetAnimationCondition(ENERGY, (int)UserData.Energy);
        foreach (GameObject equippedAccessory in equippedAccessories)
        {
            CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
            if (characterAnim)
            {
                characterAnim.SetAnimationCondition(MOOD, moodValue);
                characterAnim.SetAnimationCondition(ENERGY, (int)UserData.Energy);
            }
            CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
            foreach (CharacterAnimation animation in animations)
            {
                animation.SetAnimationCondition(MOOD, moodValue);
                animation.SetAnimationCondition(ENERGY, (int)UserData.Energy);
            }
        }
    }
}
