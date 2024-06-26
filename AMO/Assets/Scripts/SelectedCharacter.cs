using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
    private List<CharacterAnimation> characterAnimations = new List<CharacterAnimation>();

    private CharacterAnimation characterAnimation;
    private const string HELMET_KEY = "helmet";
    private const string OUTFIT_KEY = "outfit";


    private void Awake()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
        voiceSource = GetComponent<AudioSource>();
    }

    public IEnumerator Init(AvatarInfo info)
    {
        Info = info;
        this.info = info;
        //int helmetId = LoadAccessory(AccessoryType.Helmet);
        //int outfitId = LoadAccessory(AccessoryType.Outfit);
        AddAccessory(info.helmetId, false);
        AddAccessory(info.outfitId, false);
        yield return new WaitForSeconds(0.5f);
        SetEnergyAndMood();
        //PlayIdleAnimation();
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

    public IEnumerator PlayIdleAnimation()
    {
        yield return null;
        Debug.LogWarning("Play Idle Animation");
        string conditionName = "Idle";
        if (characterAnimation)
        {
            float time = characterAnimation.SetAnimationCondition(conditionName);
            //foreach (GameObject equippedAccessory in equippedAccessories)
            //{
            //    CharacterAnimation characterAnim = equippedAccessory.GetComponent<CharacterAnimation>();
            //    if (characterAnim) characterAnim.SetIdleAnimation(conditionName, time);
            //    CharacterAnimation[] animations = equippedAccessory.GetComponentsInChildren<CharacterAnimation>();
            //    foreach (CharacterAnimation animation in animations)
            //    {
            //        animation.SetIdleAnimation(conditionName, time);
            //    }
            //}
            foreach (CharacterAnimation anim in characterAnimations)
            {
                anim.SetIdleAnimation(conditionName, time);
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

    public GameObject AddAccessory(int accessoryId, bool useAnimation = true)
    {
        Debug.LogWarning("acc id : " + accessoryId);
        if (accessoryId > 0)
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
                Info.helmetId = 0;
                break;
            case AccessoryType.Outfit:
                if (outfitAccessory)
                {
                    Destroy(outfitAccessory);
                    outfitAccessory = null;
                }
                Info.outfitId = 0;
                break;
        }
    }

    private GameObject AddHelmetAccessory(AccessoryInfo info)
    {
        Debug.LogError("helmet acc : " + helmetAccessory);
        if (helmetAccessory != null)
        {
            characterAnimations.Remove(helmetAccessory.GetComponent<CharacterAnimation>());
            equippedAccessories.Remove(helmetAccessory.gameObject);
            Destroy(helmetAccessory.gameObject);
        }
        if (info.accessoryPrefabs != null)
        {
            GameObject prefab = AccessoryController.Instance.GetAccessoryPrefab(info, Info.stageType);
            //Debug.Log("prefab : " + prefab.name);
            if (prefab != null)
            {
                GameObject head = Instantiate(prefab, transform, false);
                head.transform.localEulerAngles = Vector3.zero;
                helmetAccessory = head.GetComponent<Accessory>();
                helmetAccessory.Init(info);
                Info.helmetId = info.accessoryId;
                equippedAccessories.Add(head);
                characterAnimations.Add(head.GetComponent<CharacterAnimation>());

                //SaveAccessory(AccessoryType.Helmet, info.accessoryId);
                PlayIdleAnimation();
                return head;
            }
            else
            {
                Info.helmetId = info.accessoryId;
                //SaveAccessory(AccessoryType.Helmet, info.accessoryId);
            }
        }
        else
        {
            Info.helmetId = info.accessoryId;
            //SaveAccessory(AccessoryType.Helmet, info.accessoryId);
        }
        return null;
    }

    private GameObject AddOutfitAccessory(AccessoryInfo info)
    {
        Debug.LogWarning("outfit id : " + Info.outfitId);
        
        Debug.LogWarning("outfit : " + outfitAccessory);
        if (outfitAccessory != null)
        {
            foreach (CharacterAnimation anim in outfitAccessory.GetComponentsInChildren<CharacterAnimation>())
            {
                characterAnimations.Remove(anim);
            }
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
                characterAnimations.AddRange(body.GetComponentsInChildren<CharacterAnimation>());
                //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
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
                    //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                    PlayIdleAnimation();
                }
                else
                {
                    Info.outfitId = info.accessoryId;
                    //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                }
            }
            else
            {
                Info.outfitId = info.accessoryId;
                //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
            }
        }
        else if (info.materials != null)
        {
            Material material = AccessoryController.Instance.GetAccessoryMaterial(info, Info.stageType);
            if (material)
            {
                Info.outfitId = info.accessoryId;
                bodyMeshRenderer.material = material;
                //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
                PlayIdleAnimation();
            }
            else
            {
                Info.outfitId = info.accessoryId;
                //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
            }
        }
        else
        {
            Info.outfitId = info.accessoryId;
            //SaveAccessory(AccessoryType.Outfit, info.accessoryId);
        }
        return null;
    }

    //private void SaveAccessory(AccessoryType accessoryType, int accesoryId)
    //{
    //    if (accessoryType == AccessoryType.Helmet)
    //    {
    //        PlayerPrefs.SetInt(HELMET_KEY + info.avatarName, accesoryId);
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetInt(OUTFIT_KEY + info.avatarName, accesoryId);
    //    }
    //    PlayerPrefs.Save();
    //}

    //private int LoadAccessory(AccessoryType accessoryType)
    //{
    //    if (accessoryType == AccessoryType.Helmet)
    //    {
    //        int helmetId = info.helmetId == 0 ? GetDefaultAccessory(accessoryType) : info.helmetId;
    //        return PlayerPrefs.HasKey(HELMET_KEY + info.avatarName) ? PlayerPrefs.GetInt(HELMET_KEY + info.avatarName) : helmetId;
    //    }
    //    else
    //    {
    //        int outfitId = info.outfitId == 0 ? GetDefaultAccessory(accessoryType) : info.outfitId;
    //        return PlayerPrefs.HasKey(OUTFIT_KEY + info.avatarName) ? PlayerPrefs.GetInt(OUTFIT_KEY + info.avatarName) : outfitId;
    //    }
    //}

    private int GetDefaultAccessory(AccessoryType accessoryType)
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
        return 0;
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

    //private void TransferStats(int avatarId)
    //{
    //    AvatarInfo targetAvatarInfo = Character.Instance.GetAvatarInfo(avatarId);
    //    Debug.LogError("avatar id : " + targetAvatarInfo.avatarId + " " + targetAvatarInfo.stageType.ToString());
    //    //targetAvatarInfo.mood = Info.mood;
    //    //targetAvatarInfo.energy = Info.energy;
    //    targetAvatarInfo.level = Info.level;
    //    targetAvatarInfo.isUnlocked = true;
    //    HomeController.Instance.SelectCharacter(targetAvatarInfo);
    //    Info = targetAvatarInfo;
    //}

    private void LateUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    PlayChoosenAnimation();
        //}

        
        //float moodValue = 0;
        //int index = HomeController.Instance.energyController.energyMeterList.FindIndex(x => x.maxEnergy >= UserData.Energy && x.minEnergy < UserData.Energy);
        //if (index >= 0)
        //{
        //    moodValue = index * 0.25f;
        //}
        SetEnergyAndMood();
    }

    public void SetEnergyAndMood()
    {
        string MOOD = "Mood";
        string ENERGY = "Energy";

        float moodValue = 0;
        int index = HomeController.Instance.energyController.energyMeterList.FindIndex(x => x.maxEnergy >= UserData.Energy && x.minEnergy < UserData.Energy);
        if (index >= 0)
        {
            moodValue = index * 0.25f;
        }

        characterAnimation.SetAnimationCondition(MOOD, moodValue);
        characterAnimation.SetAnimationCondition(ENERGY, (int)UserData.Energy);
        foreach (CharacterAnimation anim in characterAnimations)
        {
            anim.SetAnimationCondition(MOOD, moodValue);
            anim.SetAnimationCondition(ENERGY, (int)UserData.Energy);
        }
    }
}
