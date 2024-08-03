using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSupportState : PopupState {

    [SerializeField] GameObject _japanPolicyObj;

    public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupSupportState));
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        _japanPolicyObj.SetActive(SettingManager.Instance.LangType == LangType.JP);

        return base.OnPreOpen(args);
    }

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

    public void OnJapanPolicy()
    {
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.jppurchasePolicyUrl);
    }


    #endregion
}
