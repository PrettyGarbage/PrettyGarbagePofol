using System;
using System.Collections.Generic;
using System.Linq;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;

public class ADXCloseAd : MonoBehaviour 
{
	public static ADXCloseAd Instance { get; private set; }

	public static event Action OnADXCloseAdOk;
	public static event Action OnADXCloseAdCancel;

	private void Awake()
	{
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else
			Destroy(this);
	}
		
	private void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	public void EmitADXCloseAdOk()
	{
		OnADXCloseAdOk ();
	}

	public void EmitADXCloseAdCancel()
	{
		OnADXCloseAdCancel ();
	}
}