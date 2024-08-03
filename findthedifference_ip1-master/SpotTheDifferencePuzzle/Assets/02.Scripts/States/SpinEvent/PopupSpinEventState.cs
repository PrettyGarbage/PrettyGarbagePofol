using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupSpinEventState : PopupState
{
    [Header("PopupSpinEvent")]
    [SerializeField]
    private TMP_Text _spinTicketText;

    [SerializeField]
    private SpinBoard _spinBoard;

    [SerializeField]
    private GameObject _buttonSpinObject;
    [SerializeField]
    private GameObject _buttonSpinAdObject;

    private ShopInfo _shopInfo;
    private ProductInfo _productInfo;

    public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupSpinEventState));
    }

    public static void Close()
    {
        StateManager.Instance.GetRegisteredState(typeof(PopupSpinEventState)).OnBack();       
    }

    public override IEnumerator OnInitialize()
    {  
        return base.OnInitialize();
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);
        
        SetSpinTicketAmount();

        if(_shopInfo==null)
            _shopInfo = DataManager.Instance.GetShopInfo(ShopConstants.SHOPID_CURRENCY_EVENT, ShopConstants.PRODUCTID_JACKPOT);
        if(_productInfo==null)
        _productInfo = DataManager.Instance.GetProductInfo(ShopConstants.PRODUCTID_JACKPOT);
    }

    public override IEnumerator OnPostOpen()
    {
        return base.OnPostOpen();       
    }

    public void OnSpin()
    {
        if(DataManager.Instance.UserInfo.GetItemCount(ItemType.SPIN_TICKET) > 0)
        {
            StateManager.Instance.ActiveBlockScreen(true);
            ApiManager.Instance.GetLuckySpinResult(apiResult => {
                if (apiResult.isSuccess)
                {
                    _spinBoard.Spin(apiResult.result,()=> {
                        SetSpinTicketAmount();
                        StateManager.Instance.ActiveBlockScreen(false);
                    });                    
                }
                else
                {
                    MessageBoxState.Open(apiResult.errorInfo);
                    SetSpinTicketAmount();
                    StateManager.Instance.ActiveBlockScreen(false);
                }
            });
        }   
        else
        {
            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.ALERT_NOT_ENOUGH_ITEMCOUNT, ()=> {
                SetSpinTicketAmount();
            });
        }
    }

    bool _isRewardSuccess;
    public void OnSpinAD()
    {
        _isRewardSuccess = false;
        StateManager.Instance.ActiveBlockScreen(true);
        NetworkManager.Instance.ShowRewardVideoAd(ADConstants.REWARD_LUCKY_SPIN, Reward => {

            Debug.Log("REWARED REWARD_LUCKY_SPIN !");
            _isRewardSuccess = true;
            
        }, isShowSuccess => {

            StateManager.Instance.ActiveBlockScreen(false);
            if (isShowSuccess)
            {
                if (_isRewardSuccess)
                {
                    OnSpin();                    
                }
                else
                {
                    SetSpinTicketAmount();
                }
            }            
        });
    }

    private void SetSpinTicketAmount()
    {
        _buttonSpinObject.SetActive(false);
        _buttonSpinAdObject.SetActive(false);

        int spinTicketAmount = DataManager.Instance.UserInfo.GetItemCount(ItemType.SPIN_TICKET);
        _spinTicketText.text = SystemUtil.GetCommaText(spinTicketAmount);

        if (spinTicketAmount > 0)
        {
            _buttonSpinObject.SetActive(true);
        }
        else
        {
            if (DataManager.Instance.IsEnableRewardAd(ADConstants.REWARD_LUCKY_SPIN))
            {
                _buttonSpinAdObject.SetActive(true);
            }
        }
    }

    public void OnBuyDetail()
    {
        PopupEventShopDetailState.Open(_productInfo, _shopInfo, isBuyComplete => {
            if (isBuyComplete)
            {
                SetSpinTicketAmount();
            }
        });
    }


}