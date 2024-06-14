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
    public int avatarId;
    public Sprite avatarSprite;
    public string avatarName;
    public int level;
    public int mood;
    public enum StageType
    {
        Baby,
        Toddler,
        Teen,
        Android,
        Humanoid
    }
    public StageType stageType;
    public float energy;
    public int exp;

    public int helmetId;
    public int outfitId;

    public string skinId;
    public List<EvolutionResponse> evolutionList;
    public GameObject characterPrefab;
    public GameObject characterSelectionPrefab;
    public Color color = Color.white;
    public bool isUnlocked;
    public int nextEvolutionId;
}

[Serializable]
public class AvatarAsset
{
    public int id;
    public Sprite sprite;
    public GameObject prefab;
}

[Serializable]
public class EvolutionResponse
{
    public int evolutionId;
    public string evolutionName;
    public int experienceToEvolve;
    public GameObject characterPrefab;
    public GameObject characterSelectionPrefab;
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
