using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdxAdsCtrl : MonoBehaviour, IAdsCtrl
{

	private AdData _adData;
	private GoogleMobileAdsCtrl _googleMobileAdsCtrl;

	private Dictionary<string, AdxBannerRequest> _bannerAdsObjectDic = new Dictionary<string, AdxBannerRequest>();
	private AdxInterstitialRequest _adxBannerRequest;

	// Use this for initialization
	public void Init (AdData adData) {
		_adData = adData;
		
		// debugState 0 : DEFAULT
		// debugState 1 : DEBUG (InEEA)
		ADXGDPRManager.InitializeWithShowADXConsent (adData.BannerAdInfo[0].AdUnitId, 0);
		ADXGDPR.OnADXConsentCompleted += onADXConsentCompleted;

		//Google
		_googleMobileAdsCtrl = gameObject.AddComponent<GoogleMobileAdsCtrl>();
		_googleMobileAdsCtrl.Init(_adData);
	}

    private void onADXConsentCompleted(string obj)
    {
        Debug.Log("onADXConsentCompleted");

		//ADX
		string[] allBannerAdUnits = new string[_adData.BannerAdInfo.Length];
		string[] allInterstitialAdUnits = new string[_adData.InterstitialAdInfo.Length];

		for (int i = 0; i <allBannerAdUnits.Length; i++)
		{
			allBannerAdUnits[i] = _adData.BannerAdInfo[i].AdUnitId;
		}
		for (int i = 0; i < allInterstitialAdUnits.Length; i++)
		{
			allInterstitialAdUnits[i] = _adData.InterstitialAdInfo[i].AdUnitId;
		}

		MoPub.LoadBannerPluginsForAdUnits(allBannerAdUnits);
		MoPub.LoadInterstitialPluginsForAdUnits(allInterstitialAdUnits);

    }

	public void RequestBanner(string adName, AdSize adSize, AdPosition adPosition){

		AdxBannerRequest bannerAdsObject;
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			bannerAdsObject = _bannerAdsObjectDic[adName];
		}else {
			GameObject gameObject = new GameObject("BannerAdsObject_"+adName);
			bannerAdsObject = gameObject.AddComponent<AdxBannerRequest>();
			_bannerAdsObjectDic.Add(adName, bannerAdsObject);
		}

		MoPub.AdPosition mAdPosition = MoPub.AdPosition.BottomCenter;
		switch (adPosition)
		{
			case AdPosition.Bottom:
				mAdPosition = MoPub.AdPosition.BottomCenter;
				break;
			case AdPosition.BottomLeft:
				mAdPosition = MoPub.AdPosition.BottomLeft;
				break;
			case AdPosition.BottomRight:
				mAdPosition = MoPub.AdPosition.BottomRight;
				break;
			case AdPosition.Center:
				mAdPosition = MoPub.AdPosition.Centered;
				break;
			case AdPosition.Top:
				mAdPosition = MoPub.AdPosition.TopCenter;
				break;
			case AdPosition.TopLeft:
				mAdPosition = MoPub.AdPosition.TopLeft;
				break;
			case AdPosition.TopRight:
				mAdPosition = MoPub.AdPosition.TopRight;
				break;			
		}


		bannerAdsObject.RequestBanner(_adData.GetAdUnitId(AdType.BANNER, adName), mAdPosition);
	}

	public void RemoveBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].RemoveBanner();
		}
	}

	public void HideBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].HideBanner();
		}
	}

	public void ShowBanner(string adName){
		if(_bannerAdsObjectDic.ContainsKey(adName)){
			_bannerAdsObjectDic[adName].ShowBanner();
		}
	}

	public void LoadInterstitialAd(string adName){
		if(_adxBannerRequest == null || !_adxBannerRequest){
			_adxBannerRequest =  new GameObject("adxBannerRequest_"+adName).AddComponent<AdxInterstitialRequest>();
		}
		_adxBannerRequest.LoadInterstitialAd(_adData.GetAdUnitId(AdType.INTERSTITIAL, adName));
	}

	public void ShowInterstitialAd(string adName, Action<bool> onClosed){
		_adxBannerRequest.ShowInterstitialAd(_adData.GetAdUnitId(AdType.INTERSTITIAL, adName), onClosed);
	}

	public void LoadRewardVideoAd(string adName){
		_googleMobileAdsCtrl.LoadRewardVideoAd(adName);
	}

	public void UnLoadRewardVideoAd(string adName){
		_googleMobileAdsCtrl.UnLoadRewardVideoAd(adName);
	}

	public void ShowRewardVideoAd(string adName, Action<Reward> _rewardCallback, Action<bool> callback){
		_googleMobileAdsCtrl.ShowRewardVideoAd(adName, _rewardCallback, callback);
	}
	
}
