using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainShopListCtrl : ShopListCtrl
{
    [Header("ShopMain")]
    [SerializeField]
    private ShopMainType _shopMainType;

    private ShopStateData _shopStateData;

    public override void SetProductList(ShopState shopState)
    {
        base.SetProductList(shopState);
        _shopStateData = new ShopStateData(_shopType, _shopMainType);
    }

    override protected void SetTabTitle(ShopInfo shopInfo)
    {
        if(_shopMainType != ShopMainType.EVENT)
        {
            //ProductInfo productInfo = DataManager.Instance.GetProductInfo(shopInfo.productId);
            //_titleText.SetText(ResourceDataManager.Instance.GetSpriteDataList(productInfo)[0].LocalizedTextKey);
        }
    }

    override public List<ShopInfo> GetProductList()
    {
        return DataManager.Instance.GetShopInfoList(_shopMainType);
    }

    override public void Show(BaseStateData baseStateData)
    {
        ShopStateData shopStateData = (ShopStateData)baseStateData;
        Show(shopStateData.shopMainType == _shopMainType);
    }

    override public void OnTab()
    {
        _shopState.OnTab(_shopStateData);
    }
}
