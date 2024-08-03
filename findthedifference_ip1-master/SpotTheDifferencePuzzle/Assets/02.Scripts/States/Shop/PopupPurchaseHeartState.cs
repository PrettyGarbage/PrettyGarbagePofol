using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupPurchaseHeartStateData : BaseStateData
{
    public Action<bool> callback;
    public PopupPurchaseHeartStateData() { }
    public PopupPurchaseHeartStateData(Action<bool> callback)
    {
        this.callback = callback;
    }
}

public class PopupPurchaseHeartState : PopupState
{

    [SerializeField]
    private TMP_Text _heartAmount;
    
    [SerializeField]
    private BuyButtonCtrl _buyButtonCtrl;

    [SerializeField]
    private Button _btnWatchAd;

    private ProductInfo _productInfo;

    public Action<bool> _callback;
    public bool _isPurchased;

    public static void Open(Action<bool> callback)
    {
        StateManager.Instance.OpenPopupState(typeof(PopupPurchaseHeartState), new PopupPurchaseHeartStateData(callback));        
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        _isPurchased = false;
        _callback =  GetData<PopupPurchaseHeartStateData>().callback;

        if (_productInfo == null)
        {
            List<ShopInfo> shopInfoList = DataManager.Instance.GetShopInfoList(ShopConstants.SHOPID_CURRENCY_HEART);
            if (shopInfoList == null || shopInfoList.Count <= 0) yield break;

            ShopInfo shopInfo = shopInfoList[0];
            _productInfo = DataManager.Instance.GetProductInfo(shopInfo.productId);
            _buyButtonCtrl.SetButton(_productInfo, shopInfo.id, OnBuyComplete);

            _btnWatchAd.onClick.AddListener(OnWatchAd);

            ProductItemInfo productItemInfo = _productInfo.productItemList[0];

            _heartAmount.text = productItemInfo.TotalCount.ToString();
        }

        _btnWatchAd.gameObject.SetActive(DataManager.Instance.IsEnableRewardAd(ADConstants.REWARD_READY_HEART_NOTENOUGH));
        
    }

    #region EVENTS
    private void OnBuyComplete(bool isBuyComplete)
    {
        if (isBuyComplete)
        {
            OnBack();
            _isPurchased = true;
        }        
    }

    public override void OnPostClosed()
    {
        if (_callback != null)
            _callback(_isPurchased);
    }

    private void OnWatchAd()
    {
        NetworkManager.Instance.ShowRewardVideoAd(ADConstants.REWARD_READY_HEART_NOTENOUGH, Reward => {

            _isPurchased = true;

        }, isShowSuccess => {

            if (isShowSuccess && _isPurchased)
            {
                OnBack();
            }
            
        });
    }

	#endregion EVENTS
}