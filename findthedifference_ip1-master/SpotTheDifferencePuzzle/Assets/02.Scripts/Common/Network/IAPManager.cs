using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using ProductType = UnityEngine.Purchasing.ProductType;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IAPManager _instance;
    public static IAPManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<IAPManager>("IAPManager");                                
            }
            return _instance;
        }
    }

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public const string TEST_ANDROID_PRODUCT_ID = "android.test.purchased";

    private Action _initializeCallback;
    private Action<BuyResultInfo> _buyCallback;
    private string _storeProductId;
    private string _productId;
    private string _shopId;
    
    private BuyResultInfo _buyResultInfo;

    #region Initialize
    public void InitializePurchasing(List<ProductInfo> productInfos, Action callback)
    {
        if (_initializeCallback != null) return;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        _initializeCallback = callback;

        _buyResultInfo = new BuyResultInfo();

        List<ProductDefinition> productDefinitions = new List<ProductDefinition>();
        for (int i = 0; i < productInfos.Count; i++)
        {
            string storeProductId = productInfos[i].GetStoreProductId();
            if (storeProductId.Length>0)
            {
                Debug.Log("IAP InitializePurchasing id : " + productInfos[i] .id+ "/ productId : " + storeProductId);
                productDefinitions.Add(new ProductDefinition(storeProductId, UnityEngine.Purchasing.ProductType.Consumable));
            }
        }
        
        builder.AddProducts(productDefinitions);
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {  
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("IAP OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        _initializeCallback();
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("IAP OnInitializeFailed InitializationFailureReason:" + error);
        _initializeCallback();
    }
    #endregion

    #region Buy Product
    public void Buy(String shopId, ProductInfo productInfo, Action<BuyResultInfo> buyCallback)
    {
        _storeProductId = productInfo.GetStoreProductId();
        _productId = productInfo.id;
        _shopId = shopId;
        _buyCallback = buyCallback;

        if (DataManager.Instance.UserInfo.Locale == SystemLanguage.Japanese.ToString())
        {
            //생년월일 입력창.
            if (DataManager.Instance.UserInfo.BirthDay == string.Empty)
            {  
                StateManager.Instance.ActiveBlockScreen(false);
                PopupBirthdayInputState.Open((isSuccess) => {
                    if (isSuccess)
                    {
                        Buy(shopId, productInfo, buyCallback);
                    }
                    else
                    {
                        PurchaseCallback(_buyResultInfo.Set(false, ApiErrorCode.PURCHASE_FAIL));
                    }
                });
            }
            else
            {
                //만 20세 이하.
                if (SystemUtil.GetAge(DataManager.Instance.UserInfo.BirthDay) < 20)
                {
                    ApiManager.Instance.ValifyJpBuyAmountLimit(apiResult => {

                        if (apiResult.isSuccess)
                        {                            
                            BuyProductID(_storeProductId);
                        }
                        else
                        {
                            PurchaseCallback(_buyResultInfo.Set(false, apiResult.errorInfo.errorCode));
                        }

                    });
                }
                else
                {
                    BuyProductID(_storeProductId);
                }
            }
        }
        else
        {
            BuyProductID(_storeProductId);
        }

    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("IAP Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product, DataManager.Instance.UserInfo.Id.ToString());
            }
            else
            {
                Debug.Log("IAP BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                PurchaseCallback(_buyResultInfo.Set(false, ApiErrorCode.BAD_REQUEST));
            }
        }
        else
        {
            Debug.Log("IAP BuyProductID FAIL. Not initialized.");
            PurchaseCallback(_buyResultInfo.Set(false, ApiErrorCode.BAD_REQUEST));
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Product storeProduct = args.purchasedProduct;
        ProductInfo productInfo = DataManager.Instance.GetProductInfoByStoreId(storeProduct.definition.id);

        Debug.Log("IAP PurchaseProcessingResult  Id : " + storeProduct.definition.id + ", productInfo.id : " + productInfo.id+ ", receipt : " + args.purchasedProduct.receipt);
        ApiManager.Instance.BuyByCash(string.Empty, productInfo.id , storeProduct.receipt, apiResult => {

            if (apiResult.isSuccess)
            {
                m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                PurchaseCallback(_buyResultInfo.Set(true, ApiErrorCode.NONE));
            }
            else
            {
                Debug.Log("IAP Error ProcessPurchase : " + apiResult.errorInfo.GetErrorMessage());
                PurchaseCallback(_buyResultInfo.Set(false, apiResult.errorInfo.errorCode));
            }

        });

        return PurchaseProcessingResult.Pending;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        PurchaseCallback(_buyResultInfo.Set(false, ApiErrorCode.BAD_REQUEST));
        Debug.Log(string.Format("IAP OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
    #endregion


    #region Restore
    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("IAP RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("IAP RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("IAP RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("IAP RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    } 

    private void PurchaseCallback(BuyResultInfo buyResultInfo)
    {
        if (_buyCallback != null)
        {
            _buyCallback(buyResultInfo);
            _buyCallback = null;
        }
    }

    public String GetStoreProductPrice(ProductInfo productInfo)
    {
        if (m_StoreController != null)
        {
            Product product = m_StoreController.products.WithID(productInfo.GetStoreProductId());
            Debug.Log("IAP GetStoreProductPrice : " + product.metadata.localizedPriceString);
            return product.metadata.localizedPriceString;
        }
        else
        {
            return string.Format("$ {0}", SystemUtil.GetCommaText(productInfo.price));
        }
        
    }
    #endregion
}