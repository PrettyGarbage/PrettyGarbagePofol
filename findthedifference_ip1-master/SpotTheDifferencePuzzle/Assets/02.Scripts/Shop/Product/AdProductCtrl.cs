using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdProductCtrl : MonoBehaviour {

    [SerializeField]
    protected Image _productIconImage;
    [SerializeField]
    private string _adName;
    [SerializeField]
    private Button _buttonWatchAd;

    private bool _isRewarded;

    public void SetProductInfo(Sprite productIcon)
    {
        _productIconImage.sprite = productIcon;
        SetWatchAdButton();
    }

    private void SetWatchAdButton()
    {
        _buttonWatchAd.interactable = DataManager.Instance.IsEnableRewardAd(_adName);
    }

    public void OnWatchAd()
    {
        if (DataManager.Instance.IsEnableRewardAd(_adName))
        {
            NetworkManager.Instance.ShowRewardVideoAd(_adName, Reward => {

                _isRewarded = true;

            }, isShowSuccess => {

                if (isShowSuccess && _isRewarded)
                {
                    PopupInstantState.Open(PopupInstantType.PURCHSE_SUCCESS, string.Empty);
                    DataManager.Instance.AddRewardAdCount(_adName);
                    SetWatchAdButton();
                }

            });
        }
        
    }

}
