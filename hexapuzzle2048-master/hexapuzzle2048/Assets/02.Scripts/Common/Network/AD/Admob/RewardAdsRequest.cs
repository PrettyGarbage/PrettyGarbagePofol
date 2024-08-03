using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class RewardAdsRequest : MonoBehaviour {

	private RewardBasedVideoAd _rewardBasedVideo;
	private Action<bool> _onClosed;
    private Action<Reward> _rewardCallback;
    private string _adName;

    AdRequestStatus _requestStatus = AdRequestStatus.NONE;
    bool _isReward = false;
    Reward _reward;

    public string AdName { get { return _adName; } }

    void InitEvent(){
		// Get singleton reward based video ad reference.
        this._rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        _rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        _rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        _rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        _rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        _rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        _rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        _rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
        
	}

    public void RequestAd(string adName, string adUnitId, AdRequest adRequest){
		if(_rewardBasedVideo==null){
			InitEvent();
		}
		_adName = adName;
        _requestStatus = AdRequestStatus.LOADING;
        this._rewardBasedVideo.LoadAd(adRequest, adUnitId);
    }

    public void ShowAd(Action<Reward> rewardCallback, Action<bool> onClosed){		
        
        _onClosed = onClosed;
        if(_rewardBasedVideo==null) {
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

        while(_requestStatus == AdRequestStatus.LOADING){
			yield return CommonConstants.WaitLoopSeconds;
		}

        if(_requestStatus == AdRequestStatus.FAIL){
			CallbackClose(false);
		}
		else {
            _requestStatus = AdRequestStatus.SHOW;
            _rewardBasedVideo.Show();

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

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
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
	}

    public void UnLoad(){
         
    }

}
