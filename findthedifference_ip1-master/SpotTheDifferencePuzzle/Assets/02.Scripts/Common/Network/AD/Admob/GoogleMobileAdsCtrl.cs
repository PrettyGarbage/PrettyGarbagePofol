using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
//using static GoogleMobileAds.Api.AdLoader;

public class GoogleMobileAdsCtrl : MonoBehaviour {

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
		if(_admobData!=null)
			MobileAds.Initialize(_admobData.AppId);

		_interstitialAdFactory = new InterstitialAdFactory(admobData);
        
    }

    public void PreLoadRewardAd()
    {
        _interstitialAdFactory.PreLoadRewardAd();
    }

    //a_184856526086757532 | 2E3F6BCDA4B35480858F6A3DABA5555F
    public static AdRequest GetAdRequest(AdData admobData)
    {
		AdRequest.Builder builder = new AdRequest.Builder();;
		for (int i = 0; i < admobData.TestUserInfos.Length; i++)
		{
			builder.AddTestDevice(admobData.TestUserInfos[i].DeviceId);	
		}

		return builder.Build();
	}

	

	public void RequestBanner(string adName, AdSize adSize, AdPosition adPosition){

        if (_admobData == null) return;

		BannerAdsObject bannerAdsObject;
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			bannerAdsObject = _bannerAdsObjectDic[adName];
		}else {
			GameObject gameObject = new GameObject("BannerAdsObject_"+adName);
			bannerAdsObject = gameObject.AddComponent<BannerAdsObject>();
			_bannerAdsObjectDic.Add(adName, bannerAdsObject);
		}

		bannerAdsObject.SetAd(adSize, adPosition);
		bannerAdsObject.RequestAd(adName, _admobData.GetAdUnitId(AdType.BANNER, adName), GetAdRequest(_admobData), null);
	}

	public void RemoveBanner(string adName){

        if (_admobData == null) return;

        if (_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].DestroyAd();
		}
	}

	public void HideBanner(string adName){

        if (_admobData == null) return;

        if (_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].HideAd();
		}
	}

	public void ShowBanner(string adName){

        if (_admobData == null) return;

        if (_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].ShowAd();
		}
	}

	public void LoadInterstitialAd(string adName){

        if (_admobData == null) return;

        _interstitialAdFactory.RequestLoadInterstitialAd(adName);		
	}

	public void ShowInterstitialAd(string adName, Action<bool> onClosed){

        if (_admobData == null){ onClosed(false); return; }

        _interstitialAdFactory.ShowLoadInterstitialAd(adName, onClosed);
	}

	//public void LoadRewardVideoAd(string adName){

 //       if (_admobData == null) return;
 //       string adUnit = _admobData.GetAdUnitId(AdType.REWARD, adName);
 //       Debug.Log("LoadRewardVideoAd : " + adName + "/ adUnit: " + adUnit);
 //       //_interstitialAdFactory.RequestLoadRewardAd(adName, adUnit, GetAdRequest());
	//}

	public void ShowRewardVideoAd(string adName, Action<Reward> _rewardCallback, Action<bool> callback){

        if (_admobData == null) { callback(false); return; }
        Debug.Log("ShowRewardVideoAd : " + adName);
        _interstitialAdFactory.ShowLoadRewardAd(adName, _rewardCallback, callback);
	}
	
}
