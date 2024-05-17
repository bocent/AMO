using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeItem : MonoBehaviour
{
    public ItemInfo Info { get; private set; }
    public Image image;
    public TMP_Text quantityText;
    public Button useButton;

    private Inventory inventory;

    private void Start()
    {
        useButton.onClick.AddListener(UseItem);
    }

    public void Init(Inventory inventory, ItemInfo info)
    {
        Info = info; 
        image.sprite = Info.sprite;
        quantityText.text = "x" + Info.itemCount;
        this.inventory = inventory;
        gameObject.SetActive(info.itemCount > 0);
    }

    public void UseItem()
    {
        inventory.UseItem(Info);
        quantityText.text = "x" + Info.itemCount;
        if (Info.itemCount <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
