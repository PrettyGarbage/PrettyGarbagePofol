using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class SoundManager : MonoBehaviour
{

    const string AUDIO_DATA_PATH = "Data/AudioDataBase";

    private static SoundManager _instance;

    [SerializeField]
    private AudioDataBase _audioDataBase;

    [SerializeField]
    private VoicePlayer _voicePlayer;

    //public bool isMute = false;
    public float spatialBlend = 0.95f;
    public int priority3D = 128;
    public int priority2D = 10;
    public int priorityBGM = 5;
    public float minDistance = 2f;
    public float maxDistance = 40f;

    private float[] _isVolumeAarry = new float[3]{ 1f, 1f, 1f };

    //BGM
    private BgmPlayer _bgmPlayer;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<SoundManager>("SoundManager");
            }

            return _instance;
        }
    }

    public IEnumerator Init(){
        _bgmPlayer = gameObject.AddComponent<BgmPlayer>();
        yield return null;
    }

    public bool GetIsMute(AudioType audioType)
    {
        return _isVolumeAarry[(int)audioType] == 0f;
    }

    public void SetVolume(AudioType audioType, float volume)
    {
        if (volume == 0)
        {
            LeanTween.cancel(gameObject);
        }
        _isVolumeAarry[(int)audioType] = volume;
        EventPool.Send(EventNames.ON_AUDIO_VOLUME_CTRL, audioType, volume);
    }

    public float GetVolume(AudioType audioType)
    {
        return _isVolumeAarry[(int)audioType];
    }

    public void InstanceVolume(float volume, float time)
    {
        float bgmVol = SoundManager.Instance.GetVolume(AudioType.BGM);
        float sfxVol = SoundManager.Instance.GetVolume(AudioType.FX);
        if(bgmVol>0)
        {
            gameObject.LeanValue(bgmVol, volume, time).setOnUpdate((float value) => {
                SetVolume(AudioType.BGM, value);
            });
        }
        if (sfxVol > 0)
        {
            gameObject.LeanValue(sfxVol, volume, time).setOnUpdate((float value) => {
                SetVolume(AudioType.FX, value);
            });
        }
    }

    #region Voice
    /// <summary>
    /// 보이스용.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClipInfo"></param>
    public void PlayVoiceSound(AudioSource audioSource, AudioClipInfo audioClipInfo)
    {
        PlayAudiosourceSound(audioSource, audioClipInfo, false);
    }

    public void PlayVoiceSound(string key, Action<bool> voiceCallback = null)
    {
        _voicePlayer.Play(key, voiceCallback);
    } 
    #endregion

    /// <summary>
    /// 사운드 재생.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClipInfo"></param>
    /// <param name="is3DSound"></param>
    private void PlayAudiosourceSound(AudioSource audioSource, AudioClipInfo audioClipInfo, bool is3DSound = true)
    {
        if (audioSource)
        {
            if(!SettingManager.Instance.IsOnSound(audioClipInfo.Type)){
                SetVolume(audioClipInfo.Type, 0);
            }

            if (audioClipInfo.Clip)
            {
                audioSource.clip = audioClipInfo.Clip;
            }
            if(audioClipInfo.Type == AudioType.BGM){
                is3DSound = false;   
            }
            audioSource = SetAudioSource(audioSource, _isVolumeAarry[(int)audioClipInfo.Type] * audioClipInfo.VolumeRate, audioClipInfo.IsLoop, is3DSound);
            if (audioClipInfo.Type == AudioType.BGM) audioSource.priority = priorityBGM;


            AudioCtrl.Play(audioSource, audioClipInfo.Type);

        }
    }

    /// <summary>
    /// 사운드 글로벌 세팅.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="volume"></param>
    /// <param name="isLoop"></param>
    /// <param name="is3DSound"></param>
    /// <returns></returns>
    private AudioSource SetAudioSource(AudioSource source, float volume, bool isLoop, bool is3DSound = true)
    {
        if (is3DSound)
        {
            source.spatialBlend = spatialBlend;
            source.spatialize = true;
            source.priority = priority3D;
        }
        else
        {
            source.spatialBlend = 0f;
            source.spatialize = false;
            source.priority = priority2D;
        }

        source.volume = volume;
        source.loop = isLoop;
        source.playOnAwake = false;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.enabled = true;
        return source;
    }

    /// <summary>
    /// AudioSource 체크 후 생성.
    /// </summary>
    /// <param name="pGameobject"></param>
    /// <param name="audioClipInfo"></param>
    /// <param name="is3DSound"></param>
    /// <returns></returns>
    private AudioSource PlaySoundCreateSource(GameObject pGameobject, AudioDataKey audioDataKey, bool is3DSound = true)
    {

        AudioSource source = null;
        AudioClipInfo audioClipInfo = GetAudioClipInfo(audioDataKey);
        if (pGameobject && audioClipInfo != null && audioClipInfo.Clip)
        {

            source = GetAudioSourceFromObject(pGameobject);

            if (source == null)
            {
                source = pGameobject.AddComponent<AudioSource>();
            }

            PlayAudiosourceSound(source, audioClipInfo, is3DSound);
        }

        return source;
    }

    private AudioSource GetAudioSourceFromObject(GameObject pGameobject)
    {
        AudioSource[] sources = pGameobject.GetComponents<AudioSource>();

        for (int i = 0; i < sources.Length; i++)
        {
            if (!sources[i].isPlaying)
            {
                return sources[i];
            }
        }

        return null;
    }

    /// <summary>
    /// gameobject & audioClipInfo
    /// </summary>
    /// <param name="pGameobject"></param>
    /// <param name="audioClipInfo"></param>
    /// <returns></returns>
    public AudioSource PlaySound(GameObject pGameobject, AudioDataKey audioDataKey)
    {
        return PlaySoundCreateSource(pGameobject, audioDataKey);
    }

    /// <summary>
    /// UI 사운드 플레이. (AudioType과 관계 없음)
    /// </summary>
    /// <param name="pGameobject"></param>
    /// <param name="audioClipInfo"></param>
    /// <returns></returns>
    public AudioSource PlayUISound(GameObject pGameobject, AudioDataKey audioDataKey)
    {
        string key = audioDataKey.ToString();
        return PlaySoundCreateSource(pGameobject, audioDataKey, false);
    }

    /// <summary>
    /// audioSource & AudioClipInfo.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClipInfo"></param>
    public void PlaySound(AudioSource audioSource, AudioDataKey audioDataKey, bool is3D)
    {
        if (audioSource)
        {
            AudioClipInfo audioClipInfo = GetAudioClipInfo(audioDataKey);
            //Ignore BGM Type - New AudioSource 
            if (audioSource.isPlaying && audioClipInfo.Type == AudioType.FX)
            {
                PlaySoundCreateSource(audioSource.gameObject, audioDataKey, is3D);
            }
            else
            {
                if(IsValidAudioClipInfo(audioClipInfo)){
                    PlayAudiosourceSound(audioSource, audioClipInfo, is3D);
                }
            }
        }
    }

    public void PlayUISoundInstance(AudioDataKey audioDataKey)
    {
        AudioClipInfo audioClipInfo = GetAudioClipInfo(audioDataKey);
        if (IsValidAudioClipInfo(audioClipInfo))
        {
            GameObject fxObject = new GameObject(audioClipInfo.Clip.name);
            SoundManager.Instance.PlayUISound(fxObject, audioDataKey);
            Destroy(fxObject, audioClipInfo.Clip.length + 0.1f);
        }
    }

    public void PlaySoundInstance(AudioDataKey audioDataKey, Vector3 point)
    {
        string key = audioDataKey.ToString();
        AudioClipInfo audioClipInfo =GetAudioClipInfo(audioDataKey);
        if (audioClipInfo != null && audioClipInfo.Clip != null)
        {
            GameObject fxObject = new GameObject(audioClipInfo.Clip.name);
            fxObject.transform.position = point;
            SoundManager.Instance.PlaySound(fxObject, audioDataKey);
            Destroy(fxObject, audioClipInfo.Clip.length + 0.1f);
        }
    }

    public AudioClipInfo GetAudioClipInfo(AudioDataKey audioDataKey)
    {
        return _audioDataBase.GetAudioClipInfo(audioDataKey.ToString());
    }

    public bool IsValidAudioClipInfo(AudioClipInfo audioClipInfo){
        return audioClipInfo!=null && audioClipInfo.Clip !=null;
    }

    //BGM
    public void PlayBgm(AudioDataKey key){
        _bgmPlayer.Play(key);
    }

    public void StopBgm(AudioDataKey key){
        _bgmPlayer.Stop(key);
    }
    public void StopAllBgm()
    {
        _bgmPlayer.StopAll();
    }
}