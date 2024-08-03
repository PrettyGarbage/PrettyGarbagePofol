using System;
using UnityEngine;
using ADXBD = ADXBinding;

public class ADXCloseAdManager : MonoBehaviour
{
	public static ADXCloseAdManager Instance { get; private set; }

	private static ADXBD adxbd;

	static ADXCloseAdManager()
	{
		if (adxbd == null) {
			adxbd = new ADXBD();
		}
	}

	public static void InitializeCloseAdFactory(string adUnitId, string exitMessage) {
		adxbd.InitializeCloseAdFactory (adUnitId, exitMessage);
	}

	public static void PreloadCloseAd() {
		adxbd.PreloadCloseAd ();
	}

	public static void ShowCloseAd() {
		adxbd.ShowCloseAd ();
	}
}