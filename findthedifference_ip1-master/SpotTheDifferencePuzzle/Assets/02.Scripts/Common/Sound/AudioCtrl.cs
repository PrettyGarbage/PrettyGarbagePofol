using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AudioCtrl : BaseObject
{

    private List<AudioSource> _audioSourceList;
    private List<AudioType> _audioTypeList;

    public List<AudioSource> AudioSourceList
    {
        get
        {
            if (_audioSourceList == null) _audioSourceList = new List<AudioSource>();
            return _audioSourceList;
        }
    }

    public List<AudioType> AudioTypeList
    {
        get
        {
            if (_audioTypeList == null) _audioTypeList = new List<AudioType>();
            return _audioTypeList;
        }
    }

    public static void Play(AudioSource audioSource, AudioType audioType)
    {

        if (!audioSource.isActiveAndEnabled) return;

        AudioCtrl audioCtrl = audioSource.gameObject.GetComponent<AudioCtrl>();
        if (audioCtrl == null)
        {
            audioCtrl = audioSource.gameObject.AddComponent<AudioCtrl>();
        }

        audioCtrl.PlaySetting(audioSource, audioType);
        audioSource.Play();
        audioSource.mute = SoundManager.Instance.GetIsMute(audioType);
    }

    public void PlaySetting(AudioSource audioSource, AudioType audioType)
    {
        int findIndex = AudioSourceList.BinarySearch(audioSource, new AudioSourceComparer());
        if (findIndex < 0)
        {
            AudioSourceList.Add(audioSource);
            AudioTypeList.Add(audioType);
        }
        else
        {
            AudioTypeList[findIndex] = audioType;
        }
        EventPool.Listen(GetInstanceID(), EventNames.ON_AUDIO_VOLUME_CTRL, OnAudioMute);
    }

    public class AudioSourceComparer : IComparer<AudioSource>
    {
        public int Compare(AudioSource x, AudioSource y) { return (x.GetInstanceID() == y.GetInstanceID() ? 1 : -1); }
    }

    private SoundFadePlayer _soundFadePlayer;

    private void OnAudioMute(object[] args)
    {
        AudioType audioType = (AudioType)args[0];
        float volume = (float)args[1];

        _soundFadePlayer = GetComponent<SoundFadePlayer>();
        if (_soundFadePlayer && audioType == AudioType.BGM)
        {
            _soundFadePlayer.volumeRate = volume * _soundFadePlayer.curVolumeRate;
            if (_soundFadePlayer.AudioSource)
            {
                _soundFadePlayer.AudioSource.mute = volume == 0f;
                _soundFadePlayer.AudioSource.volume = _soundFadePlayer.volumeRate;
            }

        }
        else
        {
            for (int i = 0; i < _audioTypeList.Count; i++)
            {
                if (_audioTypeList[i] == audioType)
                {
                    _audioSourceList[i].mute = volume == 0f;
                    _audioSourceList[i].volume = volume;
                }
            }
        }
    }

    public override void Dispose()
    {
        if (_audioSourceList != null)
        {
            AudioSourceList.Clear();
            AudioTypeList.Clear();
        }
        EventPool.Remove(GetInstanceID(), EventNames.ON_AUDIO_VOLUME_CTRL, OnAudioMute);
    }
}