using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutfitIAP : ItemProduct
{
    protected override void Start()
    {
        purchaseButton.onClick.AddListener(Purchase);
    }

    public override void Init(ShopItem item)
    {
        priceText.text = item.price + " COINS";
        this.item = item;
        productImage.sprite = AccessoryController.Instance.GetAccessoryInfo(item.items_id).accessorySprite;
    }

    private void Purchase()
    {
        IAP.Instance.BuyProduct(item);
    }
}
