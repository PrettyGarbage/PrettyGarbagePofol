using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PopupSpinEventRewardState : PopupState
{
	[Header("SpinEventReward")]
    [SerializeField]
    private LocalizationUIText _rewardDescText;

    [SerializeField]
    private Animator _animator;

    private PopupRewardStateData _popupRewardStateData;
    private bool _isShow;

    public static void Open(List<ProductItemInfo> productItemInfos, Action onClosed = null)
    {
        PopupRewardStateData popupRewardStateData = new PopupRewardStateData(productItemInfos, onClosed);
        StateManager.Instance.OpenPopupState(typeof(PopupSpinEventRewardState), popupRewardStateData);
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);
        _isShow = false;
        _popupRewardStateData = GetData<PopupRewardStateData>();
        _rewardDescText.SetText(LocalizationTextKey.LUCKYSPIN_REWARD_JACKPOT);

    }

    public override IEnumerator OnPostOpen()
    {
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.eff_reward_jackpot);
        return base.OnPostOpen();
    }

    public override IEnumerator OnOpening()
    {

        _animator.GetBehaviours<BaseAniBehaviour>()[0].onStateExit = stateInfo =>
        {
            _isShow = true;
        };

        _popup.transform.localScale = Vector3.one;
        
        _animator.Play("Open", -1, 0f);

        while (!_isShow)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
    }

    public override void OnBack()
    {
        base.OnBack();
        if (_popupRewardStateData.onClosed != null)
            _popupRewardStateData.onClosed();

        _popupRewardStateData = null;
    }

}