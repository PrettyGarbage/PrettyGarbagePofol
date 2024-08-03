using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupEventShopDetailStateData : BaseStateData
{
    public ProductInfo productInfo;
    public ShopInfo shopInfo;
    public Action<bool> onBuyCompleted;

    public PopupEventShopDetailStateData() { }
    public PopupEventShopDetailStateData(ProductInfo productInfo, ShopInfo shopInfo, Action<bool> onBuyCompleted)
    {
        this.productInfo = productInfo;
        this.shopInfo = shopInfo;
        this.onBuyCompleted = onBuyCompleted;
    }
}

public class PopupEventShopDetailState : PopupState
{
    const string TEXT_KEY_TITLE = "_TITLE";
    const string TEXT_KEY_DESC01 = "_POPUP_DESC_1";
    const string TEXT_KEY_DESC02 = "_POPUP_DESC_2";
    const string TEXT_KEY_PRICE = "_POPUP_PRICE";

    [SerializeField]
    private Image _packageImage;
    [SerializeField]
    private LocalizationUIText _titleText;
    [SerializeField]
    private LocalizationUIText _descText;
    [SerializeField]
    private LocalizationUIText _discountDescText;

    [SerializeField]
    protected BuyButtonCtrl _buyButtonCtrl;

    private Action<bool> _onBuyCompleted;
    private bool _isBuyComplete;

    public static void Open(ProductInfo productInfo, ShopInfo shopInfo, Action<bool> onBuyCompleted)
    {
        StateManager.Instance.OpenPopupState(typeof(PopupEventShopDetailState), new PopupEventShopDetailStateData(productInfo, shopInfo, onBuyCompleted));        
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        PopupEventShopDetailStateData detailStateData = GetData<PopupEventShopDetailStateData>();

        _isBuyComplete = false;
        _onBuyCompleted = detailStateData.onBuyCompleted;

        _buyButtonCtrl.SetButton(detailStateData.productInfo, detailStateData.shopInfo.id, OnBuyCompleted, true);

        string shopName = detailStateData.shopInfo.name;

        _packageImage.sprite = ResourceDataManager.Instance.GetEventShopSpriteData(shopName).Sprite;
        _titleText.SetText(shopName + TEXT_KEY_TITLE);
        _descText.SetText(shopName + TEXT_KEY_DESC01);
        _discountDescText.SetText(shopName + TEXT_KEY_PRICE);

    }

    private void OnBuyCompleted(bool isBuyComplete)
    {
        if (isBuyComplete)
        {
            OnBack();
            _isBuyComplete = true;
        }        
    }

    public override void OnPostClosed()
    {
        _onBuyCompleted(_isBuyComplete);
    }
}