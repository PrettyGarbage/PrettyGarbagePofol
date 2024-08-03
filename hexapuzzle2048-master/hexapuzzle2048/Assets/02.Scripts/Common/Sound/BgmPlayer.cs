using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour {

	private Dictionary<string,SoundFadePlayer> _soundFadePlayerDic = new Dictionary<string, SoundFadePlayer>();

	public void Play(string key, float fadeInTime = 1f, float fadeOutTime = 1f){
		SoundFadePlayer soundFadePlayer;
		if(_soundFadePlayerDic.ContainsKey(key)){
			soundFadePlayer = _soundFadePlayerDic[key];
		}
		else{
			GameObject go = new GameObject("BGM_"+ key);
			go.AddComponent<AudioSource>();
			soundFadePlayer = go.AddComponent<SoundFadePlayer>();
			_soundFadePlayerDic.Add(key, soundFadePlayer);
		}
		soundFadePlayer.fadeInTime = fadeInTime;
		soundFadePlayer.fadeOutTime = fadeOutTime;
		soundFadePlayer.enabled = true;
		soundFadePlayer.Play(key);
	}

	public void Stop(string key){
		SoundFadePlayer soundFadePlayer;
		if(_soundFadePlayerDic.ContainsKey(key)){
			soundFadePlayer = _soundFadePlayerDic[key];
			soundFadePlayer.Stop();
		}
	}
	
}
