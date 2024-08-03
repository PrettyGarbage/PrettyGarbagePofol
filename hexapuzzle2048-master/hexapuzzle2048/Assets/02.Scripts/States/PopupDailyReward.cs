using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDailyReward : PopupState
{
	public override string stateName { get { return GameConstants.STATENAME_POPUPDAILYREWARD; } }

	[Header("Daily Reward")]
	[SerializeField] Button _okButton;
	[SerializeField] Button _watchButton;

	[SerializeField]
	private TMP_Text _rewardCoinText;

	[Header("Box Animator")]
	[SerializeField]
	private UIChestBox _uiChestBox;

	public override IEnumerator OnInitialize()
	{
		_okButton.onClick.AddListener(OnBack);

		_watchButton.onClick.AddListener(()=>{
			_watchButton.interactable = false;
			NetworkManager.Instance.ShowRewardVideoAd(
				GameConstants.ADUNIT_REWARD,
				OnRewardCallback,
				OnAdClosed);
		});

		yield return base.OnInitialize();
	}

    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_uiChestBox.PlayIdle();

		_watchButton.interactable = true;

		DataManager.Instance.UserData.SetDailyReward();

		_rewardCoinText.text = SystemUtil.GetCommaText(
			DataManager.Instance.GameData.DailyBonusCoin
		);
		
		NetworkManager.Instance.LoadRewardVideoAd(
			GameConstants.ADUNIT_REWARD
		);
	}

	public override IEnumerator OnEntered(){
		yield return base.OnEntered();
		_uiChestBox.PlayOpenBox();
	}

    private void OnRewardCallback(Reward obj)
    {
        DataManager.Instance.UserData.AddCoin(
			DataManager.Instance.GameData.DailyBonusCoin
		);
    }

	private void OnAdClosed(bool isCloseSuccess)
    {
		if(isCloseSuccess){
        	OnBack();
		}else {
			PopupInstanceMsg.ShowPopupInstanceMsg(GameConstants.AD_FAIL_MSG, 2f, InstanceMsgType.WARNING);
		}
    }
	
	public override void OnBack()
	{
		base.OnBack();
		NetworkManager.Instance.UnLoadRewardVideoAd(
			GameConstants.ADUNIT_REWARD
		);
	}
}
