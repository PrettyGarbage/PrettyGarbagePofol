using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class BannerAdsObject : MonoBehaviour
{
    private BannerView _bannerView;
	private AdSize _adSize = AdSize.Banner;
	private AdPosition _adPosition = AdPosition.Top;

	public void SetAd(AdSize adSize, AdPosition adPosition){
		_adSize = adSize;
		_adPosition = adPosition;
	}

    public void RequestAd(string adName, string adUnitId, AdRequest adRequest,Action<bool> callback)
    {
        _bannerView = new BannerView(adUnitId, _adSize, _adPosition);
        // Called when an ad request has successfully loaded.
        _bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        _bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        _bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        _bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        _bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        _bannerView.LoadAd(adRequest);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("gbros HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("gbros HandleAdLeavingApplication event received");
    }

    public void DestroyAd()
    {
        if(_bannerView!=null)
			_bannerView.Destroy();
    }

    public void HideAd(){
        if(_bannerView!=null)
            _bannerView.Hide();
    }

    public void ShowAd(){
        if(_bannerView!=null)
            _bannerView.Show();
    }

    void OnDestroy(){
        DestroyAd();
    }
}
