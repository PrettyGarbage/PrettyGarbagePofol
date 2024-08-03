using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour {

	public const string DATA_ISONSOUND_SFX = "IsOnSound_SFX";
    public const string DATA_ISONSOUND_BGM = "IsOnSound_BGM";
	private static SettingManager _instance;

	private bool _isOnSoundSFX = true;
    private bool _isOnSoundBGM = true;

	public bool IsOnSoundSFX
    {
        get
        {
            return _isOnSoundSFX;
        }
    }

    public bool IsOnSoundBGM
    {
        get
        {
            return _isOnSoundBGM;
        }
    }

	
	public static SettingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<SettingManager>("SettingManager");
                _instance._isOnSoundSFX = CommonConstants.INITEGER_TRUE == PlayerPrefs.GetInt(DATA_ISONSOUND_SFX , CommonConstants.INITEGER_TRUE);
                _instance._isOnSoundBGM = CommonConstants.INITEGER_TRUE == PlayerPrefs.GetInt(DATA_ISONSOUND_BGM , CommonConstants.INITEGER_TRUE);
                Debug.LogFormat("{0} / {1}", _instance._isOnSoundSFX, _instance._isOnSoundBGM);
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

	public void SetOnSound(AudioType audioType, bool isOnSound){
        if(audioType == AudioType.FX){
		    _isOnSoundSFX = isOnSound;
		    PlayerPrefs.SetInt(DATA_ISONSOUND_SFX , _isOnSoundSFX ? CommonConstants.INITEGER_TRUE: CommonConstants.INITEGER_FALSE);
        }else {            
            _isOnSoundBGM = isOnSound;
            PlayerPrefs.SetInt(DATA_ISONSOUND_BGM , _isOnSoundBGM ? CommonConstants.INITEGER_TRUE: CommonConstants.INITEGER_FALSE);
        }
        SoundManager.Instance.SetVolume(audioType, isOnSound? 1 : 0);
	}

    public bool IsOnSound(AudioType audioType){
        return audioType == AudioType.FX ? _isOnSoundSFX : _isOnSoundBGM;
    }

}
