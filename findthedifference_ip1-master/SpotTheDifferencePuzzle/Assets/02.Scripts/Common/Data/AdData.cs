using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class admobTestUserInfo
{
    //a_184856526086757532
    [SerializeField]
    private string deviceName;
    [SerializeField]
    private string deviceId;

    public string DeviceName
    {
        get
        {
            return deviceName;
        }
    }

    public string DeviceId
    {
        get
        {
            return deviceId;
        }
    }
}

[System.Serializable]
public class  AdmobAdUnitIdInfo
{
    [SerializeField]
    private string adName;
    [SerializeField]
    private string adUnitId;
    [SerializeField]
    private string adUnitIdForTest;

    public string AdName { get { return adName; } }
    public string AdUnitId { get { return adUnitId; } }
    public string AdUnitIdForTest { get { return adUnitIdForTest; } }
}

[CreateAssetMenu(fileName = "AdData.asset", menuName = "gbros/AdData", order = 1)]
public class AdData : ScriptableObject{

    const string TEST_UNIT_ID_BANNER = "ca-app-pub-3940256099942544/6300978111";
	const string TEST_UNIT_ID_INTERSTITIAL = "ca-app-pub-3940256099942544/1033173712";
	const string TEST_UNIT_ID_INTERSTITIAL_VIDEO = "ca-app-pub-3940256099942544/8691691433";
	const string TEST_UNIT_ID_REWARD_VIDEO = "ca-app-pub-3940256099942544/5224354917";

    [Header("App")]
    [SerializeField]
	private string _appId;
    [Header("Ad")]
    [SerializeField]
    private AdmobAdUnitIdInfo[] _bannerAdInfo;
    [SerializeField]
    private AdmobAdUnitIdInfo[] _interstitialAdInfo;
    [SerializeField]
    private AdmobAdUnitIdInfo[] _interstitialVideoAdInfo;
    [SerializeField]
    private AdmobAdUnitIdInfo[] _rewardAdInfo;

    [Header("Test")]
    [SerializeField]
    private admobTestUserInfo[] _testUserInfos;
    
    public string AppId
    {
        get
        {
            return _appId;
        }
    }

    public AdmobAdUnitIdInfo[] BannerAdInfo
    {
        get
        {
            return _bannerAdInfo;
        }
    }

    public AdmobAdUnitIdInfo[] InterstitialAdInfo
    {
        get
        {
            return _interstitialAdInfo;
        }
    }

    public AdmobAdUnitIdInfo[] InterstitialVideoAdInfo
    {
        get
        {
            return _interstitialVideoAdInfo;
        }
    }

    public AdmobAdUnitIdInfo[] RewardAdInfo
    {
        get
        {
            return _rewardAdInfo;
        }
    }
    
    public admobTestUserInfo[] TestUserInfos
    {
        get
        {
            return _testUserInfos;
        }
    }

    public string GetAdUnitId(AdType admobAdType, string adName){
		
        AdmobAdUnitIdInfo[] admobAdUnitIdInfos = null;
		switch (admobAdType)
		{
            case AdType.BANNER:
				
				admobAdUnitIdInfos = BannerAdInfo;
				break;
            case AdType.INTERSTITIAL:
				if(BuildManager.Instance.IsDevBuild())
					return TEST_UNIT_ID_INTERSTITIAL;

				admobAdUnitIdInfos = InterstitialAdInfo;
                break;
            case AdType.INTERSTITIAL_VIDEO:
				if(BuildManager.Instance.IsDevBuild())
					return TEST_UNIT_ID_INTERSTITIAL_VIDEO;

				admobAdUnitIdInfos = InterstitialVideoAdInfo;
                break;
            case AdType.REWARD:
				admobAdUnitIdInfos = RewardAdInfo;
                break;
		}
		AdmobAdUnitIdInfo admobAdUnitIdInfo = Array.Find(admobAdUnitIdInfos, (adUnitIdInfo) =>{ return adUnitIdInfo.AdName.Trim().Equals(adName);});
        string adUnit = admobAdUnitIdInfo == null ? string.Empty : (BuildManager.Instance.IsDevBuild() ? admobAdUnitIdInfo.AdUnitIdForTest : admobAdUnitIdInfo.AdUnitId);

        return adUnit.Trim();
	}
}
