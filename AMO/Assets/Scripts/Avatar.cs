using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AvatarInfo
{
    public string avatarId;
    public Sprite avatarSprite;
    public string avatarName;
    public int level;
    public int mood;
    public enum StageType
    {
        Baby,
        Toddler,
        Teen,
        Adult
    }
    public StageType stageType;
    public float energy;
    public int exp;

    public string helmetId;
    public string outfitId;

    public string skinId;

    public GameObject characterPrefab;
    public bool isUnlocked;
}

public class Avatar : MonoBehaviour
{
    [SerializeField] private Image avatarPanel;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text avatarNameText;

    private Button switchCharacterButton;

    private AvatarInfo info;

    private void Start()
    {
        switchCharacterButton = GetComponent<Button>();
        switchCharacterButton.onClick.AddListener(OpenCharacterSelection);
    }

    public void SetAvatar(AvatarInfo info)
    {
        this.info = info;
        avatarImage.sprite = info.avatarSprite;
        avatarNameText.text = info.avatarName;
    }

    private void OpenCharacterSelection()
    {
        Debug.LogError("OpenCharacterSelection");
        HomeController.Instance.ShowCharacterSelection(true);
    }
}
