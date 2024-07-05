using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public AccessoryInfo Info { get; private set; }
    public Image image;

    private Toggle toggle;
    private ItemLibrary library;

    public void Init(ItemLibrary library, AccessoryInfo info, ToggleGroup toggleGroup)
    {
        this.library = library;
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(SelectItem);
        }
        Info = info;
        if (Info == null)
        {
            toggle.enabled = false;
            image.sprite = library.emptyItemSprite;
        }
        else
        {
            toggle.enabled = true;
            image.sprite = Info.accessorySprite;
        }
        toggle.group = toggleGroup;
        toggle.isOn = false;
    }

    public void SetToggle(bool value)
    {
        toggle.SetIsOnWithoutNotify(value);
        Debug.LogWarning("settoggle " + value, this);
    }

    public void SelectItem(bool isOn)
    {
        Debug.LogWarning("equip item : " + isOn);
        if (isOn)
        {
            Debug.LogWarning("accId : " + Info.accessoryId);
            StartCoroutine(library.RequestEquipItem(Info.accessoryId, (itemId) => { 
                HomeController.Instance.selectedCharacter.AddAccessory(itemId);
                StartCoroutine(Character.Instance.RequestUserData((id) => LoadingManager.Instance.HideSpinLoading(), null));
            }, null));
        }
    }

    public void SelectItem()
    {
        HomeController.Instance.selectedCharacter.AddAccessory(Info.accessoryId, false);
        Character.Instance.currentCharacter.PlayIdleAnimation();
    }
}
