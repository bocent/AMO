using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

[Serializable]
public class ButtonInfo
{
    public string content;
    public Sprite buttonSprite;
    public Action onButtonClicked;

    public ButtonInfo() { }
    public ButtonInfo(string table, string key, Sprite sprite, Action onPressed)
    {
        //Localization.Translate(table, key, (result) => content = result);
        buttonSprite = sprite;
        onButtonClicked = onPressed;
    }
}
