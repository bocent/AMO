using DG.Tweening;
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
        PlayCoinAnimation();
        
    }

    private void PlayCoinAnimation()
    {
        coinIcon.transform.DOPunchScale(new Vector3(2, 2, 2), 1f, 5, 0.1f).SetDelay(3f).Play().OnComplete(PlayCoinAnimation);
    }

    private void OpenInAppPurchase()
    {
        return;
        HomeController.Instance.ShowIAP(true);
    }

    public void SetCoin(string coin)
    {
        coinText.text = coin + " COINS";
    }

    public void SetCoin(int coin)
    {
        SetCoin(coin.ToString());
    }
}
