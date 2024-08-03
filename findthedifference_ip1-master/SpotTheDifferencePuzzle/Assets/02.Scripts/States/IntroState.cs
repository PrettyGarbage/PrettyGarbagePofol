using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class IntroState : State
{
    public const string NAME = "Intro";

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);
        StateManager.Instance.ShowSpinner();

        DataManager.Instance.SetAppInfo();

    }

    public override IEnumerator OnPostOpen()
    {

        while (DataManager.Instance.AppInfo == null)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }

        Debug.Log(string.Format("=====> Version {0}, {1}", DataManager.Instance.AppInfo.clientVersion, BuildManager.Instance.BuildInfoData.Version));
        string[] sVersion = DataManager.Instance.AppInfo.clientVersion.Split('.');
        string[] cVersion = BuildManager.Instance.BuildInfoData.Version.Split('.');
        bool isMatchVersion = sVersion[0].Equals(cVersion[0]) && sVersion[1].Equals(cVersion[1]);

        //점검체크.
        if (DataManager.Instance.AppInfo.maintenanceYn)
        {
            MessageBoxState.Open(LocalizationTextKey.INTRO_NOTICE_TITLE, LocalizationTextKey.INTRO_NOTICE_DESC_MAINTENANCE, LocalizationTextKey.INTRO_NOTICE_BUTTON_MAINTENANCE, () =>
            {
                GameManager.Instance.Restart();
            });
        }
        //클라 버전체크.
        else if (!isMatchVersion)
        {
            Debug.Log( string.Format("Ser clientVersion :  {0}, local : {1}" , DataManager.Instance.AppInfo.clientVersion, BuildManager.Instance.BuildInfoData.Version));
            MessageBoxState.Open(LocalizationTextKey.INTRO_NOTICE_TITLE, LocalizationTextKey.INTRO_NOTICE_DESC_VERSION, LocalizationTextKey.INTRO_NOTICE_BUTTON_VERSION, () =>
            {
#if UNITY_EDITOR
                GameManager.Instance.Restart();
#elif UNITY_IOS
                Application.OpenURL("itms-apps://itunes.apple.com/app/" + CommonConstants.IOS_APP_ID);
#elif UNITY_ANDROID
                Application.OpenURL("market://details?id=" + CommonConstants.ANDROID_PACKAGE_NAME);
#endif
            });
        }
        else
        {
            if (DataManager.Instance.AssetBundleInfoList == null)
            {
                yield return StartCoroutine(LoadAssetBundleData());
            }

            StateManager.Instance.HideSpinner();
            if (UserInfo.IsLogined())
            {
                if (DataManager.Instance.GetAssetBundleDownloadSize() > 0)
                {
                    StateManager.Instance.OpenState<BaseStateData>(typeof(LoadContentsState), false);
                }
                else
                {
                    DataManager.Instance.LoadGameDataInfo(() =>
                    {
                        StateManager.Instance.OpenState<BaseStateData>(typeof(TitleState), false);
                    });
                }
            }
            else
            {
                StateManager.Instance.OpenState<BaseStateData>(typeof(TermsOfServiceState), false);
            }
        }
    }

    IEnumerator LoadAssetBundleData()
    {
        bool isLoadedAssetData = false;

        ApiManager.Instance.GetAssetBundleData((apiResult) =>
        {
            DataManager.Instance.SetAssetBundleData(apiResult.result);
            isLoadedAssetData = true;
        });

        while (!isLoadedAssetData)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
    }

}