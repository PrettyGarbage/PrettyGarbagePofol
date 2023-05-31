using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library.Manager
{
    [System.Serializable]
    public class SoundClip
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
        [field: SerializeField, Range(0, 1)] public float DefaultVolume { get; private set; } = 0.5f;
    }

    ///<summary>
    ///사운드를 관리하는 싱글톤 객체입니다.
    ///</summary>
    public class SoundManager : AppContext<SoundManager>
    {
        #region Field

        [SerializeField] SoundClip[] soundClips;
        Dictionary<string, SoundClip> soundClipDictionary = new();
        AudioSource audioSourceForBGM;
        List<AudioSource> audioSourcesForEffect = new List<AudioSource>();
        bool isMuteBGM = false;
        bool isMuteEffect = false;

        #endregion

        #region Property

        public bool IsMuteBGM
        {
            get => isMuteBGM; set
            {
                isMuteBGM = value;

                audioSourceForBGM.mute = isMuteBGM;
            }
        }
        public bool IsMuteEffect
        {
            get => isMuteEffect; set
            {
                isMuteEffect = value;

                foreach (var audioSourceForEffect in audioSourcesForEffect)
                {
                    audioSourceForEffect.mute = isMuteEffect;
                }
            }
        }

        public AudioClip CurrentBGM => audioSourceForBGM.clip;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            soundClipDictionary = soundClips.ToDictionary(clip => clip.Key, clip => clip);
            
            audioSourceForBGM = gameObject.AddComponent<AudioSource>();
            audioSourceForBGM.loop = true;
            audioSourceForBGM.playOnAwake = false;
            
            Logger.LogEnter<SoundManager>();
        }

        #endregion

        #region Private Method

        AudioSource AddAudioSourceForEffect()
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSourcesForEffect.Add(audioSource);
            return audioSource;
        }

        void PlaySound(AudioSource audioSource, AudioClip sound, float volume)
        {
            audioSource.clip = sound;
            audioSource.volume = volume;
            audioSource.Play();
        }

        #endregion

        #region Public Method

        ///<summary>
        ///BGM을 재생합니다.
        ///</summary>
        ///<param name="soundKey">재생할 사운드 정보에 대한 참조키</param>
        ///<param name="volume">볼륨(0일 시 기본 값)</param>
        public AudioSource PlaySoundBGM(string soundKey, float volume = 0f) => PlaySoundBGM(soundClipDictionary[soundKey], volume);
        
        ///<summary>
        ///BGM을 재생합니다.
        ///</summary>
        ///<param name="sound">재생할 사운드 정보</param>
        ///<param name="volume">볼륨(0일 시 기본 값)</param>
        public AudioSource PlaySoundBGM(SoundClip sound, float volume = 0f)
        {
            return PlaySoundBGM(sound.AudioClip, volume == 0 ? sound.DefaultVolume : volume);
        }

        public AudioSource PlaySoundBGM(AudioClip sound, float volume)
        {
            if (CurrentBGM == sound) return null;

            audioSourceForBGM.mute = isMuteBGM;
            PlaySound(audioSourceForBGM, sound, volume);

            return audioSourceForBGM;
        }

        ///<summary>
        ///효과음을 재생합니다.
        ///</summary>
        ///<param name="soundKey">재생할 사운드 정보에 대한 참조키</param>
        ///<param name="volume">볼륨(0일 시 기본 값)</param>
        public AudioSource PlaySoundEffect(string soundKey, float volume = 0f)
        {
            try
            {
                return PlaySoundEffect(soundClipDictionary[soundKey], volume);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        ///<summary>
        ///효과음을 재생합니다.
        ///</summary>
        ///<param name="sound">재생할 사운드 정보</param>
        ///<param name="volume">볼륨(0일 시 기본 값)</param>
        public AudioSource PlaySoundEffect(SoundClip sound, float volume = 0f)
        {
            return PlaySoundEffect(sound.AudioClip, volume == 0 ? sound.DefaultVolume : volume);
        }

        public AudioSource PlaySoundEffect(AudioClip sound, float volume = 0.5f)
        {
            if (isMuteEffect) return null;

            for (int i = 0; i < audioSourcesForEffect.Count; i++)
            {
                if (!audioSourcesForEffect[i].isPlaying)
                {
                    PlaySound(audioSourcesForEffect[i], sound, volume);
                    return audioSourcesForEffect[i];
                }
            }

            var newAudioSource = AddAudioSourceForEffect();
            PlaySound(newAudioSource, sound, volume);
            return newAudioSource;
        }

        #endregion
    }
}
