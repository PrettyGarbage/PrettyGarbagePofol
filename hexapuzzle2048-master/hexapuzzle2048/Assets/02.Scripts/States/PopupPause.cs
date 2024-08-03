using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
args 
onHomeClick 
onRestartClick
 */
public class PopupPause : PopupState
{
	public override string stateName { get { return GameConstants.STATENAME_POPUPPAUSE; } }

	[Header("Pause")]
	[SerializeField] Button _closeButton;
	[SerializeField] Button _sfxButton;
	[SerializeField] Button _bgmButton;
	[SerializeField] Button _homeButton;
	[SerializeField] Button _restartButton;
	[SerializeField] UIToggleSwitch _sfxToggleSwich;
	[SerializeField] UIToggleSwitch _bgmToggleSwich;

	[Header("version")]
	[SerializeField]
	private TMP_Text _versionText;

	public override IEnumerator OnInitialize()
	{
		yield return base.OnInitialize();

		_closeButton.onClick.AddListener(OnBack);
		_sfxButton.onClick.AddListener(sfxToggleSound);
		_bgmButton.onClick.AddListener(bgmToggleSound);

		_homeButton.onClick.AddListener(() =>
			StateManager.instance.OpenState(GameConstants.STATENAME_LOBBY)
		);

		_restartButton.onClick.AddListener(() =>{
			DataManager.Instance.DeletePlayData();			
			StateManager.instance.OpenStateForced(GameConstants.STATENAME_INGAME);
		});

		_versionText.text = BuildManager.Instance.GetVersion();
	}

	public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_bgmToggleSwich.ToggleSoundSwich(SettingManager.Instance.IsOnSoundBGM);
		_sfxToggleSwich.ToggleSoundSwich(SettingManager.Instance.IsOnSoundSFX);
	}

	private void sfxToggleSound()
	{
		bool isOnSound = SettingManager.Instance.IsOnSoundSFX;
		SettingManager.Instance.SetOnSound(AudioType.FX, !isOnSound);
		_sfxToggleSwich.ToggleSoundSwich(!isOnSound);
	}

	private void bgmToggleSound()
	{
		bool isOnSound = SettingManager.Instance.IsOnSoundBGM;
		SettingManager.Instance.SetOnSound(AudioType.BGM, !isOnSound);
		_bgmToggleSwich.ToggleSoundSwich(!isOnSound);
	}
}
