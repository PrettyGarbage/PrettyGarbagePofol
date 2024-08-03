using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class PopupContinue : PopupState
{
	public const string PARAM_REWARDED_CALLBACK = "onRewardComplete";
	public const string PARAM_CANCEL_CALLBACK = "onCancel";

	public override string stateName { get { return GameConstants.STATENAME_POPUPCONTINUE; } }

	[Header("Continue")]
	[SerializeField] Button _closeButton;
	[SerializeField] Button _watchButton;

	private Action _onRewardComplete;
	Action onCancel;
	
	public override IEnumerator OnInitialize()
	{
		yield return base.OnInitialize();

		Action<Reward> onRewarded = reward => {
			if (_onRewardComplete != null) {
				_onRewardComplete();
			}
		};

		Action<bool> onClosed = state => {
			if (state) {
				OnBack();
			} else {
				PopupInstanceMsg.ShowPopupInstanceMsg(
					GameConstants.AD_FAIL_MSG,
					2.0f,
					InstanceMsgType.WARNING
				);
			}
		};

		_closeButton.onClick.AddListener(OnBack);
		_watchButton.onClick.AddListener(() => {
			AnalyticsManager.Instance.EventLog(GameConstants.EVENTLOG_BASICMODE_CONTINUE);
			_closeButton.interactable = false;
			_watchButton.interactable = false;
			NetworkManager.Instance.ShowRewardVideoAd(
				GameConstants.ADUNIT_REWARD,
				onRewarded,
				onClosed);
		});
	}

    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_closeButton.interactable = true;
		_watchButton.interactable = true;

		_onRewardComplete = GetParam<Action>(PARAM_REWARDED_CALLBACK, null);
		onCancel = GetParam<Action>(PARAM_CANCEL_CALLBACK, null);

		NetworkManager.Instance.LoadRewardVideoAd(GameConstants.ADUNIT_REWARD);		
	}

	public override void OnUpdate()
	{
		if (_watchButton.interactable)
		{
			base.OnUpdate();
		}
	}

	public override void OnBack()
	{
		base.OnBack();

		NetworkManager.Instance.UnLoadRewardVideoAd(GameConstants.ADUNIT_REWARD);
		
		if (onCancel != null)
		{
			onCancel();
		}
	}
}
