using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using static GoogleMobileAds.Api.AdLoader;

public class GoogleMobileAdsCtrl : MonoBehaviour, IAdsCtrl {

	private AdData _admobData;

// 구글 테스트 APPID
// 배너 광고 ca-app-pub-3940256099942544/6300978111
// 전면 광고 ca-app-pub-3940256099942544/1033173712
// 전면 동영상 광고	ca-app-pub-3940256099942544/8691691433
// 보상형 동영상 광고 ca-app-pub-3940256099942544/5224354917

	private InterstitialAdFactory _interstitialAdFactory;
	private Dictionary<string, BannerAdsObject> _bannerAdsObjectDic = new Dictionary<string, BannerAdsObject>();
	
	// Use this for initialization
	public void Init (AdData admobData) {
		_admobData = admobData;		
		_interstitialAdFactory = new InterstitialAdFactory();
		if(_admobData!=null)
			MobileAds.Initialize(_admobData.AppId);
	}

	//a_184856526086757532 | 2E3F6BCDA4B35480858F6A3DABA5555F
	private AdRequest GetAdRequest(){
		AdRequest.Builder builder = new AdRequest.Builder();;
		for (int i = 0; i < _admobData.TestUserInfos.Length; i++)
		{
			builder.AddTestDevice(_admobData.TestUserInfos[i].DeviceId);	
		}

		return builder.Build();
	}

	

	public void RequestBanner(string adName, AdSize adSize, AdPosition adPosition){

		BannerAdsObject bannerAdsObject;
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			bannerAdsObject = _bannerAdsObjectDic[adName];
		}else {
			GameObject gameObject = new GameObject("BannerAdsObject_"+adName);
			bannerAdsObject = gameObject.AddComponent<BannerAdsObject>();
			_bannerAdsObjectDic.Add(adName, bannerAdsObject);
		}

		bannerAdsObject.SetAd(adSize, adPosition);
		bannerAdsObject.RequestAd(adName, _admobData.GetAdUnitId(AdType.BANNER, adName), GetAdRequest(), null);
	}

	public void RemoveBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].DestroyAd();
		}
	}

	public void HideBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].HideAd();
		}
	}

	public void ShowBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].ShowAd();
		}
	}

	public void LoadInterstitialAd(string adName){
		_interstitialAdFactory.RequestLoadInterstitialAd(adName,_admobData.GetAdUnitId(AdType.INTERSTITIAL, adName), GetAdRequest());		
	}

	public void ShowInterstitialAd(string adName, Action<bool> onClosed){
		_interstitialAdFactory.ShowLoadInterstitialAd(adName, onClosed);
	}

	public void LoadRewardVideoAd(string adName){
		_interstitialAdFactory.RequestLoadRewardAd(adName, _admobData.GetAdUnitId(AdType.REWARD, adName), GetAdRequest());
	}

	public void UnLoadRewardVideoAd(string adName){
		_interstitialAdFactory.UnLoadRewardAd(adName);
	}

	public void ShowRewardVideoAd(string adName, Action<Reward> _rewardCallback, Action<bool> callback){
		_interstitialAdFactory.ShowLoadRewardAd(adName, _rewardCallback, callback);
	}
	
}
