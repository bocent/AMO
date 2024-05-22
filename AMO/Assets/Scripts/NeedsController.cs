using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeedsController : MonoBehaviour
{
    public List<BubbleNeeds> bubbleNeedsList;

    public static NeedsController Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        foreach (BubbleNeeds bubbleNeeds in bubbleNeedsList)
        {
            bubbleNeeds.gameObject.SetActive(false);
        }

        List<int> requirementList = UserData.GetRequirementList();
        foreach (int type in requirementList)
        {
            BubbleNeeds bubbleNeeds = null;
            switch (type)
            {
                case (int)Main.RequirementType.NEED_CLEAN_UP:
                    bubbleNeeds = GetItem();
                    bubbleNeeds.Init(Main.RequirementType.NEED_CLEAN_UP, Main.Instance.GetSprite("Soap"));
                    break;
                case (int)Main.RequirementType.NEED_FOOD:
                    bubbleNeeds = GetItem();
                    bubbleNeeds.Init(Main.RequirementType.NEED_FOOD, Main.Instance.GetSprite("Battery"));
                    break;
                case (int)Main.RequirementType.NEED_FIX_UP:
                    bubbleNeeds = GetItem();
                    bubbleNeeds.Init(Main.RequirementType.NEED_FIX_UP, Main.Instance.GetSprite("Wrench"));
                    break;
            }
        }
    }

    private BubbleNeeds GetItem()
    {
        for (int i = 0; i < bubbleNeedsList.Count; i++)
        {
            if (!bubbleNeedsList[i].gameObject.activeSelf)
            {
                bubbleNeedsList[i].Reset();
                bubbleNeedsList[i].gameObject.SetActive(true);
                return bubbleNeedsList[i];
            }
        }
        return null;
    }

    public void Pop(Main.RequirementType type)
    {
        BubbleNeeds bubbleNeeds = bubbleNeedsList.Where(x => x.type == type).FirstOrDefault();
        Debug.LogError("bubbleNeeds : " + bubbleNeeds);
        if(bubbleNeeds) bubbleNeeds.Pop();
    }
}
