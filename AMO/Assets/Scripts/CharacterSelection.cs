using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject characterItemPrefab;
    public Transform characterItemParent;
    public SimpleScrollSnap simpleScrollSnap;
    public Button backButton;

    public GameObject container;

    private ToggleGroup toggleGroup;
    private List<CharacterItem> characterItemList = new List<CharacterItem>();

    private void Start()
    {
        simpleScrollSnap.OnPanelCentered.AddListener(OnItemSelected);
        backButton.onClick.AddListener(Hide);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitToRefresh());
    }

    private void Hide()
    {
        Show(false);
    }

    public void Show(bool value)
    {
        Debug.LogWarning("Show selection");
        container.SetActive(value);
        HomeController.Instance.ShowCharacterSelectionRoom(value);
        HomeController.Instance.ShowHome(!value);
        HomeController.Instance.ShowHUD(!value);

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

    private IEnumerator WaitToRefresh()
    {
        yield return null;
        for (int i = 0; i < simpleScrollSnap.Content.childCount; i++)
        {
            Debug.LogWarning("avatar id : " + Character.Instance.SelectedAvatarId + " " + simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId);
            if (simpleScrollSnap.Content.GetChild(i).GetComponent<CharacterItem>().Info.avatarId == Character.Instance.SelectedAvatarId)
            {
                simpleScrollSnap.GoToPanel(i);
                break;
            }
        }
    }

    public void Init()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        int unlockedCharacterCount = 0;
        List<AvatarInfo> infoList = Character.Instance.GetAvatarInfoList();
        for (int i = 0; i < infoList.Count; i++)
        {
            if (infoList[i].isUnlocked)
            {
                AvatarInfo info = infoList[i];
                GameObject item = Instantiate(characterItemPrefab, characterItemParent, false);
                CharacterItem characterItem = item.GetComponent<CharacterItem>();
                characterItem.Init(info, toggleGroup);
                characterItemList.Add(characterItem);
                unlockedCharacterCount += 1;
            }
        }
        if (unlockedCharacterCount < 3)
        {
            GameObject item = Instantiate(characterItemPrefab, characterItemParent, false);
            CharacterItem characterItem = item.GetComponent<CharacterItem>();
            characterItem.Init(characterItemList[0].Info, toggleGroup);

            item = Instantiate(characterItemPrefab, characterItemParent, false);
            characterItem = item.GetComponent<CharacterItem>();
            characterItem.Init(characterItemList[characterItemList.Count - 1].Info, toggleGroup);
        }
        simpleScrollSnap.Setup();
    }

    private void OnItemSelected(int to, int from)
    {
        simpleScrollSnap.Content.GetChild(to).GetComponent<Toggle>().isOn = true;
    }
}
