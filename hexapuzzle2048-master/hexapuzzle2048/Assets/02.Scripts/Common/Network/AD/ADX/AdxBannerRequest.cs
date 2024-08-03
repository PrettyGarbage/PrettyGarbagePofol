using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdxBannerRequest : MonoBehaviour {

 	string _adUnitId;

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
		MoPubManager.OnAdFailedEvent += OnAdFailedEvent;
		MoPubManager.OnAdClickedEvent += OnAdClickedEvent;
		MoPubManager.OnAdExpandedEvent += OnAdExpandedEvent;
		MoPubManager.OnAdCollapsedEvent += OnAdCollapsedEvent;
	}

    void OnDisable()
	{
		MoPubManager.OnAdLoadedEvent -= OnAdLoadedEvent;
		MoPubManager.OnAdFailedEvent -= OnAdFailedEvent;
		MoPubManager.OnAdClickedEvent -= OnAdClickedEvent;
		MoPubManager.OnAdExpandedEvent -= OnAdExpandedEvent;
		MoPubManager.OnAdCollapsedEvent -= OnAdCollapsedEvent;
	}

	public void RequestBanner(string adUnitId, MoPubBase.AdPosition adPosition){
		Debug.LogFormat("RequestBanner {0}", adUnitId);
		_adUnitId = adUnitId;
		MoPub.CreateBanner (adUnitId, adPosition);		
	}

	public void RemoveBanner(){
		MoPub.DestroyBanner(_adUnitId);		
	}

	public void HideBanner(){
		MoPub.ShowBanner(_adUnitId, false);
	}

	public void ShowBanner(){
		MoPub.ShowBanner(_adUnitId, true);
	}

	private void OnAdCollapsedEvent(string obj)
    {
        Debug.LogFormat("OnAdCollapsedEvent : {0}", obj);
    }

    private void OnAdExpandedEvent(string obj)
    {
        Debug.LogFormat("OnAdExpandedEvent : {0}", obj);
    }

    private void OnAdClickedEvent(string obj)
    {
        Debug.LogFormat("OnAdClickedEvent : {0}", obj);
    }

    private void OnAdFailedEvent(string arg1, string arg2)
    {
        Debug.LogFormat("OnAdFailedEvent : {0} / {1}", arg1, arg2);
    }

    private void OnAdLoadedEvent(string arg1, float arg2)
    {
        Debug.LogFormat("OnAdLoadedEvent : {0} / {1}", arg1, arg2);
    }

	
}
