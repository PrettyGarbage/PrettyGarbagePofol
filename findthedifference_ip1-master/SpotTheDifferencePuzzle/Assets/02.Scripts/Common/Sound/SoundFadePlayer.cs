using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(AudioSource))]
public class SoundFadePlayer : MonoBehaviour
{

    [SerializeField]
    private AudioDataKey _audioKey;
    private AudioClipInfo _audioClipInfo;

    private AudioSource _audioSource;

    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float volumeRate = 1f;
    [HideInInspector]
    public float curVolumeRate = 1f;

    [SerializeField]
    private bool isFadeIn = false;
    [SerializeField]
    private bool isFadeOut = false;

    private float fadeValue;

    public AudioSource AudioSource
    {
        get
        {
            return _audioSource;
        }
    }

    public bool IsFadeOut
    {
        get
        {
            return isFadeOut;
        }
    }

    private void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioClipInfo = SoundManager.Instance.GetAudioClipInfo(_audioKey);
        _audioSource.volume = 0f;
        _audioSource.playOnAwake = false;
        volumeRate = _audioClipInfo.VolumeRate;
        curVolumeRate = volumeRate;
    }

    private void Update()
    {
        if (_audioSource && _audioSource.isPlaying)
        {
            if (isFadeIn && _audioSource.volume <= volumeRate)
            {
                _audioSource.volume += (Time.deltaTime * fadeValue);
            }

            if (isFadeOut && _audioSource.volume > 0f)
            {
                _audioSource.volume -= (Time.deltaTime * fadeValue);

                if (_audioSource.volume <= 0f)
                {
                    _audioSource.Stop();
                    isFadeOut = false;
                }
            }
        }
    }

    public void Play(AudioDataKey key){
        _audioKey = key;
        Play();
    }

    public void Play()
    {
        if(_audioSource == null){
            Init();
        }

        if (_audioSource.isPlaying)
        {
            return;
        }

        isFadeIn = true;
        isFadeOut = false;

        volumeRate = curVolumeRate * SoundManager.Instance.GetVolume(AudioType.BGM);

        fadeValue = volumeRate / 1f;
        SoundManager.Instance.PlaySound(_audioSource, _audioKey, false);
        _audioSource.volume = 0;
    }

    public void Stop()
    {
        if(_audioSource.mute){
            _audioSource.Stop();
        }
        else {
            isFadeIn = false;
            isFadeOut = true;
            fadeValue = SoundManager.Instance.GetVolume(AudioType.BGM) / 1f;
        }        
    }

}