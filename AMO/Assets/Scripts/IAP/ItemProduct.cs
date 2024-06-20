using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemProduct : MonoBehaviour
{
    private string productId;
    public Image productImage;
    public Button purchaseButton;
    public TMP_Text coinText;
    public TMP_Text priceText;

    protected ShopItem item;
    protected ItemCoin coin;

    protected virtual void Start()
    {
        purchaseButton.onClick.AddListener(PurchaseCoin);
    }

    //public virtual void Init(string productId)
    //{
    //    this.productId = productId;
    //    InAppProduct iap = IAP.Instance.iapList.Where(x => x.productId == productId).FirstOrDefault();

    //    Debug.LogWarning("iap : " + iap);

    //    if (iap != null)
    //    {
    //        CultureInfo cultureInfo = new CultureInfo("ID-id");

    //        coinText.text = iap.quantity.ToString();
    //        priceText.text = iap.price.ToString("C3", cultureInfo);
    //        Debug.LogWarning("price : " + iap.price);
    //    }
    //}

    public virtual void Init(ItemCoin item)
    {
        coin = item;
        InAppProduct iap = IAP.Instance.iapList.Where(x => x.productId == item.topup_coin_id).FirstOrDefault();
        CultureInfo cultureInfo = new CultureInfo("ID-id");

        productImage.sprite = iap.sprite;
        coinText.text = item.qty.ToString() + " COIN";
        priceText.text = int.Parse(item.price).ToString("C2", cultureInfo);
    }

    public virtual void Init(ShopItem item)
    {
        
    }


    private void PurchaseCoin()
    {
        IAP.Instance.BuyProduct(coin);
    }

    private void Purchase()
    {
        IAP.Instance.BuyProduct(item);   
    }
}
