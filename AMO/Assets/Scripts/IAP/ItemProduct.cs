using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemProduct : MonoBehaviour
{
    public string productId;
    public Button purchaseButton;
    public TMP_Text coinText;
    public TMP_Text priceText;

    private void Start()
    {
        purchaseButton.onClick.AddListener(Purchase);
        InAppProduct iap = IAP.Instance.iapList.Where(x => x.productId == productId).FirstOrDefault();
        if (iap != null)
        {
            CultureInfo cultureInfo = new CultureInfo("ID-id");

            coinText.text = iap.quantity.ToString();
            priceText.text = iap.price.ToString("C3", cultureInfo);
        }
    }

    public void Purchase()
    {
        IAP.Instance.BuyProduct(productId);
    }
}
