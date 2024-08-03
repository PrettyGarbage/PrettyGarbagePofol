using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoicePlayer : MonoBehaviour {

    [SerializeField]
    private VoiceData _voiceData;

    [SerializeField]
    private float _fadeTime = 0.2f;

    [SerializeField]
    private AudioSource _audioSource;

    private AudioClipInfo _audioClipInfo;

    private Action<bool> _voiceCallback;

    Coroutine playCoroutine;

    private Dictionary<string, int> _playCountDic = new Dictionary<string, int>();

    public void Play(string key, Action<bool> voiceCallback = null)
    {
        AudioClip audioClip = GetAudioClip(key);
        if(playCoroutine!=null)
            StopCoroutine(playCoroutine);
        if (audioClip != null)
        {
            if (_audioSource.isPlaying)
            {
                StopFade(() => {                    
                    playCoroutine = StartCoroutine(PlayVoice(audioClip, voiceCallback));
                });
            }
            else
            {
                playCoroutine = StartCoroutine(PlayVoice(audioClip, voiceCallback));
            }            
        }        
    }

    private IEnumerator PlayVoice(AudioClip audioClip, Action<bool> voiceCallback = null)
    {
        _voiceCallback = voiceCallback;
        _audioClipInfo = new AudioClipInfo(audioClip, 1f, AudioType.VOICE, false);
        SoundManager.Instance.InstanceVolume(0.2f, 0.2f);
        SoundManager.Instance.PlayVoiceSound(_audioSource, _audioClipInfo);

        while (_audioSource.isPlaying)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
        SoundManager.Instance.InstanceVolume(1f, 0.2f);
        PlayCallback(true);
    }

    private AudioClip GetAudioClip(string key)
    {

        VoiceInfo voiceInfo = _voiceData.VoiceInfoList.Find(vd => vd.Key.Equals(key));
        if (voiceInfo != null)
        {
            int audioIndex = 0;
            if (_playCountDic.ContainsKey(key))
            {  
                audioIndex = _playCountDic[key]++;

                if(audioIndex == voiceInfo.AudioClips.Length)
                {
                    audioIndex = 0;
                    voiceInfo.AudioClips.Shuffle();
                    _playCountDic[key] = 1;
                }
            }
            else
            {
                _playCountDic.Add(key, 1);
                audioIndex = 0;
                voiceInfo.AudioClips.Shuffle();
            }
            return voiceInfo.AudioClips[audioIndex];
        }
        else
        {
            Debug.Log("NOT FOUND AudioClip : " + key);
            return null;
        }
    }

    private void StopFade(System.Action callback)
    {        
        LeanTween.value(_audioSource.volume, 0, _fadeTime).setOnUpdate(v => {
            _audioSource.volume = v;
        }).setOnComplete(() => {

            PlayCallback(false);

            if (callback != null)
            {
                callback();
            }
        });
    }

    private void PlayCallback(bool isEndAfterPlay)
    {
        if (_voiceCallback != null)
        {
            _voiceCallback(isEndAfterPlay);
        }
    }
}
