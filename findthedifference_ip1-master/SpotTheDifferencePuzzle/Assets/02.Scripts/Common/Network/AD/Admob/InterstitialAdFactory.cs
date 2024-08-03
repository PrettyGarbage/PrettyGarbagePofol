using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdFactory  {

    private AdData _admobData;

    private Dictionary<string, InterstitialAdsRequest> _interstitialAdsDic = new Dictionary<string, InterstitialAdsRequest>();
    private Dictionary<string, RewardAdsRequest> _rewardAdsDic = new Dictionary<string, RewardAdsRequest>();
    
    public InterstitialAdFactory(AdData admobData)
    {
        _admobData = admobData;       
    }

    public void PreLoadRewardAd()
    {
        //초기 리워드 로드.
        AdmobAdUnitIdInfo[] admobAdUnitIdInfos = _admobData.RewardAdInfo;
        for (int i = 0; i < admobAdUnitIdInfos.Length; i++)
        {
            if (DataManager.Instance.IsEnableRewardAd(admobAdUnitIdInfos[i].AdName))
            {
                RequestLoadRewardAd(admobAdUnitIdInfos[i].AdName);
            }
        }
    }

	public void RequestLoadInterstitialAd(string adName){
		if(_interstitialAdsDic.ContainsKey(adName)){			
			Debug.LogWarning("gbros 이미 요청한 광고가 존재합니다. " + adName);
			return;					
		}

		InterstitialAdsRequest interstitialAdsRequest = new GameObject("interstitialAdsRequest_"+adName).AddComponent<InterstitialAdsRequest>();
		interstitialAdsRequest.RequestAd(adName, _admobData.GetAdUnitId(AdType.INTERSTITIAL, adName), GoogleMobileAdsCtrl.GetAdRequest(_admobData));
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

	private void RequestLoadRewardAd(string adName){

        if (_rewardAdsDic.ContainsKey(adName))
        {
            Debug.LogWarning("gbros 이미 요청한 광고가 존재합니다. " + adName);
            return;
        }

        RewardAdsRequest rewardAdsRequest = new GameObject("rewardAdsRequest_" + adName).AddComponent<RewardAdsRequest>();
        rewardAdsRequest.RequestAd(adName, _admobData.GetAdUnitId(AdType.REWARD, adName), GoogleMobileAdsCtrl.GetAdRequest(_admobData));
        _rewardAdsDic.Add(adName, rewardAdsRequest);
	}

	public void ShowLoadRewardAd(string adName, Action<Reward> rewardCallback, Action<bool> onClosed){
        if (_rewardAdsDic.ContainsKey(adName))
        {
            RewardAdsRequest rewardAdsRequest = _rewardAdsDic[adName];
            _rewardAdsDic.Remove(adName);
            rewardAdsRequest.ShowAd(rewardCallback, (isSuccess)=> {
                onClosed(isSuccess);
                RequestLoadRewardAd(adName);
            });
        }
        else
        {
            Debug.LogWarning("gbros 요청한 광고가 존재하지 않습니다. " + adName);
            RequestLoadRewardAd(adName);
            ShowLoadRewardAd(adName, rewardCallback, onClosed);
        }
	}	
}
