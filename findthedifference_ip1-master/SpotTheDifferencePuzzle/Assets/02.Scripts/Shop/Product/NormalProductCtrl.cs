using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NormalProductCtrl : ProductCtrl
{
    [Header("Product")]
    [SerializeField]
    protected Image _productIconImage;
    [SerializeField]
    protected TMP_Text _amountText;
    [SerializeField]
    protected TMP_Text _amountBonusText;
    
    override public void SetProductInfo(ShopInfo shopInfo, ProductInfo product, Sprite productIcon)
    {
        base.SetProductInfo(shopInfo, product, productIcon);

        Debug.Log("NormalProductCtrl : " + product.ToString());

        ProductItemInfo productItemInfo = _productInfo.productItemList[0];
        int bonusCount = productItemInfo.bonusCount;

        if (product.paymentType == PaymentType.CASH && _productInfo.productItemList.Count == 2)
        {
            ProductItemInfo sProductIconInfo = _productInfo.productItemList[1];
            if (productItemInfo.currencyType == CurrencyType.GOLD && sProductIconInfo.currencyType == CurrencyType.FREE_GOLD)
            {
                bonusCount += sProductIconInfo.bonusCount;
            }
        }

        _amountText.text = SystemUtil.GetCommaText(productItemInfo.count);
        if (bonusCount > 0)
        {
            _amountBonusText.text = string.Format("({0})", CommonConstants.PLUS_STRING + SystemUtil.GetCommaText(bonusCount) );
        }
        else {
            _amountBonusText.text = string.Empty;
        }
        if(productIcon)_productIconImage.sprite = productIcon;

    }

}
