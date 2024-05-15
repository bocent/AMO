using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAP : MonoBehaviour, IDetailedStoreListener
{
    public GameObject container;
    public Button backButton;
    public List<ItemProduct> productList;

    private IStoreController storeController;
    public static IAP Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        backButton.onClick.AddListener(Hide);
    }

    private void Show()
    {
        container.SetActive(true);
    }

    private void Hide()
    {
        container?.SetActive(false);
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < productList.Count; i++)
        {
            //Add products that will be purchasable and indicate its type.
            builder.AddProduct(productList[i].productId, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");
        UserData.AddCoins((int)product.definition.payout.quantity);
        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
    }
}
