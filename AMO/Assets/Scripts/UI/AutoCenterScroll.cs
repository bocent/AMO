using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoCenterScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private LayoutGroup layoutGroup;

    public float centeringSpeed = 1f;
    [Tooltip("item width required for first item position placement")]
    [SerializeField] private float childWidth;
    [Tooltip("item height required for first item position placement")]
    [SerializeField] private float childHeight;

    // Start is called before the first frame update
    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        layoutGroup = scrollRect.content.GetComponent<LayoutGroup>();
        Init();
    }

    private void Init()
    {
        //Place first index item to center
        if (scrollRect.horizontal)
        {
            layoutGroup.padding.left = Mathf.RoundToInt(((RectTransform)scrollRect.transform.parent).rect.width / 2 - childWidth / 2);
            layoutGroup.padding.right = Mathf.RoundToInt(((RectTransform)scrollRect.transform.parent).rect.width / 2 - childWidth / 2);
        }
        if (scrollRect.vertical)
        {
            layoutGroup.padding.top = Mathf.RoundToInt(((RectTransform)scrollRect.transform.parent).rect.height / 2 - childHeight / 2);
            layoutGroup.padding.bottom = Mathf.RoundToInt(((RectTransform)scrollRect.transform.parent).rect.height / 2 - childHeight / 2);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        CheckPosition();
    }

    private void CheckPosition()
    {
        Transform closestChild = null;
        float closestDistance = float.MaxValue;
        foreach (Transform child in scrollRect.content)
        {
            float distance = Vector2.Distance(child.position, scrollRect.transform.position);
            float absDistance = Mathf.Abs(distance);
            if (distance < closestDistance)
            {
                closestChild = child;
                closestDistance = distance;
            }
        }
        if (closestChild)
        {
            Recentering(closestChild, closestDistance);
        }
    }

    private void Recentering(Transform child, float distance)
    {
        if (scrollRect.horizontal)
        {
            if (child.position.x > scrollRect.transform.parent.position.x)
            {
                //move left
                scrollRect.velocity = new Vector2(-distance * centeringSpeed, 0);
            }
            else if (child.position.x < scrollRect.transform.parent.position.x)
            {
                //move right
                scrollRect.velocity = new Vector2(distance * centeringSpeed, 0);
            }
        }
        else if (scrollRect.vertical)
        {
            if (child.position.y > scrollRect.transform.parent.position.y)
            {
                //move down
                scrollRect.velocity = new Vector2(-distance * centeringSpeed, 0);
            }
            else if (child.position.y < scrollRect.transform.parent.position.y)
            {
                //move up
                scrollRect.velocity = new Vector2(distance * centeringSpeed, 0);
            }
        }
    }
}
