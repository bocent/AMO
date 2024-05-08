using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItemUI : MonoBehaviour
{
    public Image image;
    public TMP_Text nameText;

    public void Set(Sprite sprite, string name)
    {
        Set(sprite);
        nameText.text = name;
    }
    public void Set(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnItemSelected()
    {
        
    }
}
