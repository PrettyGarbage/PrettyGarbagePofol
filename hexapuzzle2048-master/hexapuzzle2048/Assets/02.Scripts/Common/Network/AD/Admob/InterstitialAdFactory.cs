using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdFactory  {

	private Dictionary<string, InterstitialAdsRequest> _interstitialAdsDic = new Dictionary<string, InterstitialAdsRequest>();
	private RewardAdsRequest _rewardAdsRequest;

	public void RequestLoadInterstitialAd(string adName, string adUnitId, AdRequest adRequest){
		if(_interstitialAdsDic.ContainsKey(adName)){			
			Debug.LogWarning("gbros 이미 요청한 광고가 존재합니다. " + adName);
			return;					
		}

		InterstitialAdsRequest interstitialAdsRequest = new GameObject("interstitialAdsRequest_"+adName).AddComponent<InterstitialAdsRequest>();
		interstitialAdsRequest.RequestAd(adName, adUnitId, adRequest);
		_interstitialAdsDic.Add(adName, interstitialAdsRequest);		
	}

	public void ShowLoadInterstitialAd(string adName, Action<bool> onClosed){
		if(_interstitialAdsDic.ContainsKey(adName)){
			_interstitialAdsDic[adName].ShowAd(onClosed);
			_interstitialAdsDic.Remove(adName);
		}
		else {
			if(onClosed!=null)
				onClosed(false);
			Debug.LogWarning("gbros 요청한 광고가 존재하지 않습니다. " + adName);							
		}
	}

	public void RequestLoadRewardAd(string adName, string adUnitId, AdRequest adRequest){
		if(_rewardAdsRequest==null){
			_rewardAdsRequest = new GameObject("RewardAdsRequest_"+adName).AddComponent<RewardAdsRequest>();
		}
		_rewardAdsRequest.RequestAd(adName, adUnitId, adRequest);		
	}

	public void UnLoadRewardAd(string adName){
		if(_rewardAdsRequest != null){		
			_rewardAdsRequest.UnLoad();			
		}
	}

	public void ShowLoadRewardAd(string adName, Action<Reward> rewardCallback, Action<bool> onClosed){
		if(_rewardAdsRequest == null){
			Debug.LogWarning("gbros 요청한 광고가 존재하지 않습니다. " + adName);	
			if(onClosed!=null)
				onClosed(false);					
				return;
		}

		if(!_rewardAdsRequest.AdName.Equals(adName)){
			Debug.LogWarningFormat("gbros 로드되어 있는 광고가 요청광고와 다릅니다. 로드된 광고 : {0}, 요청 광고 : {1} " ,_rewardAdsRequest.AdName, adName);	
			if(onClosed!=null)
				onClosed(false);					
				return;
		}

		_rewardAdsRequest.ShowAd(rewardCallback, onClosed);
	}
	
}
