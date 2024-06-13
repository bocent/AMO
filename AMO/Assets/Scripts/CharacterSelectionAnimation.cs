using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionAnimation : MonoBehaviour
{
    public AvatarInfo info;
    public GameObject[] equippedAccessories;
    public int index;
    private CharacterAnimation characterAnimation;

    private void Start()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
    }

    public void OnCharacterSelected()
    {
        PlayChoosenAnimation();
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
}
