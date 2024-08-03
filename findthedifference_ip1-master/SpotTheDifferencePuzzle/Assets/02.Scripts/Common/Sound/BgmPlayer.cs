using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour {

	private Dictionary<AudioDataKey, SoundFadePlayer> _soundFadePlayerDic = new Dictionary<AudioDataKey, SoundFadePlayer>();

	public void Play(AudioDataKey audioDataKey, float fadeInTime = 1f, float fadeOutTime = 1f){
		SoundFadePlayer soundFadePlayer;
		if(_soundFadePlayerDic.ContainsKey(audioDataKey)){
			soundFadePlayer = _soundFadePlayerDic[audioDataKey];
		}
		else{
			GameObject go = new GameObject("BGM_"+ audioDataKey);
            go.transform.SetParent(transform);
            go.AddComponent<AudioSource>();
			soundFadePlayer = go.AddComponent<SoundFadePlayer>();
			_soundFadePlayerDic.Add(audioDataKey, soundFadePlayer);
		}
		soundFadePlayer.fadeInTime = fadeInTime;
		soundFadePlayer.fadeOutTime = fadeOutTime;
		soundFadePlayer.enabled = true;
		soundFadePlayer.Play(audioDataKey);
	}

	public void Stop(AudioDataKey audioDataKey)
    {
		SoundFadePlayer soundFadePlayer;
		if(_soundFadePlayerDic.ContainsKey(audioDataKey)){
			soundFadePlayer = _soundFadePlayerDic[audioDataKey];
			soundFadePlayer.Stop();
		}
	}

    public void StopAll()
    {
        foreach (SoundFadePlayer soundFadePlayer in _soundFadePlayerDic.Values)
        {
            soundFadePlayer.Stop();
        }
    }
}
