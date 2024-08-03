using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventProductCtrl : ProductCtrl
{

    const string TEXT_KEY_TITLE = "_TITLE";
    const string TEXT_KEY_DESC01 = "_BANNER_DESC_1";
    const string TEXT_KEY_DESC02 = "_BANNER_DESC_2";
    const string TEXT_KEY_PRICE = "_BANNER_PRICE";

    [Header("Event")]
    [SerializeField]
    private Image _packageImage;
    [SerializeField]
    private LocalizationUIText _titleText;
    [SerializeField]
    private LocalizationUIText _descText;
    [SerializeField]
    private LocalizationUIText _discountDescText;
    [SerializeField]
    private Text _priceText;

    private ShopInfo _shopInfo;

    override public void SetProductInfo(ShopInfo shopInfo, ProductInfo product, Sprite productIcon)
    {
        base.SetProductInfo(shopInfo, product, productIcon);

        _shopInfo = shopInfo;
        Debug.Log("event shop " + shopInfo.name + TEXT_KEY_TITLE);

        _packageImage.sprite = ResourceDataManager.Instance.GetEventShopSpriteData(shopInfo.name).Sprite;
        _titleText.SetText(shopInfo.name + TEXT_KEY_TITLE);
        _descText.SetText(shopInfo.name + TEXT_KEY_DESC02);
        _discountDescText.SetText(shopInfo.name + TEXT_KEY_PRICE);

        _priceText.text = IAPManager.Instance.GetStoreProductPrice(product);
    }

    public void OnOpenDetail()
    {
        PopupEventShopDetailState.Open(_productInfo, _shopInfo, isBuyComplete => {
            if (isBuyComplete)
            {
                OnBuyCompleted(isBuyComplete);                
            }
        });
    }
   

    protected override void OnBuyCompleted(bool isBuyComplete)
    {
        if(isBuyComplete && _productInfo.buyLimitCount == 1)
        {
            Destroy(gameObject);
        }
    }

}
