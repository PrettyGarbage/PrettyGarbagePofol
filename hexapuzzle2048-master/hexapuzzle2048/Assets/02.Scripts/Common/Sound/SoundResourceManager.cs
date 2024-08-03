using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AudioClipInfo
{
    [SerializeField]
    private bool _isLoop = false;
    [SerializeField]
    private float _volumeRate = 1f;
    [SerializeField]
    private AudioClip _clip;
    [SerializeField]
    private AudioType _type = AudioType.FX;

    public AudioClipInfo(AudioClip clip, float volumeRate, AudioType audioType, bool isLoop){
        _clip = clip;
        _volumeRate = volumeRate;
        _type = audioType;
        _isLoop = isLoop;
    }

    public float length
    {
        get
        {
            return (Clip != null) ? Clip.length : 0.0f;
        }
    }

    public bool IsLoop
    {
        get
        {
            return _isLoop;
        }
    }

    public float VolumeRate
    {
        get
        {
            return _volumeRate;
        }
    }

    public AudioClip Clip
    {
        get
        {
            return _clip;
        }
    }

    public AudioType Type
    {
        get
        {
            return _type;
        }
    }
}


public class SoundResourceManager : MonoBehaviour
{
    private static SoundResourceManager _instance;
    public static SoundResourceManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);
    }

}