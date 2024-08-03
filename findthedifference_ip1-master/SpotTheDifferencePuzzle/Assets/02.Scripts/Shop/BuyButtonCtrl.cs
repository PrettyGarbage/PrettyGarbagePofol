using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class BuyResultInfo
{
    public bool isSuccess;
    public ApiErrorCode errorCode;

    public BuyResultInfo Set(bool isSuccess, ApiErrorCode errorCode)
    {
        this.isSuccess = isSuccess;
        this.errorCode = errorCode;

        return this;
    }
}

[RequireComponent(typeof(Button))]
public class BuyButtonCtrl : MonoBehaviour {

    [SerializeField]
    protected Image _itemImage;

    [SerializeField]
    protected TMP_Text _priceText;
    [SerializeField]
    protected Text _priceTextForCurrency;

    protected Button _button;
    protected ProductInfo _productInfo;
    protected string _shopId;
    protected Action<bool> _onBuyComplete;

    private BuyResultInfo _buyResultInfo;

    private bool _isBuyStatus = false;

    public bool IsBuyStatus { get { return _isBuyStatus; } }

    virtual public void SetButton(ProductInfo productInfo, string shopId, Action<bool> onBuyComplete = null, bool isUpdate = false)
    {
        if (_productInfo == null)
        {
            UpdateInfo(productInfo, shopId, onBuyComplete);

             _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickBuy);

            _buyResultInfo = new BuyResultInfo();
        }

        if (isUpdate)
        {
            UpdateInfo(productInfo, shopId, onBuyComplete);
        }
        
    }

    virtual protected void UpdateInfo(ProductInfo productInfo, string shopId, Action<bool> onBuyComplete = null)
    {
        _productInfo = productInfo;
        _shopId = shopId;
        _onBuyComplete = onBuyComplete;
        if (productInfo.paymentType == PaymentType.CASH)
        {
            SetPrice(IAPManager.Instance.GetStoreProductPrice(productInfo));
            if (_itemImage) _itemImage.gameObject.SetActive(false);
        }
        else
        {
            SetPrice(SystemUtil.GetCommaText(productInfo.price));
            PaymentSpriteData paymentSpriteData = ResourceDataManager.Instance.GetPaymentResourceData(productInfo.paymentType);
            if (_itemImage) _itemImage.sprite = paymentSpriteData.Sprite;
        }
    }

    virtual public void SetPrice(String pricestring)
    {
        if (_priceText)
            _priceText.gameObject.SetActive(false);
        if (_priceTextForCurrency)
            _priceTextForCurrency.gameObject.SetActive(false);

        if (_productInfo.paymentType == PaymentType.CASH)
        {
            if (_priceTextForCurrency)
            {
                _priceTextForCurrency.text = pricestring;
                _priceTextForCurrency.gameObject.SetActive(true);
            }

        }
        else
        {
            if (_priceText)
            {
                _priceText.text = pricestring;
                _priceText.gameObject.SetActive(true);
            }
        }
    }

    virtual public void OnClickBuy()
    {
        //TODO : 공통 DIM
        if (_isBuyStatus) return;

        _isBuyStatus = true;
        StateManager.Instance.ActiveBlockScreen(true);
        if (_productInfo.paymentType == PaymentType.CASH)
        {
            IAPManager.Instance.Buy(_shopId, _productInfo, result =>
            {
                ShowBuyResult(result);
            });
        }
        else
        {
            ApiErrorCode apiErrorCode = _productInfo.GetBuyEnableCode();
            if (apiErrorCode != ApiErrorCode.NONE)
            {
                ShowBuyResult(_buyResultInfo.Set(false, apiErrorCode));
                return;
            }
            
            ApiManager.Instance.BuyByGold(_shopId, _productInfo.id, apiResult =>
            {
                if (apiResult.isSuccess)
                {
                    ShowBuyResult(_buyResultInfo.Set(true, ApiErrorCode.NONE));
                }
                else
                {
                    ShowBuyResult(_buyResultInfo.Set(false, apiResult.errorInfo.errorCode));
                }
            });
        }
    }

    private void ShowBuyResult(BuyResultInfo buyResultInfo)
    {
        if (buyResultInfo.isSuccess)
        {
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_shop_buy);
            if(_productInfo.paymentType == PaymentType.CASH)
            {
                MessageBoxState.Open(LocalizationTextKey.STORE_PURCHASEPOPUP_SUCESS_TITLE, LocalizationTextKey.STORE_PURCHASEPOPUP_SUCESS_DESC, () => {
                    OnBuyComplete(true);
                });
            }
            else
            {
                SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.BUY_ACTION_TEXT, (isEndAfterPlay) => {
                    if (isEndAfterPlay)
                    {
                        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CALL_NAME);
                    }
                });

                PopupInstantState.Open(PopupInstantType.PURCHSE_SUCCESS, string.Empty, ()=> {
                    OnBuyComplete(true);
                });
            }            
        }
        else
        {
            if(buyResultInfo.errorCode == ApiErrorCode.NOT_ENOUGH_GOLD)
            {
                MessageBoxState.Open(new GameErrorInfo(buyResultInfo.errorCode), OnNotEnoughGold);
            }
            else
            {
                MessageBoxState.Open(new GameErrorInfo(buyResultInfo.errorCode));                
            }
            OnBuyComplete(false);

        }
        StateManager.Instance.ActiveBlockScreen(false);
    }

    

    private void OnNotEnoughGold()
    {
        Debug.Log("OnNotEnoughGold");
        if (StateManager.Instance.IsStacked((typeof(ShopState)))){
            ShopState shopState = (ShopState)StateManager.Instance.GetRegisteredState(typeof(ShopState));
            shopState.OnTab(new ShopStateData(ShopType.MAIN, ShopMainType.GOLD));
        }
        else
        {
            ShopState.Open(ShopMainType.GOLD);
        }
    }

    protected void OnBuyComplete(bool isBuyCompleted)
    {
        if (_onBuyComplete != null)
            _onBuyComplete(isBuyCompleted);

        _isBuyStatus = false;
    }

}
