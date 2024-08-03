using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCoinBox : PopupState
{
	public const string PARAM_REWARDED_CALLBACK = "Rewarded";

	public override string stateName { get { return GameConstants.STATENAME_POPUPCOINBOX; } }

	[Header("Show Hide Objects")]
	[SerializeField] GameObject _beforeObject;
	[SerializeField] GameObject _afterObject;

	[Header("Coin Box")]
	[SerializeField] Button _closeButton;
	[SerializeField] Button _watchButton;
	[SerializeField] Button _claimButton;

	[SerializeField]
	private TMP_Text _rewardCoinText;
	[SerializeField]
	private TMP_Text _useCountText;

	[SerializeField] Color _validColor = Color.white;
	[SerializeField] Color _invalidColor = Color.red;

	[Header("Box Animator")]
	[SerializeField]
	private UIChestBox _uiChestBox;

	Action _onRewarded;
	bool _rewarded;

	public override IEnumerator OnInitialize()
	{
		yield return base.OnInitialize();

		_closeButton.onClick.AddListener(OnBack);
		_claimButton.onClick.AddListener(OnBack);

		_watchButton.onClick.AddListener(() => {
			_closeButton.interactable = false;
			_watchButton.interactable = false;
			NetworkManager.Instance.ShowRewardVideoAd(
				GameConstants.ADUNIT_REWARD,
				OnAdRewarded,
				OnAdClosed
			);
		});
	}
	
    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_beforeObject.SetActive(true);
		_afterObject.SetActive(false);
		_uiChestBox.PlayIdle();

		_rewarded = false;
		_onRewarded = GetParam<Action>(PARAM_REWARDED_CALLBACK, null);

		DataManager.Instance.UserData.SetCoinBox();
		
		_rewardCoinText.text = SystemUtil.GetCommaText(
			DataManager.Instance.GameData.CoinBoxCoin
		);
		
		int coinBoxRewardedCount = DataManager.Instance.UserData.UserRewardInfo.CoinBoxRewardCount;
		int coinBoxDailyLimit = DataManager.Instance.GameData.CoinBoxDailyLimit;
		long coinBoxCoolTimeSeconds = DataManager.Instance.UserData.UserRewardInfo.GetCoinBoxCoolTimeSeconds();
		bool isValid = (coinBoxRewardedCount < coinBoxDailyLimit && coinBoxCoolTimeSeconds <= 0);

		stringBuilder.Remove(0, stringBuilder.Length);
		stringBuilder.Append(coinBoxRewardedCount);
		stringBuilder.Append("/");
		stringBuilder.Append(coinBoxDailyLimit);
		_useCountText.text =  stringBuilder.ToString();

		_closeButton.interactable = true;
		_watchButton.interactable = isValid;
		_useCountText.color = isValid ? _validColor : _invalidColor;

		if (isValid)
		{
			NetworkManager.Instance.LoadRewardVideoAd(
				GameConstants.ADUNIT_REWARD
			);
		}
	}

	public override void OnUpdate()
	{
		if (_closeButton.interactable || _afterObject.activeSelf)
		{
			base.OnUpdate();
		}
	}


	public override IEnumerator OnUnload()
	{
		NetworkManager.Instance.UnLoadRewardVideoAd(GameConstants.ADUNIT_REWARD);
		if (_rewarded)
		{
			DataManager.Instance.UserData.AddCoinBoxReward();
			if (_onRewarded != null)
			{
				_onRewarded();
			}
		}

		yield return base.OnUnload();
	}

	private void OnAdRewarded(Reward obj)
    {
        _rewarded = true;
    }

    private void OnAdClosed(bool isNormalClose)
    {
		if (!isNormalClose)
		{
			PopupInstanceMsg.ShowPopupInstanceMsg(
				GameConstants.AD_FAIL_MSG,
				2.0f,
				InstanceMsgType.WARNING
			);
		}
		else
		{
        	StartCoroutine(OnAdClosedProcess());
		}
    }

	IEnumerator OnAdClosedProcess()
	{
		yield return null;

		if (_rewarded)
		{
			_beforeObject.SetActive(false);
			_afterObject.SetActive(true);

			_uiChestBox.PlayOpenBox();			
		}
		else
		{
			_closeButton.interactable = true;
			_watchButton.interactable = true;
		}
	}
}