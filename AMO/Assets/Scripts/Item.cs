using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public AccessoryInfo Info { get; private set; }
    public Image image;

    private Toggle toggle;

    public void Init(ItemLibrary library, AccessoryInfo info, ToggleGroup toggleGroup)
    {
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
    }

    public void SelectItem(bool isOn)
    {
        if(isOn) HomeController.Instance.selectedCharacter.AddAccessory(Info.accessoryId);
    }
}
