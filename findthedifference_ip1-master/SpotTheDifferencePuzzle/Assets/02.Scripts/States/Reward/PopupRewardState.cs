using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardStateData : BaseStateData
{
    public List<ProductItemInfo> productItemInfos;
    public Action onClosed;

    public PopupRewardStateData() { }
    public PopupRewardStateData(List<ProductItemInfo> productItemInfos, Action onClosed)
    {
        this.productItemInfos = productItemInfos;
    }
}

public class PopupRewardState : PopupState
{
    [SerializeField]
    private Image _itemImage;
    [SerializeField]
    private LocalizationUIText _rewardDescText;

    private PopupRewardStateData _popupRewardStateData;

    public static void Open(List<ProductItemInfo> productItemInfos, Action onClosed=null)
    {  
        PopupRewardStateData popupRewardStateData = new PopupRewardStateData(productItemInfos, onClosed);
        StateManager.Instance.OpenPopupState(typeof(PopupRewardState), popupRewardStateData);

    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);
        _popupRewardStateData =  GetData<PopupRewardStateData>();

        if(_popupRewardStateData.productItemInfos!=null && _popupRewardStateData.productItemInfos.Count > 0)
        {
            ProductItemInfo productItemInfo =  _popupRewardStateData.productItemInfos[0];
            BaseSpriteData baseSpriteData = ResourceDataManager.Instance.GetBaseSpriteData(productItemInfo);
            _itemImage.sprite = baseSpriteData.Sprite;
            string itemName = LocalizationManager.Instance.GetText(baseSpriteData.LocalizedTextKey);
            string rewardCount = productItemInfo.count.ToString();
            if(productItemInfo.bonusCount> 0)
            {
                rewardCount+= "(+" + productItemInfo.bonusCount+")";
            }
            _rewardDescText.SetText(LocalizationTextKey.REWARD_DESC, itemName, rewardCount);
        }
    }

    public override IEnumerator OnPostOpen()
    {
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_get_reward);

        return base.OnPostOpen();
    }

    public override void OnBack()
    {
        base.OnBack();
        if(_popupRewardStateData.onClosed!=null)
            _popupRewardStateData.onClosed();

        _popupRewardStateData = null;
    }
}

