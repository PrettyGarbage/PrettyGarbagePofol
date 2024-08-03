using System;
using System.Collections.Generic;
using System.Linq;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;

public class ADXGDPR : MonoBehaviour 
{
	public static ADXGDPR Instance { get; private set; }

	public static event Action<string> OnADXConsentCompleted;


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

	// Will return a non-null array of strings with at least 'min' non-null string values at the front.
	private string[] DecodeArgs(string argsJson, int min)
	{
		bool err = false;
		var args = Json.Deserialize(argsJson) as List<object>;
		if (args == null) {
			Debug.LogError("Invalid JSON data: " + argsJson);
			args = new List<object>();
			err = true;
		}
		if (args.Count < min) {
			if (!err)  // Don't double up the error messages for invalid JSON
				Debug.LogError("Missing one or more values: " + argsJson + " (expected " + min + ")");
			while (args.Count < min)
				args.Add("");
		}
		return args.Select(v => v.ToString()).ToArray();
	}

	public void EmitADXConsentCompletion(string argsJson)
	{
		var args = DecodeArgs(argsJson, min: 1);
		var completion = args[0];

		var evt = OnADXConsentCompleted;
		if (evt != null) evt(completion);
	}

	public static void ADXConsentCompletion(string state)
	{
		OnADXConsentCompleted (state);
	}
		
}