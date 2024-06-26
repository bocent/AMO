using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[Serializable]
public class  EvolutionTree
{
    public GameObject parent;
    public Image image;
    public GameObject nextArrow;
}

public class CharacterSelection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject characterItemPrefab;
    public Transform characterItemParent;
    public Button backButton;

    public GameObject container;
    public Image background;
    public Image nameBackground;
    public TMP_Text nameText;
    public TMP_Text evolutionText;

    public List<EvolutionTree> evolutionTreeList;

    private List<CharacterSelectionAnimation> characterItemList = new List<CharacterSelectionAnimation>();
    private List<CharacterSelectionAnimation> characterList = new List<CharacterSelectionAnimation>();

    private bool isShown = false;
    private float minSwipeLength = 50f;
    private float swipeDistance;
    private Vector2 touchPos;

    public List<Transform> characterPlaceholderList;

    private void Start()
    {
        //simpleScrollSnap.OnPanelCentered.AddListener(OnItemSelected);
        backButton.onClick.AddListener(Hide);
        ChangeColor(Character.Instance.GetCurrentAvatarInfo());
    }

    private void OnEnable()
    {
        StartCoroutine(WaitToRefresh());
    }

    private void Hide()
    {
        Show(false);
        isShown = false;
        //Character.Instance.RequestSelectCharacter(Character.Instance.currentCharacter.);
    }

    public void Show(bool value)
    {
        isShown = value;
        Debug.LogWarning("Show selection");
        container.SetActive(value);
        HomeController.Instance.ShowCharacterSelectionRoom(value);
        HomeController.Instance.ShowHome(!value);
        HomeController.Instance.ShowHUD(!value);

        if (value)
        {
            SetEvolutionTree(Character.Instance.GetCurrentAvatarInfo());
        }

        //for (int i = 0; i < simpleScrollSnap.Content.childCount; i++)
        //{
        //    Debug.LogWarning("avatar id : " + Character.Instance.SelectedAvatarId + " " + simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId);
        //    if (simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId == Character.Instance.SelectedAvatarId)
        //    {
        //        simpleScrollSnap.GoToPanel(i);
        //        break;
        //    }
        //}

    }

    public void SetEvolutionTree(AvatarInfo info)
    {
        foreach (EvolutionTree tree in evolutionTreeList)
        {
            tree.parent.SetActive(false);
        }
        if (evolutionTreeList.Count >= info.evolutionList.Count)
        {
            for (int i = 0; i < info.evolutionList.Count; i++)
            {
                evolutionTreeList[i].parent.SetActive(true);
                evolutionTreeList[i].image.sprite = info.evolutionList[i].avatarSprite;
                if (evolutionTreeList[i].nextArrow)
                {
                    evolutionTreeList[i].nextArrow.SetActive(i != info.evolutionList.Count - 1);
                }
            }
        }
    }
    private IEnumerator WaitToRefresh()
    {
        yield return null;
        //for (int i = 0; i < simpleScrollSnap.Content.childCount; i++)
        //{
        //    Debug.LogWarning("avatar id : " + Character.Instance.SelectedAvatarId + " " + simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId);
        //    if (simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId == Character.Instance.SelectedAvatarId)
        //    {
        //        simpleScrollSnap.GoToPanel(i);
        //        break;
        //    }
        //}
    }

    private int GetPositionIndex(int index)
    {
        if (index > 0)
        {
            index = index % (characterPlaceholderList.Count);
        }
        return index;
    }

    private int GetIndex(int index)
    {
        if (index > 0)
        {
            index = index % (characterList.Count);
        }
        else if (index < 0)
        {
            index = characterList.Count + index;
        }
        return index;
    }

    public void Init()
    {
        Debug.LogWarning("Init : " + Character.Instance.SelectedAvatarId);
        int unlockedCharacterCount = 0;
        List<AvatarInfo> infoList = Character.Instance.GetAvatarInfoList().Where(x => x.isUnlocked == true).ToList();
        Debug.LogWarning("get unlocked character count : " + infoList.Count);
        int centerIndex = 0;
        int targetIndex = infoList.FindIndex(x => x.avatarId == Character.Instance.SelectedAvatarId);
        int indexPos = GetPositionIndex(centerIndex - targetIndex);
        int index = centerIndex - targetIndex;
        Debug.LogWarning("first index : " + index);
        for (int i = 0; i < infoList.Count; i++)
        {
            AvatarInfo info = infoList[i];
            EvolutionResponse data = info.evolutionList.Where(x => x.evolutionName == info.stageType.ToString().ToUpper()).FirstOrDefault();
            if (data != null)
            {
                GameObject item = Instantiate(data.characterSelectionPrefab, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();
                characterItem.info = info;
                characterList.Add(characterItem);
                characterItem.index = index;
                if (i < characterPlaceholderList.Count)
                {
                    Debug.LogWarning("index : " + indexPos);
                    //item.transform.position = characterPlaceholderList[indexPos].position;
                    characterItemList.Add(characterItem);
                    unlockedCharacterCount += 1;
                    //indexPos = GetPositionIndex(indexPos + 1);
                }
                else
                {
                    //Witem.SetActive(false);                
                }
            }
            index += 1;
        }
        if (unlockedCharacterCount == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject item = Instantiate(characterItemList[0].gameObject, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();
                characterItem.index = index;
                characterList.Add(characterItem);
                index += 1;
            }
        }
        else if (unlockedCharacterCount == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < characterItemList.Count; j++)
                {
                    GameObject item = Instantiate(characterItemList[j].gameObject, characterItemParent, false);
                    CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();
                    characterItem.index = index;
                    characterList.Add(characterItem);
                    index += 1;
                }
            }
        }
        else if (unlockedCharacterCount < 6)
        {
            for (int i = 0; i < characterItemList.Count; i++)
            {
                GameObject item = Instantiate(characterItemList[i].gameObject, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();
                characterItem.index = index;
                characterList.Add(characterItem);
                index += 1;
            }
        }

        characterList = characterList.OrderBy(x => x.index).ToList();
        for (int i = 0; i < characterList.Count; i++)
        {
            characterList[i].index = GetIndex(characterList[i].index);
        }
        Reposition();
    }

    private void Reposition(bool withAnimation = false)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].index < 2)
            {
                if (withAnimation)
                {
                    characterList[i].transform.DOMove(characterPlaceholderList[characterList[i].index].position, 0.2f).SetEase(Ease.InOutQuad).Play();
                }
                else
                {
                    characterList[i].transform.position = characterPlaceholderList[characterList[i].index].position;
                }
            }
            else if (characterList[i].index >= characterList.Count - 2)
            {
                int lastIndex = characterList.Count - characterList[i].index;
                if (withAnimation)
                {
                    characterList[i].transform.DOMove(characterPlaceholderList[characterPlaceholderList.Count - lastIndex].position, 0.2f).SetEase(Ease.InOutQuad).Play();
                }
                else
                {
                    characterList[i].transform.position = characterPlaceholderList[characterPlaceholderList.Count - lastIndex].position;
                }
            }
            else
            {
                if (withAnimation)
                {
                    characterList[i].transform.DOMove(characterPlaceholderList[3].position, 0.2f).SetEase(Ease.InOutQuad).Play();
                }
                else
                {
                    characterList[i].transform.position = characterPlaceholderList[3].position;
                }
            }
        }
    }

    private void OnItemSelected(int to, int from)
    {
        //simpleScrollSnap.Content.GetChild(to).GetComponent<Toggle>().isOn = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        swipeDistance = 0f;
        touchPos = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 releasePos = Input.mousePosition;

        swipeDistance = Vector2.Distance(touchPos, releasePos);

        if (releasePos.x > touchPos.x + minSwipeLength)
        {
            NextCharacter();
        }
        else if (releasePos.x < touchPos.x - minSwipeLength)
        {
            PrevCharacter();
        }
    }

    private void NextCharacter()
    {
        Debug.LogWarning("next character");
        for (int i = 0; i < characterList.Count; i++)
        {
            characterList[i].index = GetIndex(characterList[i].index + 1);
        }
        Reposition(true);
        SelectCharacter(GetSelectedCharacter());
    }

    private void PrevCharacter()
    {
        Debug.LogWarning("prev character");
        for (int i = 0; i < characterList.Count; i++)
        {
            characterList[i].index = GetIndex(characterList[i].index - 1);
        }
        Reposition(true);
        SelectCharacter(GetSelectedCharacter());
    }

    private AvatarInfo GetSelectedCharacter()
    {
        CharacterSelectionAnimation character = characterList.Where(x => x.index == 0).FirstOrDefault();
        if (character != null)
        {
            character.OnCharacterSelected();
            return character.info;
        }
        return null;
    }

    private void ChangeColor(AvatarInfo info)
    {
        background.color = info.color;
        nameText.text = info.avatarName;
        evolutionText.color = info.textColor;
        nameBackground.sprite = Main.Instance.GetSprite(info.avatarName + "_Button");
        backButton.GetComponent<Image>().sprite = Main.Instance.GetSprite(info.avatarName + "_Back");
    }

    private void SelectCharacter(AvatarInfo info)
    {
        HomeController.Instance.SelectCharacter(info, gameObject.activeInHierarchy);
        UserData.SetAvatarName(info);
        Character.Instance.UpdateSelectedAvatar(info.avatarId);
        ChangeColor(info);

        SetEvolutionTree(info);
    }
}
