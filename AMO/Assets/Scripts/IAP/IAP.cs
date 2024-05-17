using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

[Serializable]
public class InAppProduct
{
    public string productId;
    public int quantity;
    public int price;
}

public class IAP : MonoBehaviour, IDetailedStoreListener
{
    public List<InAppProduct> iapList;

    public GameObject container;
    public Button backButton;
    public TMP_Text coinText;
    public List<ItemProduct> productList;

    private const string environment = "production";
    private IStoreController storeController;
    public static IAP Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitializeGamingService(OnInitializeSuccess, OnInitializeError);
    }

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
        InitializePurchasing();
    }

    public void Show()
    {
        container.SetActive(true);
        UpdateCoin();
    }

    public void Show(bool value)
    {
        if (value)
            Show();
        else
            Hide();
    }

    public void Hide()
    {
        container?.SetActive(false);
    }

    private void InitializeGamingService(Action onSuccess, Action<string> onError)
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);

            UnityServices.InitializeAsync(options).ContinueWith(task => onSuccess());
        }
        catch (Exception exception)
        {
            onError(exception.Message);
        }
    }

    void OnInitializeSuccess()
    {
        var text = "Congratulations!\nUnity Gaming Services has been successfully initialized.";
        Debug.Log(text);
    }

    void OnInitializeError(string message)
    {
        var text = $"Unity Gaming Services failed to initialize with error: {message}.";
        Debug.LogError(text);
    }

    public void BuyProduct(string id)
    {
        storeController.InitiatePurchase(id);
    }


    private void InitializePurchasing()
    {
        Debug.LogWarning("InitializePurchasing...");
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < productList.Count; i++)
        {
            builder.AddProduct(productList[i].productId, ProductType.Consumable);
            Debug.LogWarning("product : " + productList[i].productId);
        }
        UnityPurchasing.Initialize(this, builder);
        Debug.LogWarning("initializing");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        storeController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;

        //Add the purchased product to the players inventory

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        InAppProduct iap = iapList.Where(x => x.productId == args.purchasedProduct.definition.id).FirstOrDefault();

        UserData.AddCoins(iap.quantity);
        UpdateCoin();
        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

    private void UpdateCoin()
    {
        coinText.text = UserData.Coins.ToString();
    }
}
