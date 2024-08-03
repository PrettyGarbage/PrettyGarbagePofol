using System;
using UnityEngine;
using ADXBD = ADXBinding;

public class ADXGDPRManager : MonoBehaviour
{
	public static ADXGDPRManager Instance { get; private set; }

	private static ADXBD adxbd;

	static ADXGDPRManager()
	{
		if (adxbd == null) {
			adxbd = new ADXBD();
		}
	}

	public static void InitializeWithShowADXConsent(string adUnitId, int debugState) {
		adxbd.InitializeWithShowADXConsent (adUnitId, debugState);
	}

	public static void InitializeWithSetConsentState(string adUnitId, int state) {
		adxbd.InitializeWithSetConsentState (adUnitId, state);
	}

	public static void SetDebugState(int state)
	{
		adxbd.SetDebugState (state);
	}

	public static int GetConsentState()
	{
		return adxbd.GetConsentState ();
	}

	public static void SetConsentState(int state)
	{
		adxbd.SetConsentState (state);
	}

	public static string GetPrivacyPolicyURL()
	{
		return adxbd.GetPrivacyPolicyURL ();
	}
}