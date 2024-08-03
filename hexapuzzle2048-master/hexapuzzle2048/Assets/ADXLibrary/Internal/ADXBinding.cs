#define ENABLE_CLOSE_AD

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;

public class ADXBinding
{
	#if UNITY_ANDROID
	private static readonly AndroidJavaClass PluginClass = new AndroidJavaClass("com.adxcorp.unity.ADXUnityPlugin");
	#if ENABLE_CLOSE_AD
	private static readonly AndroidJavaClass CloseAdPluginClass = new AndroidJavaClass("com.adxcorp.unity.ADXUnityPluginCloseAd");
	#endif
	#elif UNITY_IOS
	private static bool isShowADXConsent = false;
	private static int gdprState = 0;
	#endif

	private static string ADX_UNITY_PLUGIN_VERSION = "1.4.5";

	protected static void InitManager()
	{
		var type = typeof(ADXGDPR);
		var mgr = new GameObject("ADXGDPR", type).GetComponent<ADXGDPR>();

		var type2 = typeof(ADXCloseAd);
		var mgr2 = new GameObject("ADXCloseAd", type2).GetComponent<ADXCloseAd>();

		Debug.Log ("ADXUnityPlugin version: " + ADX_UNITY_PLUGIN_VERSION);
	}

	public void InitializeWithShowADXConsent(string adUnitId, int debugState) {
		InitManager ();
		#if UNITY_ANDROID
		PluginClass.CallStatic("initWithShowAdxConsent", adUnitId, debugState);
		#elif UNITY_IOS
		isShowADXConsent = true;
		SetDebugState(debugState);

		MoPubManager.OnSdkInitializedEvent += OnSdkInitalizedEvent;
		MoPub.InitializeSdk(adUnitId);
		#endif
	}

	public void InitializeWithSetConsentState(string adUnitId, int state) {
		InitManager ();
		#if UNITY_ANDROID
		PluginClass.CallStatic("initWithSaveGDPRState", adUnitId, state);
		#elif UNITY_IOS
		isShowADXConsent = false;
		gdprState = state;

		MoPubManager.OnSdkInitializedEvent += OnSdkInitalizedEvent;
		MoPub.InitializeSdk(adUnitId);
		#endif

	}

	private void OnSdkInitalizedEvent(string s) {
		#if UNITY_IOS
		if (isShowADXConsent) {
		_showADXConsent();
		} else {
		SetConsentState(gdprState);
		ADXGDPR.ADXConsentCompletion(gdprState.ToString());
		}
		#endif
	}

	public void SetDebugState(int state)
	{
		#if UNITY_ANDROID
		PluginClass.CallStatic("setDebugState", state);
		#elif UNITY_IOS
		_setDebugState(state);
		#endif
	}

	public int GetConsentState()
	{
		#if UNITY_ANDROID
		return PluginClass.CallStatic<int>("getConsentState");
		#elif UNITY_IOS
		return _getConsentState();
		#else
		return 1;
		#endif
	}

	public void SetConsentState(int state)
	{
		#if UNITY_ANDROID
		PluginClass.CallStatic("setConsentState", state);
		#elif UNITY_IOS
		_setConsentState(state);
		#endif
	}

	public string GetPrivacyPolicyURL()
	{
		#if UNITY_ANDROID
		return PluginClass.CallStatic<string>("getPrivacyURL");
		#elif UNITY_IOS
		return _getPrivacyPolicyURL();
		#else
		return "https://assets.adxcorp.kr/privacy/partners";
		#endif
	}

	// supprot for Close Ad 
	public void InitializeCloseAdFactory(string adUnitId, string exitMessage) {
		#if UNITY_ANDROID
		#if ENABLE_CLOSE_AD
		CloseAdPluginClass.CallStatic("initCloseAdFactory", adUnitId, exitMessage);
		#endif
		#endif
	}

	public void PreloadCloseAd() {
		#if UNITY_ANDROID
		#if ENABLE_CLOSE_AD
		CloseAdPluginClass.CallStatic("preloadCloseAd");
		#endif
		#endif
	}

	public void ShowCloseAd() {
		#if UNITY_ANDROID
		#if ENABLE_CLOSE_AD
		CloseAdPluginClass.CallStatic("showCloseAd");
		#endif
		#endif
	}

	#if UNITY_IOS
	#region DllImports

	[DllImport("__Internal")]
	private static extern void _showADXConsent();

	[DllImport("__Internal")]
	private static extern void _setDebugState(int state);

	[DllImport("__Internal")]
	private static extern int _getConsentState();

	[DllImport("__Internal")]
	private static extern void _setConsentState(int state);

	[DllImport("__Internal")]
	private static extern string _getPrivacyPolicyURL();

	#endregion
	#endif
}



