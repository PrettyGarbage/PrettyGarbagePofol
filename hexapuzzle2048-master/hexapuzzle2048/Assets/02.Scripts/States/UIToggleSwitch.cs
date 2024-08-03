using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleSwitch : MonoBehaviour {

	[SerializeField]
	private Image _toggleSwich;

	public void ToggleSoundSwich(bool isOn){
		_toggleSwich.color = isOn ? Color.green : Color.gray;
		_toggleSwich.transform.localEulerAngles = isOn ? Vector3.zero : Vector3.up*180f;
	}
}
