using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeIAP : ItemProduct
{
    protected override void Start()
    {
        purchaseButton.onClick.AddListener(Purchase);
    }

    public override void Init(ShopItem item)
    {
        priceText.text = item.price + " COINS";
        productImage.sprite = Inventory.Instance.GetItemInfo(item.items_id)?.sprite;
    }

    private void Purchase()
    {
        IAP.Instance.BuyProduct(item);
    }
}
