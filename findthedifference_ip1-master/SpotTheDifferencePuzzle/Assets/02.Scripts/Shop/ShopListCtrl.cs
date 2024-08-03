using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public struct ProductIconInfo
{
    public string productId;
    public Sprite Sprite;
}

public class ShopListCtrl : MonoBehaviour, IShopList
{
    [Header("type")]
    [SerializeField]
    protected ShopType _shopType;

    [Header("TAP")]
    [SerializeField]
    protected Sprite _tabOnSprite;
    [SerializeField]
    protected Sprite _tabOffSprite;
    [SerializeField]
    protected Image _tapImage;

    [Header("List")]
    [SerializeField]
    protected Transform _listTransform;

    [SerializeField]
    protected ProductCtrl _pfProduct;

    [SerializeField]
    protected List<ProductIconInfo> _productIconInfo;

    [Header("AD")]
    [SerializeField]
    protected AdProductCtrl _adProductCtrl;

    protected ShopState _shopState;

    virtual public void SetProductList(ShopState shopState)
    {
        _shopState = shopState;
        List<ShopInfo> shopInfos = GetProductList();
        for (int i = 0; i < shopInfos.Count; i++)
        {
            ShopInfo shopInfo = shopInfos[i];
            ProductInfo productInfo = DataManager.Instance.GetProductInfo(shopInfo.productId);
            Sprite productSprite = _productIconInfo.Find(p => p.productId.Equals(shopInfo.productId)).Sprite;

            if (i == 0)
            {
                if (_adProductCtrl)
                {
                    _adProductCtrl.SetProductInfo(productSprite);
                }

                SetTabTitle(shopInfo);
            }

            if (!DataManager.Instance.IsLimitProduct(shopInfo.productId))
            {
                GameObject go = ObjectUtil.InstantiateAtTarget(_pfProduct.gameObject, _listTransform) as GameObject;
                ProductCtrl productCtrl = go.GetComponent<ProductCtrl>();
                productCtrl.SetProductInfo(shopInfo, productInfo, productSprite);
            }
        }

        _tapImage.GetComponent<Button>().onClick.AddListener(OnTab);
    }

    virtual protected void SetTabTitle(ShopInfo shopInfo)
    {
        
    }

    protected void Show(bool isShow)
    {
        _tapImage.sprite = isShow ? _tabOnSprite : _tabOffSprite;
        _listTransform.gameObject.SetActive(isShow);
    }

    virtual public List<ShopInfo> GetProductList()
    {
        throw new System.NotImplementedException();
    }

    virtual public void Show(BaseStateData baseStateData)
    {
        throw new System.NotImplementedException();
    }

    virtual public void OnTab()
    {
        throw new System.NotImplementedException();
    }
}
