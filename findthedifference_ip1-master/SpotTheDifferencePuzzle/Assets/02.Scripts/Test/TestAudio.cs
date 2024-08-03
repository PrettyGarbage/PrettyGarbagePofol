using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAudio : MonoBehaviour {

	public Text _text;
    public AudioDataKey dataKey;

	// Use this for initialization
	void Start () {
		
	}
	
	public void PlayBgm(){
		SoundManager.Instance.PlayBgm(AudioDataKey.BGM_lobby);
	}

	public void StopBgm(){
		SoundManager.Instance.StopBgm(AudioDataKey.BGM_lobby);
	}

	public void PlayFx(){
		SoundManager.Instance.PlayUISoundInstance(dataKey);
	}
}
