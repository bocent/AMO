using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIAP : ItemProduct
{
    public GameObject ownedObj;

    protected override void Start()
    {
        purchaseButton.onClick.AddListener(Purchase);
    }

    public override void Init(AvatarInfo info)
    {
        avatarInfo = info;
        priceText.text = info.price + " COINS";
        productImage.sprite = info.evolutionList[0].avatarSprite;
        ownedObj.SetActive(info.isUnlocked);
        purchaseButton.interactable = !info.isUnlocked;
    }

    private void Purchase()
    {
        IAP.Instance.BuyProduct(avatarInfo);
    }
}
