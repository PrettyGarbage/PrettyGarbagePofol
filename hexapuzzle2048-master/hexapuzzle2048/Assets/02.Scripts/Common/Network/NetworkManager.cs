using GoogleMobileAds.Api;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class NetworkManager : MonoBehaviour {

    private static NetworkManager _instance;
    
    private IMobilePlatform _mobilePlatform;
    private IAdsCtrl _adsCtrl;

    [SerializeField]
    private AdData _adDataForAndroid;
    [SerializeField]
    private AdData _adDataForIos;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<NetworkManager>("NetworkManager");                
            }

            return _instance;
        }
    }

    public IEnumerator Init(){
        AnalyticsManager.Instance.Init();
#if UNITY_ANDROID
        AdData adData = _adDataForAndroid;
        _mobilePlatform = gameObject.AddComponent<GooglePlayServiceCtrl>();
#elif UNITY_IOS
        AdData adData = _adDataForIos;
        _mobilePlatform = gameObject.AddComponent<GameCenterServiceCtrl>();
#endif
        _mobilePlatform.Init();

        if (adData.adAdaptorType == AdAdaptorType.ADMOB){
            _adsCtrl = gameObject.AddComponent<GoogleMobileAdsCtrl>();
        }
        if(adData.adAdaptorType == AdAdaptorType.ADAX){
            _adsCtrl = gameObject.AddComponent<AdxAdsCtrl>();
        }
        _adsCtrl.Init(adData);
        yield return null;
    }

    /// <summary>
    /// https://github.com/yasirkula/UnityNativeShare
    /// Apps the share.
    /// </summary>
    public void AppShare(){
        string text;
#if UNITY_ANDROID
        text = CommonConstants.SHARE_APPLINK_MSG + CommonConstants.SHARE_APPLINK_ANDROID + CommonConstants.ANDROID_PACKAGE_NAME;
#elif UNITY_IOS
        text = CommonConstants.SHARE_APPLINK_MSG + CommonConstants.SHARE_APPLINK_IOS;
#endif
        new NativeShare()
            .SetTitle(CommonConstants.SHARE_APPLINK_TITLE)
            .SetText(text)
            .SetSubject("")
            .Share();           
    }

#region Platform Service
    public void SetScoreToLeaderBorad(long score)
    {
        _mobilePlatform.AddScoreToLeaderBorad(score);
    }

    public void ShowLeaderBoard()
    {
        if(!IsLoggedIn()){
            SignIn((bool isloggin)=>{
                if(isloggin){
                    _mobilePlatform.ShowLeaderBoard();
                }
            });
        }else {
            _mobilePlatform.ShowLeaderBoard();
        }
    }

    public bool IsLoggedIn()
    {
        return _mobilePlatform ==null ? false : _mobilePlatform.IsLoggedIn();
    }

    public void SignIn(ActionBool onLogIn=null)
    {
        _mobilePlatform.LogIn(onLogIn);
    }

    public void SignOut()
    {
        _mobilePlatform.LogOut();
    }

    public string GetUserId(){
        return _mobilePlatform.GetUserId();
    }
    #endregion

    #region Ads
    public void RequestBanner(string adName, AdSize adSize, AdPosition adPosition){
		_adsCtrl.RequestBanner(adName, adSize, adPosition);
	}

    public void RemoveBanner(string adName){
		_adsCtrl.RemoveBanner(adName);
	}

    public void HideBanner(string adName){
		_adsCtrl.HideBanner(adName);
	}

    public void ShowBanner(string adName){
		_adsCtrl.ShowBanner(adName);
	}

	public void LoadInterstitialAd(string adName){
		_adsCtrl.LoadInterstitialAd(adName);
	}

    public void ShowInterstitialAd(string adName, Action<bool> onClosed){
#if UNITY_EDITOR
            if(onClosed!=null)
                onClosed(true);
#else
    		_adsCtrl.ShowInterstitialAd(adName, onClosed);    
#endif
	}

	public void LoadRewardVideoAd(string adName){
		_adsCtrl.LoadRewardVideoAd(adName);
	}

    public void UnLoadRewardVideoAd(string adName){
		_adsCtrl.UnLoadRewardVideoAd(adName);
	}

    public void ShowRewardVideoAd(string adName, Action<Reward> rewardCallback, Action<bool> onClosed){

#if UNITY_EDITOR
            if(rewardCallback!=null){
                Reward reward = new Reward();
                reward.Type = "UNITY_EDITOR";
                reward.Amount = 100;
                rewardCallback(reward);
            }

            if(onClosed!=null)
                onClosed(true);
#else
    		_adsCtrl.ShowRewardVideoAd(adName,rewardCallback, onClosed);

#endif

	}
#endregion

}
