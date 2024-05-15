using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemProduct : MonoBehaviour
{
    public string productId;
    public Button purchaseButton;

    private void Start()
    {
        purchaseButton.onClick.AddListener(Purchase);
    }

    

    public void Purchase()
    {
        IAP.Instance.ProcessPurchase(null);
    }
}
