using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour {

	public const string DATA_ISONSOUND_SFX = "IsOnSound_SFX";
    public const string DATA_ISONSOUND_BGM = "IsOnSound_BGM";
    public const string DATA_ISONSOUND_VOICE = "IsOnSound_VOICE";
    public const string DATA_LANG = "LANG";
    private static SettingManager _instance;

	private bool _isOnSoundSFX = true;
    private bool _isOnSoundBGM = true;
    private bool _isOnSoundVOICE = true;

    public bool IsOnSoundSFX { get { return _isOnSoundSFX; } }
    public bool IsOnSoundBGM { get { return _isOnSoundBGM; } }
    public bool IsOnSoundVOICE { get { return _isOnSoundVOICE; } }

    //system info
    private PlatformType _platformType;
    private LangType _langType;

    public PlatformType PlatformType { get { return _platformType; } }
    public LangType LangType { get { return _langType; } }


    public static SettingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<SettingManager>("SettingManager");
                _instance._isOnSoundSFX = CommonConstants.INITEGER_TRUE == PlayerPrefs.GetInt(DATA_ISONSOUND_SFX , CommonConstants.INITEGER_TRUE);
                _instance._isOnSoundBGM = CommonConstants.INITEGER_TRUE == PlayerPrefs.GetInt(DATA_ISONSOUND_BGM , CommonConstants.INITEGER_TRUE);
                _instance._isOnSoundVOICE= CommonConstants.INITEGER_TRUE == PlayerPrefs.GetInt(DATA_ISONSOUND_VOICE, CommonConstants.INITEGER_TRUE);
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

	public void SetOnSound(AudioType audioType, bool isOnSound){
        switch (audioType)
        {
            case AudioType.BGM:
                _isOnSoundBGM = isOnSound;
                PlayerPrefs.SetInt(DATA_ISONSOUND_BGM, _isOnSoundBGM ? CommonConstants.INITEGER_TRUE : CommonConstants.INITEGER_FALSE);
                break;
            case AudioType.FX:
                _isOnSoundSFX = isOnSound;
                PlayerPrefs.SetInt(DATA_ISONSOUND_SFX, _isOnSoundSFX ? CommonConstants.INITEGER_TRUE : CommonConstants.INITEGER_FALSE);
                break;
            case AudioType.VOICE:
                _isOnSoundVOICE = isOnSound;
                PlayerPrefs.SetInt(DATA_ISONSOUND_VOICE, _isOnSoundVOICE ? CommonConstants.INITEGER_TRUE : CommonConstants.INITEGER_FALSE);
                break;
            default:
                break;
        }

        SoundManager.Instance.SetVolume(audioType, isOnSound? 1 : 0);
	}

    public bool IsOnSound(AudioType audioType){
        switch (audioType)
        {
            case AudioType.BGM:
                return _isOnSoundBGM;
            case AudioType.FX:
                return _isOnSoundSFX;
            case AudioType.VOICE:
                return _isOnSoundVOICE;                
            default:
                return false;                
        }        
    }

    public void SetSystem()
    {
#if UNITY_ANDROID
        _platformType = PlatformType.ANDROID;
#elif UNITY_IOS
        _platformType = PlatformType.IOS;
#endif

        int langTypeInt = PlayerPrefs.GetInt(DATA_LANG, -1);

        if (langTypeInt == -1)
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _langType = LangType.KR;
            }
            else if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                _langType = LangType.JP;
            }
            else
            {
                _langType = LangType.EN;
            }
        }
        else
        {
            _langType = (LangType)langTypeInt;
        }
        //_langType = LangType.EN;
    }

    public void SetLangType(LangType langType)
    {
        PlayerPrefs.SetInt(DATA_LANG, (int)langType);
        _langType = langType;

        LocalizationManager.Instance.LoadLocalizationItem(() => {
            EventPool.Send(EventNames.ON_CHANGE_LANG_TYPE);
        });

        //GameManager.Instance.Restart();
    }
}
