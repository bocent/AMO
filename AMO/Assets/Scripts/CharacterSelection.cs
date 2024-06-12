using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject characterItemPrefab;
    public Transform characterItemParent;
    public Button backButton;

    public GameObject container;
    public Image background;
    public TMP_Text nameText;

    private List<CharacterSelectionAnimation> characterItemList = new List<CharacterSelectionAnimation>();

    private bool isShown = false;
    private float minSwipeLength = 5f;
    private float swipeDistance;
    private Vector2 touchPos;

    public List<Transform> characterPlaceholderList;

    private void Start()
    {
        //simpleScrollSnap.OnPanelCentered.AddListener(OnItemSelected);
        backButton.onClick.AddListener(Hide);
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

    public void Init()
    {
        int unlockedCharacterCount = 0;
        List<AvatarInfo> infoList = Character.Instance.GetAvatarInfoList();
        for (int i = 0; i < infoList.Count; i++)
        {
            if (infoList[i].isUnlocked)
            {
                AvatarInfo info = infoList[i];
                GameObject item = Instantiate(info.characterSelectionPrefab, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();
                unlockedCharacterCount += 1;
                item.transform.position = characterPlaceholderList[i].position;
                characterItemList.Add(characterItem);
            }
        }
        if (unlockedCharacterCount == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject item = Instantiate(characterItemList[0].gameObject, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();

                item = Instantiate(characterItemPrefab, characterItemParent, false);
                characterItem = item.GetComponent<CharacterSelectionAnimation>();
                characterItemList.Add(characterItem);
            }
        }
        else if (unlockedCharacterCount == 2)
        {
            for (int i = 1; i < 5; i++)
            {
                if (i % 2 == 0)
                {
                    GameObject item = Instantiate(characterItemList[0].gameObject, characterItemParent, false);
                    CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();

                    item = Instantiate(characterItemPrefab, characterItemParent, false);
                    characterItem = item.GetComponent<CharacterSelectionAnimation>();
                    characterItemList.Add(characterItem);
                }
                else
                {
                    GameObject item = Instantiate(characterItemList[1].gameObject, characterItemParent, false);
                    CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();

                    item = Instantiate(characterItemPrefab, characterItemParent, false);
                    characterItem = item.GetComponent<CharacterSelectionAnimation>();
                    characterItemList.Add(characterItem);
                }
            }
        }
        else
        {
            for (int i = 0; i < characterItemList.Count; i++)
            {
                GameObject item = Instantiate(characterItemList[i].gameObject, characterItemParent, false);
                CharacterSelectionAnimation characterItem = item.GetComponent<CharacterSelectionAnimation>();

                item = Instantiate(characterItemPrefab, characterItemParent, false);
                characterItem = item.GetComponent<CharacterSelectionAnimation>();
                characterItemList.Add(characterItem);
            }
        }
    }

    private void OnItemSelected(int to, int from)
    {
        //simpleScrollSnap.Content.GetChild(to).GetComponent<Toggle>().isOn = true;
    }

    private void Update()
    {
        if (isShown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                swipeDistance = 0f;
                touchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 releasePos = Input.mousePosition;

                swipeDistance = Vector2.Distance(touchPos, releasePos);

                if (releasePos.x > touchPos.x + minSwipeLength)
                {
                    NextCharacter();
                }
                else if (releasePos.x < touchPos.x + minSwipeLength)
                {
                    PrevCharacter();
                }
            }
        }
    }

    private void NextCharacter()
    {
        
    }

    private void PrevCharacter()
    {
        
    }

    private void ChangeColor(AvatarInfo info)
    {
        background.color = info.color;
        nameText.color = info.color;
    }
}
