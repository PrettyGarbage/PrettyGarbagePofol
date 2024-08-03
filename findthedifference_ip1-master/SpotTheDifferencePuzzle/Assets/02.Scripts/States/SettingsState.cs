using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsState : PopupState
{
	[Header("Settings")]
	[SerializeField] Button _signInButton;
	[SerializeField] Button _signOutButton;
    [SerializeField] TMP_Text _verText;

    public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(SettingsState));
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

        _verText.text = BuildManager.Instance.BuildInfoData.Version +" " 
            + ((BuildManager.Instance.BuildInfoData.IsDevBuild)? "Dev":"Real");

		SetLoginState();
	}

	void SetLoginState()
	{
        LoginType loginType = DataManager.Instance.UserInfo.LoginType;
		_signInButton.gameObject.SetActive(loginType == LoginType.NONE);
		_signOutButton.gameObject.SetActive(loginType != LoginType.NONE);
	}

	#region EVENTS

	public void SignIn()
	{
        NetworkManager.Instance.SignInPlatform(LoginType.GOOGLE, authLoginForm =>
        {
            ApiManager.Instance.CheckLogin(authLoginForm, apiResult => {
                if (apiResult.isSuccess)
                {
                    //이미 다른 아이디가 존재함.
                    if (apiResult.result)
                    {
                        MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_POPUP_GUEST_DATADELETE_DESC, () => {

                            NetworkManager.Instance.SignInToServer(authLoginForm, isSuccess =>
                            {
                                GameManager.Instance.Restart();
                            });

                        }, () => {
                            //NO
                        });
                    }
                    else
                    {
                        
                        NetworkManager.Instance.SignInToServer(authLoginForm, isSuccess =>
                        {
                            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_POPUP_PLATFORM_DATADELETE_DESC, () =>
                            {
                                GameManager.Instance.Restart();
                            });
                        });
                    }
                }                
            });            
        });

    }

    public void SignOut()
	{
		NetworkManager.Instance.SignOut();
	}

    public void OnSupport()
    {
        PopupSupportState.Open();
    }

    public void OnLangSelect()
    {
        PopupLangSelectState.Open();
    }

    public void OnGoldInfo()
    {
        PopupGoldInfoState.Open();
    }

    public void OnFaq()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.faqUrl);
    }

	public void Quit()
	{
        SystemUtil.QuitGame();
    }
	#endregion EVENTS
}