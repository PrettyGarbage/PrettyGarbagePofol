using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountsState : PopupState
{

    [SerializeField]
    private GameObject _warningObject;

    public override IEnumerator OnInitialize()
    {
        _warningObject.gameObject.SetActive(false);
        return base.OnInitialize();
    }


    public void OnSigninGoogle()
    {
        StateManager.Instance.ActiveBlockScreen(true);
        NetworkManager.Instance.SignInPlatform(LoginType.GOOGLE, authLoginForm =>
        {
            RequestLoginServer(authLoginForm);
        });
    }

    public void OnSigninGuest()
    {
        _warningObject.gameObject.SetActive(true);
    }

    public void OnSigninGuestCancel()
    {
        _warningObject.gameObject.SetActive(false);
    }

    public void OnSigninGuestContinue()
    {
        _warningObject.gameObject.SetActive(false);
        StateManager.Instance.ActiveBlockScreen(true);
        NetworkManager.Instance.SignInPlatform(LoginType.NONE, authLoginForm =>
        {
            RequestLoginServer(authLoginForm);
        });
    }

    private void RequestLoginServer(AuthLoginForm authLoginForm)
    {
        if (authLoginForm == null)
        {
            StateManager.Instance.ActiveBlockScreen(false);
            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_FAIL_ALERT);
        }
        else
        {
            StateManager.Instance.ShowSpinner();
            NetworkManager.Instance.SignInToServer(authLoginForm, isSuccess =>
            {
                LoadData(isSuccess);
            });
        }
    }

    private void LoadData(bool isSignIn)
    {  
        StateManager.Instance.ActiveBlockScreen(false);
        if (isSignIn)
        {
            OnBack();
        }        
    }

    public override void OnPostClosed()
    {
        DataManager.Instance.LoadGameDataInfo(() =>
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), false);
        });        
    }
}
