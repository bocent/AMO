using NUnit.Framework;
using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

[Serializable]
public class InAppProduct
{
    public int productId;
    public int quantity;
    public int price;
    public Sprite sprite;
}

public class IAP : MonoBehaviour, IDetailedStoreListener
{
    public List<InAppProduct> iapList;

    public GameObject container;
    public Button backButton;
    public TMP_Text coinText;
    public List<ItemProduct> productList;

    public GameObject coinPrefab;
    public Transform coinParent;
    [Space]
    public GameObject accessoryPrefab;
    public Transform helmetParent;
    public Transform outfitParent;
    [Space]
    public GameObject characterPrefab;
    public Transform characterParent;
    [Space]
    public GameObject chargePrefab;
    public Transform chargeParent;

    public GameObject coinContainer;
    public GameObject outfitContainer;
    public GameObject helmetContainer;
    public GameObject chargeContainer;
    public GameObject characterContainer;

    public Toggle coinToggle;
    public Toggle helmetToggle;
    public Toggle outfitToggle;
    public Toggle energyToggle;
    public Toggle characterToggle;

    private const string environment = "production";
    private IStoreController storeController;

    private List<GameObject> coinList = new List<GameObject>();
    private List<GameObject> outfitList = new List<GameObject>();
    private List<GameObject> helmetList = new List<GameObject>();
    private List<GameObject> chargeList = new List<GameObject>();
    private List<GameObject> characterList = new List<GameObject>();

