using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterItem : MonoBehaviour
{
    public AvatarInfo Info { get; private set; }

    public Image characterImage;
    public TMP_Text characterNameText;

    private Toggle toggle;

    private void Start()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnSelected);
    }

    public void Init(AvatarInfo info, ToggleGroup toggleGroup)
    {
        Info = info;
        characterImage.sprite = info.avatarSprite;
        characterNameText.text = info.avatarName;


        if (!toggle)
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnSelected);
        }
        toggle.group = toggleGroup;

    }

    private void OnSelected(bool isOn)
    {
        if ((isOn))
        {
            HomeController.Instance.SelectCharacter(Info);
        }
    }
}
