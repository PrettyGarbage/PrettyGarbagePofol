using UnityEngine;
using UnityEngine.UI;

public class UISoundController : MonoBehaviour
{
    [SerializeField]
    private AudioType _audioType;

	[SerializeField] GameObject _buttonSoundOnOffObject;
	[SerializeField] Button _buttonSoundOffOffObject;

	#region UNITY EVENTS
	void OnEnable()
	{
		if (DataManager.Instance.UserInfo != null)
		{
			SetSoundState(SettingManager.Instance.IsOnSound(_audioType));
		}
	}
	#endregion UNITY EVENTS

	void SetSoundState(bool isOn)
	{
        _buttonSoundOnOffObject.gameObject.SetActive(!isOn);
        _buttonSoundOffOffObject.gameObject.SetActive(isOn);
	}

	#region EVENTS
	public void SoundOn()
	{
		if (SettingManager.Instance.IsOnSound(_audioType))
		{
			return;	
		}

		SettingManager.Instance.SetOnSound(_audioType, true);

        SetSoundState(true);
	}

	public void SoundOff()
	{
		if (!SettingManager.Instance.IsOnSound(_audioType))
		{
			return;
		}

        SettingManager.Instance.SetOnSound(_audioType, false);

        SetSoundState(false);
	}
	#endregion EVENTS
}