using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class NetworkManager : MonoBehaviour {

    private static NetworkManager _instance;

    private GoogleSignInCtrl _googleSignInCtrl;

    private GoogleMobileAdsCtrl _adsCtrl;

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
#elif UNITY_IOS
        AdData adData = _adDataForIos;
#endif
        _adsCtrl = gameObject.AddComponent<GoogleMobileAdsCtrl>();
        _adsCtrl.Init(adData);

        yield return null;
    }

    /// <summary>
    /// https://github.com/yasirkula/UnityNativeShare
    /// Apps the share.
    /// </summary>
    public void AppShare(){
        string text = LocalizationManager.Instance.GetText(LocalizationTextKey.COMMON_BUTTON_SHARE_DESC);
#if UNITY_ANDROID
        text = text + CommonConstants.SHARE_APPLINK_ANDROID + CommonConstants.ANDROID_PACKAGE_NAME;
#elif UNITY_IOS
        text = text + CommonConstants.SHARE_APPLINK_IOS;
#endif
        new NativeShare()
            .SetTitle(LocalizationManager.Instance.GetText(LocalizationTextKey.INTRO_TITLE_MAIN))
            .SetText(text)
            .SetSubject("")
            .Share();           
    }

    #region Ads

    public void PreLoadRewardAd()
    {
        //_adsCtrl.PreLoadRewardAd();
    }

    public string GetADUnitId(AdType admobAdType, string adName)
    {
#if UNITY_ANDROID
        AdData adData = _adDataForAndroid;
#elif UNITY_IOS
        AdData adData = _adDataForIos;        
#endif
        return adData.GetAdUnitId(admobAdType, adName);
    }

    public void RequestBanner(string adName, float witdh, float heigth, AdPosition adPosition)
    {
        int xdp = (int)(witdh / (Screen.dpi / 160));
        int ydp = (int)(heigth / (Screen.dpi / 160));
        Debug.Log("RequestBanner xdp : " + xdp + "/ ydp" + ydp);
        _adsCtrl.RequestBanner(adName, new AdSize(xdp, ydp), adPosition);
    }

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

    public void ShowRewardVideoAd(string adName, Action<Reward> rewardCallback, Action<bool> onClosed){

        StateManager.Instance.ActiveBlockScreen(true);
        StateManager.Instance.ShowSpinner();

        bool _isRewarded = false;

#if UNITY_EDITOR

        //rewardCallback(new Reward());
        ShowRewardAdEndProcess(onClosed, false);

#else
        _adsCtrl.ShowRewardVideoAd(adName
            
            , reward => {
                _isRewarded = true;
                rewardCallback(reward);
            }
            , isSuccess=> {

                if (isSuccess)
                {
                    if (_isRewarded)
                    {
                        ApiManager.Instance.GetUserInfo(apiresult =>
                        {
                            ShowRewardAdEndProcess(onClosed, isSuccess);
                        });
                    }
                    else
                    {
                        ShowRewardAdEndProcess(onClosed, isSuccess);
                    }
                }
                else
                {
                    MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.NOTICE_AD_EMPTY);
                    ShowRewardAdEndProcess(onClosed, isSuccess);
                }
        });

#endif

    }

    private void ShowRewardAdEndProcess(Action<bool> onClosed, bool isSuccess)
    {
        onClosed(isSuccess);
        StateManager.Instance.ActiveBlockScreen(false);
        StateManager.Instance.HideSpinner();
    }

    #endregion

    #region Login
    public void SignInPlatform(LoginType loginType, Action<AuthLoginForm> callback)
    {
        AuthLoginForm authLoginForm;
        switch (loginType)
        {
            case LoginType.NONE:
                authLoginForm = new AuthLoginForm();
                authLoginForm.id = System.Guid.NewGuid().ToString().Substring(0, 19);
                authLoginForm.name = SystemInfo.deviceUniqueIdentifier.Substring(0, 19);
                authLoginForm.authData = string.Empty;
                authLoginForm.loginType = LoginType.NONE;
                authLoginForm.locale = Application.systemLanguage.ToString();
                callback(authLoginForm);
                break;
            case LoginType.GOOGLE:
                if(_googleSignInCtrl == null)
                {
                    _googleSignInCtrl = new GoogleSignInCtrl();
                }

                _googleSignInCtrl.SignIn(googleUser=>{
                    if (googleUser != null)
                    {
                        authLoginForm = new AuthLoginForm();
                        authLoginForm.id = googleUser.UserId;
                        authLoginForm.name = googleUser.DisplayName;
                        authLoginForm.authData = googleUser.IdToken;
                        authLoginForm.loginType = LoginType.GOOGLE;
                        authLoginForm.locale = Application.systemLanguage.ToString();
                        callback(authLoginForm);
                    }
                    else
                    {
                        callback(null);
                    }
                });

                break;
            case LoginType.FACEBOOK:
            case LoginType.TWITTER:
            default:
                callback(null);
                break;
        }
    }

    public void SignInToServer(AuthLoginForm authLoginForm, Action<bool> callback)
    {
        ApiManager.Instance.AuthLogin(authLoginForm, (apiResult) =>
        {
            if (apiResult.isSuccess)
            {
                callback(true);
            }
            else
            {
                StateManager.Instance.ActiveBlockScreen(false);
                MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_FAIL_ALERT, () =>
                {
                    callback(false);
                });
            }
        });
    }

    public void SignOut()
    {
        ApiManager.Instance.RemoveApiToken();
        GameManager.Instance.Restart();
    } 
    #endregion

}
