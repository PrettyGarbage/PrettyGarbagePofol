using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupState {

	public override string stateName { get { return GameConstants.STATENAME_POPUPSETTING; } }

	[Header("Settings")]
	[SerializeField] Button _closeButton;
	[SerializeField] Button _sfxButton;
	[SerializeField] Button _bgmButton;
	[SerializeField] Button _signInButton;
	[SerializeField] Button _signOutButton;
	[SerializeField] Button _quitButton;
	
	[SerializeField]
	private UIToggleSwitch _sfxToggleSwich;
	[SerializeField]
	private UIToggleSwitch _bgmToggleSwich;

	[Header("version")]
	[SerializeField]
	private TMP_Text _versionText;


	public override IEnumerator OnInitialize()
	{
		yield return base.OnInitialize();

		_closeButton.onClick.AddListener(OnBack);
		_sfxButton.onClick.AddListener(sfxToggleSound);
		_bgmButton.onClick.AddListener(bgmToggleSound);

		_signInButton.onClick.AddListener(() => {
			NetworkManager.Instance.SignIn();
		});

		_signOutButton.onClick.AddListener(() => {
			NetworkManager.Instance.SignOut();
		});

		_quitButton.onClick.AddListener(Application.Quit);
		_versionText.text = BuildManager.Instance.GetVersion();
	}

	public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_bgmToggleSwich.ToggleSoundSwich(SettingManager.Instance.IsOnSoundBGM);
		_sfxToggleSwich.ToggleSoundSwich(SettingManager.Instance.IsOnSoundSFX);

		_signInButton.gameObject.SetActive(!NetworkManager.Instance.IsLoggedIn());
		_signOutButton.gameObject.SetActive(NetworkManager.Instance.IsLoggedIn());
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		_signInButton.gameObject.SetActive(!NetworkManager.Instance.IsLoggedIn());
		_signOutButton.gameObject.SetActive(NetworkManager.Instance.IsLoggedIn());
	}

	private void sfxToggleSound(){
		bool isOnSound = SettingManager.Instance.IsOnSoundSFX;
		SettingManager.Instance.SetOnSound(AudioType.FX, !isOnSound);
		_sfxToggleSwich.ToggleSoundSwich(!isOnSound);

	}

	private void bgmToggleSound(){
		bool isOnSound = SettingManager.Instance.IsOnSoundBGM;
		SettingManager.Instance.SetOnSound(AudioType.BGM, !isOnSound);
		_bgmToggleSwich.ToggleSoundSwich(!isOnSound);
	}
}