using System.Collections;
using System.Collections.Generic;
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

    private List<GameObject> equippedAccessories = new List<GameObject>();

    private CharacterAnimation characterAnimation;

    private void Awake()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
    }

    private void Start()
    {
        AddAccessory(Info.helmetId);
        AddAccessory(Info.outfitId);
        //SetSkin("mochi_default");
    }

    public void Init(AvatarInfo info)
    {
        Info = info;
        this.info = info;
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

    public void PlayIdleAnimation()
    {
        string conditionName = "Idle";
        Debug.LogError("character anim : " + characterAnimation, gameObject);
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

    public void PlayChoosenAnimation()
    {
        string conditionName = "Choose";
        characterAnimation.SetAnimationCondition(conditionName, true);
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

    public GameObject AddAccessory(string accessoryId)
    {
        if (!string.IsNullOrEmpty(accessoryId))
        {
            AccessoryInfo info = AccessoryController.Instance.GetAccessoryInfo(accessoryId);
            switch (info.accessoryType)
            {
                case AccessoryType.Helmet:
                    return AddHelmetAccessory(info);
                case AccessoryType.Outfit:
                    return AddOutfitAccessory(info);
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
        if (Info.helmetId != info.accessoryId)
        {
            Debug.LogError("helmet acc : " + helmetAccessory);
            if (helmetAccessory != null)
            {
                equippedAccessories.Remove(helmetAccessory.gameObject);
                Destroy(helmetAccessory.gameObject);
            }
            if (info.accessoryPrefab != null)
            {
                GameObject head = Instantiate(info.accessoryPrefab, transform, false);
                head.transform.localEulerAngles = Vector3.zero;
                helmetAccessory = head.GetComponent<Accessory>();
                helmetAccessory.Init(info);
                Info.helmetId = info.accessoryId;
                equippedAccessories.Add(head);
                PlayIdleAnimation();
                return head;
            }
        }
        return null;
    }

    private GameObject AddOutfitAccessory(AccessoryInfo info)
    {
        if (Info.outfitId != info.accessoryId)
        {
            if (outfitAccessory != null)
            {
                equippedAccessories.Remove(outfitAccessory.gameObject);
                Destroy(outfitAccessory.gameObject);
            }
            if (info.accessoryPrefab != null)
            {
                GameObject body = Instantiate(info.accessoryPrefab, transform, false);
                body.transform.localEulerAngles = Vector3.zero;
                outfitAccessory = body.GetComponent<Accessory>();
                outfitAccessory.Init(info);
                Info.outfitId = info.accessoryId;
                equippedAccessories.Add(body);
                PlayIdleAnimation();
                return body;
            }
        }
        return null;
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

    public void SetSkin(string skinId)
    {
        Info.skinId = skinId;
        string maskId = GetBodyMaskId();
        if (!string.IsNullOrEmpty(GetBodyMaskId()))
        {
            skinId = Info.skinId + maskId;
        }

        SkinInfo skinInfo = AccessoryController.Instance.GetSkinInfo(skinId);

        bodyMeshRenderer.material = skinInfo.material;

    }

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
        string currentAvatarId = Info.avatarId;
        string[] splittedAvatarId = currentAvatarId.Split("_");
        if (int.TryParse(splittedAvatarId[1], out int avatarStageCode))
        {
            avatarStageCode += 1;
            Info.isUnlocked = false;
            TransferStats(splittedAvatarId[0] + "_" + avatarStageCode);
        }
    }

    private void TransferStats(string avatarId)
    {
        AvatarInfo targetAvatarInfo = Character.Instance.GetAvatarInfo(avatarId);
        Debug.LogError("avatar id : " + targetAvatarInfo.avatarId + " " + targetAvatarInfo.stageType.ToString());
        targetAvatarInfo.mood = Info.mood;
        targetAvatarInfo.energy = Info.energy;
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

        
    }
}
