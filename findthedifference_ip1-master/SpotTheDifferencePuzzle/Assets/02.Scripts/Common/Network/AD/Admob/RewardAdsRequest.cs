using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class RewardAdsRequest : MonoBehaviour {

	private RewardedAd _rewardedAd;
	private Action<bool> _onClosed;
    private Action<Reward> _rewardCallback;
    private string _adName;

    AdRequestStatus _requestStatus = AdRequestStatus.NONE;
    bool _isReward = false;
    Reward _reward;

    public string AdName { get { return _adName; } }

    public void RequestAd(string adName, string adUnitId, AdRequest adRequest){

        if (_requestStatus == AdRequestStatus.LOADING || _requestStatus == AdRequestStatus.LOADED)
        {
            Debug.Log("Cancel RequestAd  adName : " + adName + ", requestStatus : " + _requestStatus);
            return;
        }

        _rewardedAd = new RewardedAd(adUnitId);

        _rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        _rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        _rewardedAd.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when an ad request failed to show.
        _rewardedAd.OnAdFailedToShow += HandleRewardBasedVideoFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        _rewardedAd.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        _rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;

        _rewardedAd.SetServerSideVerificationOptions(new ServerSideVerificationOptions.Builder().SetUserId(DataManager.Instance.UserInfo.Id.ToString()).Build());

        _adName = adName;
        _requestStatus = AdRequestStatus.LOADING;
        this._rewardedAd.LoadAd(adRequest);
    }

    public void ShowAd(Action<Reward> rewardCallback, Action<bool> onClosed){		
        
        _onClosed = onClosed;
        if(_rewardedAd==null) {
			CallbackClose(false);
			return;
		}

		if(_requestStatus == AdRequestStatus.FAIL){
			Debug.LogWarning("직전 요청한 광고의 Load가 실패했습니다.");
			CallbackClose(false);
			return;
		}
		
		_rewardCallback = rewardCallback;
        
		StartCoroutine(ProcessOnAdShow());
	}

    IEnumerator ProcessOnAdShow(){

        _isReward = false;
        _reward = null;

        Debug.Log("ProcessOnAdShow Reward AD RequestStatus : " + _requestStatus);

        while(_requestStatus == AdRequestStatus.LOADING){
			yield return CommonConstants.WaitLoopSeconds;
		}

        if(_requestStatus == AdRequestStatus.FAIL){
			CallbackClose(false);
		}
		else {
            _requestStatus = AdRequestStatus.SHOW;
            _rewardedAd.Show();

            while (_requestStatus == AdRequestStatus.SHOW)
            {
                yield return new WaitForEndOfFrame();
            }

            if(_isReward && _rewardCallback!=null){
                _rewardCallback(_reward);
                _rewardCallback = null;
            }

            CallbackClose(_requestStatus == AdRequestStatus.CLOSED);
        }
        
	}

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleRewardBasedVideoLoaded event received " + _adName);	
        _requestStatus = AdRequestStatus.LOADED;
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdErrorEventArgs args)
    {
        _requestStatus = AdRequestStatus.FAIL;
        MonoBehaviour.print(
            "gbros HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message + " " + _adName) ;

    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleRewardBasedVideoOpened event received"+ " " + _adName);
    }

    private void HandleRewardBasedVideoFailedToShow(object sender, AdErrorEventArgs args)
    {
        _requestStatus = AdRequestStatus.FAIL;
        MonoBehaviour.print(
            "gbros HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message + " " + _adName);
    }


    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleRewardBasedVideoStarted event received"+ " " + _adName);
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        _requestStatus = AdRequestStatus.CLOSED;
        MonoBehaviour.print("gbros HandleRewardBasedVideoClosed event received"+ " " + _adName);
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        _reward = args;
        _isReward = true;
        MonoBehaviour.print(
            "gbros HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type+ " " + _adName);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleRewardBasedVideoLeftApplication event received"+ " " + _adName);
    }

    public void CallbackClose(bool isCloseSuccess){
        _requestStatus = AdRequestStatus.NONE;
        if(_onClosed!=null){
			_onClosed(isCloseSuccess);
			_onClosed = null;
		}

        if (_rewardedAd != null)
        {
            _rewardedAd.OnAdLoaded -= HandleRewardBasedVideoLoaded;
            _rewardedAd.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
            _rewardedAd.OnAdOpening -= HandleRewardBasedVideoOpened;
            _rewardedAd.OnAdFailedToShow -= HandleRewardBasedVideoFailedToShow;
            _rewardedAd.OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
            _rewardedAd.OnAdClosed -= HandleRewardBasedVideoClosed;
            _rewardedAd = null;
        }
	}

}
