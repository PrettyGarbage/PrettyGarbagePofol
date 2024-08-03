using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ThemeContinueState : PopupState
{

    [SerializeField]
    private TMP_Text _findCountText;
    [SerializeField]
    private TMP_Text _findTotalCountText;

    [SerializeField]
    private TMP_Text _plusSecondsGoldText;
    [SerializeField]
    private TMP_Text _plusSecondsADText;

    [SerializeField]
    private BuyButtonCtrl _buyButtonCtrl;
    [SerializeField]
    private Button _btnClose;
    [SerializeField]
    private Button _btnWatchAd;
    [SerializeField]
    private GameObject _watchAdObject;

    [SerializeField]
    private TMP_Text _goldAmount;

    private ThemeGameOverStateData _themeGameOverStateData;

    private ProductInfo _productInfo;
    private bool _isRewarded;

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);
        if (_productInfo == null)
        {
            _productInfo = DataManager.Instance.GetProductInfo(ShopConstants.PRODUCTID_TIME_BONUS);
            _buyButtonCtrl.SetButton(_productInfo, string.Empty, OnBuyComplete);
            _btnClose.onClick.AddListener(OnClose);
            _btnWatchAd.onClick.AddListener(OnWatchAd);
            _plusSecondsGoldText.text = string.Format("+{0} sec", GameConstants.INGAME_ADD_TIME_TIMEBONUS);
            _plusSecondsADText.text = string.Format("+{0} sec", GameConstants.INGAME_ADD_TIME_TIMEBONUS_AD);
        }

        _isRewarded = false;
        _themeGameOverStateData = GetData<ThemeGameOverStateData>();
        _findCountText.text = _themeGameOverStateData.findDifferenceCount.ToString();
        _findTotalCountText.text = "/ " + _themeGameOverStateData.totalDifferenceCount;
        _goldAmount.text = SystemUtil.GetCommaText(DataManager.Instance.UserInfo.Gold);

        _watchAdObject.SetActive(DataManager.Instance.IsEnableRewardAd(ADConstants.REWARD_GAME_CONTINUE));

        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);

    }

    private void UpdateCurrency(object[] args)
    {
        _goldAmount.text = SystemUtil.GetCommaText(DataManager.Instance.UserInfo.Gold);
    }

    #region EVENTS
    private void OnBuyComplete(bool isBuyComplete)
    {
        if (isBuyComplete)
        {  
            GameManager.Instance.InGameData.GameContinueTime = GameConstants.INGAME_ADD_TIME_TIMEBONUS;
            _isRewarded = true;
            OnBack();            
        }
    }
    private void OnClose()
    {
        StateManager.Instance.CloseState(GetType(),false);
        StateManager.Instance.OpenPopupState(typeof(ThemeGameOverState), _themeGameOverStateData);
        StateManager.Instance.ActiveBlockScreen(false);
    }

    public void OnGoldShop()
    {
        ShopState.Open(ShopMainType.GOLD);
    }

    private void OnWatchAd()
    {
        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CONTINUEBTN);

        NetworkManager.Instance.ShowRewardVideoAd(ADConstants.REWARD_GAME_CONTINUE, Reward => {

            _isRewarded = true;

        }, isShowSuccess => {

            if (isShowSuccess && _isRewarded)
            {
                GameManager.Instance.InGameData.GameContinueTime = GameConstants.INGAME_ADD_TIME_TIMEBONUS_AD;
                OnBack();
            }

        });
    }

    public override void OnPostClosed()
    {
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);
        if (_isRewarded)
            ((InGameState)StateManager.Instance.GetRegisteredState(typeof(InGameState))).ContinuePlay();
    }

    #endregion EVENTS
}