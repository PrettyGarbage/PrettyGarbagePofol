using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TermsOfServiceState : State
{
    [SerializeField] private Toggle _agreementToggle;
    [SerializeField] private Image _checker;
    [SerializeField] private Button _inactiveBtn;

    #region EVENTS
    public void OnPersonalInfoPolicy()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.privacyUrl);
    }

    public void OnEULA()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.eulaUrl);
    }

    public void OnServicePolicy()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.policyUrl);
    }

    public void OnFAQ()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.faqUrl);
    }

    public void OnQuit()
    {
        SystemUtil.QuitGame();
    }

    public void ToggleAgreementCheck()
    {
        Debug.Log("T/F : " + _agreementToggle.isOn);

        _inactiveBtn.gameObject.SetActive(!_agreementToggle.isOn);

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_toggle_on);
    }

    public void OnRegisterButton()
    {
        if(!_agreementToggle.isOn)
        {
            return;
        }

        StartCoroutine(WaitForSec());
    }

    public void InActiveMessage()
    {
        MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE,LocalizationTextKey.ALERT_NOT_AGREEMENT);
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(0.5f);

        if (DataManager.Instance.GetAssetBundleDownloadSize() > 0)
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(LoadContentsState), false);
        }
        else
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(TitleState), false);
        }        
    }
    #endregion

}