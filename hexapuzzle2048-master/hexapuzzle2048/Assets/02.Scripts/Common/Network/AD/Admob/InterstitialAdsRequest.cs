using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdsRequest : MonoBehaviour {

	private InterstitialAd _interstitialAd;
	private Action<bool> _onClosed;
	
	string _adName;
	AdRequestStatus _requestStatus = AdRequestStatus.NONE;
	
	public void RequestAd(string adName, string adUnitId, AdRequest adRequest)
	{
		_adName = adName;
		// Initialize an InterstitialAd.
		_interstitialAd = new InterstitialAd(adUnitId);

		// Called when an ad request has successfully loaded.
		_interstitialAd.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		_interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is shown.
		_interstitialAd.OnAdOpening += HandleOnAdOpened;
		// Called when the ad is closed.
		_interstitialAd.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		_interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

		_requestStatus = AdRequestStatus.LOADING;	
		_interstitialAd.LoadAd(adRequest);
	}

	public void ShowAd(Action<bool> onClosed){		

		_onClosed = onClosed;
		
		if(_interstitialAd==null) {
			CallbackClose(false);
			return;
		}

		if(_requestStatus == AdRequestStatus.FAIL){
			Debug.LogWarning("직전 요청한 광고의 Load가 실패했습니다.");
			CallbackClose(false);
			return;
		}
		
		
		StartCoroutine(ProcessOnAdShow());
	}

	IEnumerator ProcessOnAdShow(){

        while(_requestStatus == AdRequestStatus.LOADING){
			yield return CommonConstants.WaitLoopSeconds;
		}

		if(_requestStatus == AdRequestStatus.FAIL){
			CallbackClose(false);
		}
		else {
			_requestStatus = AdRequestStatus.SHOW;
			_interstitialAd.Show();

			while (_requestStatus == AdRequestStatus.SHOW)
			{
				yield return CommonConstants.WaitLoopSeconds;
			}

			CallbackClose(_requestStatus == AdRequestStatus.CLOSED);
		}
		
	}

	private void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdLoaded event received " + _adName);
		_requestStatus = AdRequestStatus.LOADED;

    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
		_requestStatus = AdRequestStatus.FAIL;
        MonoBehaviour.print("gbros HandleFailedToReceiveAd event received with message: "
                            + args.Message + " -" + _adName);
    }

    private void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdOpened event received"+ " -" + _adName);
    }

    private void HandleOnAdClosed(object sender, EventArgs args)
    {
		_requestStatus = AdRequestStatus.CLOSED;
        MonoBehaviour.print("gbros HandleAdClosed event received"+ " -" + _adName);
    }

    private void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdLeavingApplication event received"+ " -" + _adName);
    }

	public void CallbackClose(bool isCloseSuccess){
		
		_requestStatus = AdRequestStatus.NONE;

		if(_onClosed!=null){
			_onClosed(isCloseSuccess);
			_onClosed = null;
		}

		if(_interstitialAd!=null){
			_interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
			_interstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
			_interstitialAd.OnAdOpening -= HandleOnAdOpened;
			_interstitialAd.OnAdClosed -= HandleOnAdClosed;
			_interstitialAd.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
			_interstitialAd.Destroy();
			_interstitialAd = null;
		}

	}	
}
