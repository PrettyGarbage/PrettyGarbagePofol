using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdxInterstitialRequest : MonoBehaviour {

	Action<bool> _onClosed;
	string _adUnitId = string.Empty;
	AdRequestStatus _requestStatus = AdRequestStatus.NONE;

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
		MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
		MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
		MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;
		MoPubManager.OnInterstitialShownEvent += OnInterstitialShownEvent;
		MoPubManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;		
	}

    void OnDisable()
	{
		MoPubManager.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
		MoPubManager.OnInterstitialFailedEvent -= OnInterstitialFailedEvent;
		MoPubManager.OnInterstitialDismissedEvent -= OnInterstitialDismissedEvent;
		MoPubManager.OnInterstitialExpiredEvent -= OnInterstitialExpiredEvent;
		MoPubManager.OnInterstitialShownEvent -= OnInterstitialShownEvent;
		MoPubManager.OnInterstitialClickedEvent -= OnInterstitialClickedEvent;
	}

    private void OnInterstitialClickedEvent(string obj)
    {
        Debug.LogFormat("OnInterstitialClickedEvent : {0}", obj);
    }

    private void OnInterstitialShownEvent(string obj)
    {
        Debug.LogFormat("OnInterstitialShownEvent : {0}", obj);
    }

    private void OnInterstitialExpiredEvent(string obj)
    {
		_requestStatus = AdRequestStatus.FAIL;
        Debug.LogFormat("OnInterstitialExpiredEvent : {0}, {1}", obj, _requestStatus);
    }

    private void OnInterstitialDismissedEvent(string obj)
    {
		_requestStatus = AdRequestStatus.CLOSED;
        Debug.LogFormat("OnInterstitialDismissedEvent : {0}, {1}", obj, _requestStatus);
    }

    private void OnInterstitialFailedEvent(string arg1, string arg2)
    {
		_requestStatus = AdRequestStatus.FAIL;
        Debug.LogFormat("OnInterstitialFailedEvent : {0} / {1} / {2}", arg1, arg2, _requestStatus);
    }

    private void OnInterstitialLoadedEvent(string obj)
    {
		_requestStatus = AdRequestStatus.LOADED;
        Debug.LogFormat("OnInterstitialLoadedEvent : {0}, {1}", obj, _requestStatus);
    }

	public void LoadInterstitialAd(string adUnitId){
		Debug.LogFormat("LoadInterstitialAd {0}, {1}", adUnitId, _requestStatus);
		if((!_adUnitId.Equals(string.Empty) && !adUnitId.Equals(_adUnitId)) || _requestStatus == AdRequestStatus.LOADED){
			Debug.LogWarningFormat("이미 로드 중인 광고가 있습니다. 로드된 광고 : {0} / 요청한 광고 : {1}", adUnitId, _adUnitId);
			return;
		}
		_requestStatus = AdRequestStatus.LOADING;
		_adUnitId = adUnitId;
		MoPub.RequestInterstitialAd (adUnitId);  		
	}

	public void ShowInterstitialAd(string adUnitId, Action<bool> onClosed){

		Debug.LogFormat("ShowInterstitialAd {0}, {1}", adUnitId, _requestStatus);
		_onClosed = onClosed;
		if(_requestStatus == AdRequestStatus.FAIL){
			Debug.LogWarning("직전 요청한 광고의 Load가 실패했습니다.");
			CallbackClose(false);
			return;
		}

		if(!adUnitId.Equals(_adUnitId)){
			Debug.LogWarningFormat("로드 중인 광고와 다릅니다. 로드된 광고 : {0} / 요청한 광고 : {1}", adUnitId, _adUnitId);
			CallbackClose(false);
			return;
		}


		StartCoroutine(ShowAdCor());
	}

	private IEnumerator ShowAdCor(){
		
		while(_requestStatus == AdRequestStatus.LOADING){
			yield return CommonConstants.WaitLoopSeconds;
		}

		if(_requestStatus == AdRequestStatus.FAIL){
			CallbackClose(false);
		}
		else {
			_requestStatus = AdRequestStatus.SHOW;
			MoPub.ShowInterstitialAd (_adUnitId);  

			while (_requestStatus == AdRequestStatus.SHOW)
			{
				yield return new WaitForEndOfFrame();
			}

			CallbackClose(_requestStatus == AdRequestStatus.CLOSED);
		}

	}

	private void CallbackClose(bool isCloseSuccess){
		if(_onClosed!=null){
            _onClosed(isCloseSuccess);
            _onClosed = null;
        }
		_adUnitId = string.Empty;
		_requestStatus = AdRequestStatus.NONE;
	}
}
