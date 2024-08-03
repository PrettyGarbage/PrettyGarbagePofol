using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ProductCtrl : MonoBehaviour {

    [Header("Product")]
    [SerializeField]
    protected BuyButtonCtrl _buyButtonCtrl;

    protected ProductInfo _productInfo;
    protected string _shopId;

    virtual public void SetProductInfo(ShopInfo shopInfo, ProductInfo product, Sprite productIcon)
    {
        _shopId = shopInfo.id;
        _productInfo = product;

        if (_productInfo == null || _productInfo.productItemList[0] == null) return;

        if(_buyButtonCtrl)_buyButtonCtrl.SetButton(_productInfo, _shopId, OnBuyCompleted);
    }

    protected virtual void OnBuyCompleted(bool isBuyComplete)
    {
        
    }
}
