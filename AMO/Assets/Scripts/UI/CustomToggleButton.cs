using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleButton : MonoBehaviour
{
    public enum SelectionType
    {
        Color,
        Sprite,
        Both
    }
    public SelectionType selectionType;

    public Color selectedColor;
    public Sprite selectedSprite;

    public Image targetGraphic;
    public ToggleGroup toggleGroup;

    private Color defaultColor;
    private Sprite defaultSprite;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
        defaultColor = targetGraphic.color;
        defaultSprite = targetGraphic.sprite;
    }

    private void OnButtonClicked()
    {
        switch (selectionType)
        {
            case SelectionType.Color:
                SwitchColor();
                break;
            case SelectionType.Sprite:
                SwitchSprite();
                break;
            case SelectionType.Both:
                SwitchColor();
                SwitchSprite();
                break;
        }
        if (toggleGroup.allowSwitchOff)
        {
            foreach (CustomToggleButton customToggle in toggleGroup.GetComponentsInChildren<CustomToggleButton>())
            {
                if (customToggle != this)
                {
                    switch (selectionType)
                    {
                        case SelectionType.Color:
                            customToggle.DeselectColor();
                            break;
                        case SelectionType.Sprite:
                            customToggle.DeselectSprite();
                            break;
                        case SelectionType.Both:
                            customToggle.DeselectColor();
                            customToggle.DeselectSprite();
                            break;
                    }
                }
            }
        }
    }

    private void SwitchColor()
    {
        targetGraphic.color = selectedColor;
    }

    private void SwitchSprite()
    {
        targetGraphic.sprite = selectedSprite;
    }

    public void DeselectColor()
    {
        targetGraphic.color = defaultColor;
    }

    public void DeselectSprite()
    {
        targetGraphic.sprite = defaultSprite;
    }
}