    public static IAP Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //InitializeGamingService(OnInitializeSuccess, OnInitializeError);
    }

    public void ShowCoinPanel(bool toggle)
    {
        coinContainer.SetActive(toggle);
    }

    public void ShowOutfitPanel(bool toggle)
    {
        outfitContainer.SetActive(toggle);
    }

    public void ShowHelmetPanel(bool toggle)
    {
        helmetContainer.SetActive(toggle);
    }

    public void ShowEnergyPanel(bool toggle)
    {
        chargeContainer.SetActive(toggle);
    }

    public void ShowCharacterPanel(bool toggle)
    {
        characterContainer.SetActive(toggle);
    }

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
        //InitializePurchasing();
    }

    public void Show()
    {
        coinToggle.isOn = true;
        StartCoroutine(RequestCoinList(() => {
            StartCoroutine(RequestItemList(() => {
                container.SetActive(true);
                UpdateCoin();
            }, null));
        }, null));
       
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

        InitializePurchasing();
    }

    void OnInitializeError(string message)
    {
        var text = $"Unity Gaming Services failed to initialize with error: {message}.";
        Debug.LogError(text);
    }

    public void BuyProduct(ItemCoin item)
    {
        //storeController.InitiatePurchase(id);
        PopupManager.Instance.ShowPopupMessage("topup", "Konfirmasi", "Apakah kamu yakin untuk membeli <color=yellow>" + item.name + "</color>?",
            new ButtonInfo { 
                content = "Ya",
                onButtonClicked = () => StartCoroutine(RequestBuyCoin(item.topup_coin_id, null, null))
            },
            new ButtonInfo { content = "Batal" });
    }

    public void BuyProduct(ShopItem item)
    {
        PopupManager.Instance.ShowPopupMessage("topup", "Konfirmasi", "Apakah kamu yakin untuk membeli <color=yellow>" + item.item_name + "</color>?",
            new ButtonInfo
            {
                content = "Ya",
                onButtonClicked = () => StartCoroutine(RequestBuyItem(item.item_sell_id,
                (coins) =>
                {
                    StartCoroutine(Character.Instance.RequestUserData(null, null));
                },
                (error) =>
                { 

                }))
            },
            new ButtonInfo { content = "Batal" });
    }

    private void InitializePurchasing()
    {
        //Debug.LogWarning("InitializePurchasing...");
        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //for (int i = 0; i < iapList.Count; i++)
        //{
        //    builder.AddProduct(iapList[i].productId, ProductType.Consumable);
        //    Debug.LogWarning("product : " + iapList[i].productId);
        //    GameObject item = Instantiate(coinPrefab, coinParent, false);
        //    item.GetComponent<ItemProduct>().Init(iapList[i].productId);
        //    item.SetActive(true);
        //}
        //UnityPurchasing.Initialize(this, builder);
        //Debug.LogWarning("initializing");
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

        //InAppProduct iap = iapList.Where(x => x.productId == args.purchasedProduct.definition.id).FirstOrDefault();

        //UserData.AddCoins(iap.quantity, HomeController.Instance.RefreshCoins);
        //UpdateCoin();
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




    public IEnumerator RequestBuyCoin(int coinId, Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"topup_coin_id\" : \"" + coinId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "buy_coins", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    BuyCoinResponse response = JsonUtility.FromJson<BuyCoinResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke();
                        Application.OpenURL(response.payment_url);
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                  new ButtonInfo
                  {
                      content = "Ulangi",
                      onButtonClicked = () => StartCoroutine(RequestBuyCoin(coinId, onComplete, onFailed))
                  },
                  new ButtonInfo
                  {
                      content = "Batal"
                  });
            }
        }
    }

    private IEnumerator RequestItemList(Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        //form.AddField("data", "{\"items_id\" : \"" + string.Empty + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_item_sell", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    ShopResponse response = JsonUtility.FromJson<ShopResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        LoadHelmetList(response.item_sell.accessories.helmet);
                        LoadOutfitList(response.item_sell.accessories.outfit);
                        LoadChargeList(response.item_sell.charge);
                        LoadCharacterList(response.item_sell.karakter);
                        
                        onComplete?.Invoke();
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestItemList(onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    private IEnumerator RequestCoinList(Action onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        //form.AddField("data", "{\"items_id\" : \"" + string.Empty + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_topup_coin_item", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            //try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    CoinListResponse response = JsonUtility.FromJson<CoinListResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        LoadCoinList(response.items_coin);
                        onComplete?.Invoke();
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            //catch (Exception e)
            //{
            //    onFailed?.Invoke(e.Message);
            //    Debug.LogError("err : " + e.Message);
            //    PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
            //        new ButtonInfo
            //        {
            //            content = "Ulangi",
            //            onButtonClicked = () => StartCoroutine(RequestCoinList(onComplete, onFailed))
            //        },
            //        new ButtonInfo
            //        {
            //            content = "Batal"
            //        });
            //}
        }
    }

    private IEnumerator RequestBuyItem(int productId, Action<int> onComplete, Action<string> onFailed)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"item_sell_id\" : \"" + productId + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "get_item_sell", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    BuyItemResponse response = JsonUtility.FromJson<BuyItemResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        onComplete?.Invoke(response.sisa_coin);
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestBuyItem(productId, onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });
            }
        }
    }

    private void LoadCoinList(ItemCoin[] list)
    {
        foreach (GameObject obj in coinList)
        {
            Destroy(obj);
        }
        coinList.Clear();
        if (list != null)
        {
            foreach (ItemCoin item in list)
            {
                GameObject obj = Instantiate(coinPrefab, coinParent, false);
                obj.GetComponent<ItemProduct>().Init(item);
                obj.SetActive(true);
                coinList.Add(obj);
            }
        }
    }

    private void LoadOutfitList(ShopItem[] list)
    {
        foreach (GameObject obj in outfitList)
        {
            Destroy(obj);
        }
        outfitList.Clear();
        if (list != null)
        {
            foreach (ShopItem item in list)
            {
                GameObject obj = Instantiate(accessoryPrefab, outfitParent, false);
                obj.GetComponent<OutfitIAP>().Init(item);
                obj.SetActive(true);
                outfitList.Add(obj);
            }
        }
    }

    private void LoadHelmetList(ShopItem[] list)
    {
        foreach (GameObject obj in helmetList)
        {
            Destroy(obj);
        }
        helmetList.Clear();
        if (list != null)
        {
            foreach (ShopItem item in list)
            {
                GameObject obj = Instantiate(accessoryPrefab, helmetParent, false);
                obj.GetComponent<OutfitIAP>().Init(item);
                obj.SetActive(true);
                helmetList.Add(obj);
            }
        }
    }
    private void LoadChargeList(ShopItem[] list)
    {
        foreach (GameObject obj in chargeList)
        {
            Destroy(obj);
        }
        chargeList.Clear();
        if (list != null)
        {
            foreach (ShopItem item in list)
            {
                GameObject obj = Instantiate(chargePrefab, chargeParent, false);
                obj.GetComponent<ChargeIAP>().Init(item);
                obj.SetActive(true);
                chargeList.Add(obj);
            }
        }
    }

    private void LoadCharacterList(ShopItem[] list)
    {
        foreach (GameObject obj in characterList)
        {
            Destroy(obj);
        }
        characterList.Clear();
        if (list != null)
        {
            foreach (ShopItem item in list)
            {
                GameObject obj = Instantiate(characterPrefab, characterParent, false);
                obj.GetComponent<CharacterIAP>().Init(item);
                obj.SetActive(true);
                characterList.Add(obj);
            }
        }
    } 
}
