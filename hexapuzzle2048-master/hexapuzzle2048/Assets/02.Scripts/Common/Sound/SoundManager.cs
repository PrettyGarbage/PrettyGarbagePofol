using UnityEngine;
using UnityEditor;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    const string AUDIO_DATA_PATH = "Data/AudioDataBase";

    private static SoundManager _instance;

    [SerializeField]
    private AudioDataBase _audioDataBase;

    //public bool isMute = false;
    public float spatialBlend = 0.95f;
    public int priority3D = 128;
    public int priority2D = 10;
    public int priorityBGM = 5;
    public float minDistance = 2f;
    public float maxDistance = 40f;

    public float[] isVolumeAarry = { 1f, 1f };

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

    public void SetInitVolume(float bgmVol, float fxVol)
    {
        isVolumeAarry[(int)AudioType.BGM] = bgmVol;
        isVolumeAarry[(int)AudioType.FX] = fxVol;
    }

    public bool GetIsMute(AudioType audioType)
    {
        return isVolumeAarry[(int)audioType] == 0f;
    }

    public void SetVolume(AudioType audioType, float volume)
    {
        isVolumeAarry[(int)audioType] = volume;
        EventPool.Send(EventNames.ON_AUDIO_VOLUME_CTRL, audioType, volume);
    }

    public float GetVolume(AudioType audioType)
    {
        return isVolumeAarry[(int)audioType];
    }

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
            audioSource = SetAudioSource(audioSource, isVolumeAarry[(int)audioClipInfo.Type] * audioClipInfo.VolumeRate, audioClipInfo.IsLoop, is3DSound);
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
    private AudioSource PlaySoundCreateSource(GameObject pGameobject, string key, bool is3DSound = true)
    {

        AudioSource source = null;
        AudioClipInfo audioClipInfo = GetAudioClipInfo(key);
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
    public AudioSource PlaySound(GameObject pGameobject, string key)
    {
        return PlaySoundCreateSource(pGameobject, key);
    }

    /// <summary>
    /// UI 사운드 플레이. (AudioType과 관계 없음)
    /// </summary>
    /// <param name="pGameobject"></param>
    /// <param name="audioClipInfo"></param>
    /// <returns></returns>
    public AudioSource PlayUISound(GameObject pGameobject, string key)
    {
        return PlaySoundCreateSource(pGameobject, key, false);
    }

    /// <summary>
    /// audioSource & AudioClipInfo.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioClipInfo"></param>
    public void PlaySound(AudioSource audioSource, string key, bool is3D)
    {
        if (audioSource)
        {
            AudioClipInfo audioClipInfo = GetAudioClipInfo(key);
            //Ignore BGM Type - New AudioSource 
            if (audioSource.isPlaying && audioClipInfo.Type == AudioType.FX)
            {
                PlaySoundCreateSource(audioSource.gameObject, key, is3D);
            }
            else
            {
                if(IsValidAudioClipInfo(audioClipInfo)){
                    PlayAudiosourceSound(audioSource, audioClipInfo, is3D);
                }
            }
        }
    }

    public void PlayUISoundInstance(string key){
        AudioClipInfo audioClipInfo = GetAudioClipInfo(key);
        if(IsValidAudioClipInfo(audioClipInfo)){
            GameObject fxObject = new GameObject(audioClipInfo.Clip.name);
            SoundManager.Instance.PlayUISound(fxObject, key);
            Destroy(fxObject, audioClipInfo.Clip.length + 0.1f);    
        }
    }

    public void PlaySoundInstance(string key, Vector3 point)
    {
        AudioClipInfo audioClipInfo =GetAudioClipInfo(key);
        if (audioClipInfo != null && audioClipInfo.Clip != null)
        {
            GameObject fxObject = new GameObject(audioClipInfo.Clip.name);
            fxObject.transform.position = point;
            SoundManager.Instance.PlaySound(fxObject, key);
            Destroy(fxObject, audioClipInfo.Clip.length + 0.1f);
        }
    }

    public AudioClipInfo GetAudioClipInfo(string key){
        return _audioDataBase.GetAudioClipInfo(key);
    }

    public bool IsValidAudioClipInfo(AudioClipInfo audioClipInfo){
        return audioClipInfo!=null && audioClipInfo.Clip !=null;
    }

    //BGM
    public void PlayBgm(string key){
        _bgmPlayer.Play(key);
    }

    public void StopBgm(string key){
        _bgmPlayer.Stop(key);
    }

}