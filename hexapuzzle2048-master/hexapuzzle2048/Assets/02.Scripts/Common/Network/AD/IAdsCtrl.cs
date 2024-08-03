using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public interface IAdsCtrl
{
	void Init (AdData adData);
	void RequestBanner(string adName, AdSize adSize, AdPosition adPosition);
	void RemoveBanner(string adName);
	void HideBanner(string adName);
	void ShowBanner(string adName);
	void LoadInterstitialAd(string adName);
	void ShowInterstitialAd(string adName, Action<bool> onClosed);
	void LoadRewardVideoAd(string adName);
	void UnLoadRewardVideoAd(string adName);
	void ShowRewardVideoAd(string adName, Action<Reward> _rewardCallback, Action<bool> callback);
}