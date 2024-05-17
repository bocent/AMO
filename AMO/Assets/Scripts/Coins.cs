using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    public Image coinIcon;
    public Image coinPanel;
    public TMP_Text coinText;
    public Button purchaseButton;

    private void Start()
    {
        purchaseButton.onClick.AddListener(OpenInAppPurchase);
    }

    private void OpenInAppPurchase()
    {
        HomeController.Instance.ShowIAP(true);
    }

    public void SetCoin(string coin)
    {
        coinText.text = coin;
    }

    public void SetCoin(int coin)
    {
        SetCoin(coin.ToString());
    }
}
