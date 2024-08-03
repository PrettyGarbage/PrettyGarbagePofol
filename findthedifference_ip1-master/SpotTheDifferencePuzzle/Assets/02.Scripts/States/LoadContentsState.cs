using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LoadContentsState : State
{
    [SerializeField] Slider _totalProgress;
    [SerializeField] Slider _bundleProgress;

    [SerializeField]
    private TMP_Text _totalLoadFileSizeText;

    private double _loadedFileByteSize;
    private double _totalFileMbSize;

    private List<AssetBundleInfo> _downloadAssetBundleInfoList;

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);

        _totalLoadFileSizeText.text = string.Empty;
        _totalLoadFileSizeText.gameObject.SetActive(false);

        _totalProgress.value = 0.0f;
        _bundleProgress.value = 0.0f;
    }

    public override IEnumerator OnPostOpen()
    {
        if (!AssetBundleManager.Instance.IsLoaded)
        {
            List<AssetBundleInfo> assetBundleInfoList = DataManager.Instance.AssetBundleInfoList;
            yield return StartCoroutine(CheckUpdateDownload(assetBundleInfoList));
            yield return StartCoroutine(LoadAssetBundles(_downloadAssetBundleInfoList));
        }

        if (UserInfo.IsLogined())
        {
            DataManager.Instance.LoadGameDataInfo(() =>
            {
                StateManager.Instance.OpenState<BaseStateData>(typeof(TitleState), false);
            });
        }
        else
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(TitleState), false);
        }
    }

    public void OnUpdateLoadFileSize(double loadedFileSize, double totalFileMbSize)
    {
        Debug.Log("loadedFileSize : " + loadedFileSize);
        _totalLoadFileSizeText.text = String.Format("{0}MB / {1}MB", Math.Round(SystemUtil.ConvertBytesToMegabytes(loadedFileSize),2), totalFileMbSize);
    }

    IEnumerator CheckUpdateDownload(List<AssetBundleInfo> assetBundleInfoList)
    {

        bool isChecked = false;

        _downloadAssetBundleInfoList = AssetBundleManager.Instance.GetUpdateDownloadList(assetBundleInfoList);
        _totalFileMbSize = AssetBundleManager.Instance.GetTotalFileMbSize(_downloadAssetBundleInfoList);        
        
        Debug.Log("CheckUpdateDownload : " + _totalFileMbSize);

        if (_totalFileMbSize > 0)
        {
            PopupUpdateDownloadState.Open(_totalFileMbSize, () => {
                _totalLoadFileSizeText.gameObject.SetActive(true);
                OnUpdateLoadFileSize(0, _totalFileMbSize);
                isChecked = true;
            });
        }
        else
        {
            isChecked = true;
        }

        while (!isChecked)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
    }

    IEnumerator LoadAssetBundles(List<AssetBundleInfo> assetBundleInfoList)
    {
        yield return AssetBundleManager.Instance.LoadAssetBundlesCor(assetBundleInfoList, true
            , (assetbundlePregressInfo) => {
                _totalProgress.value = assetbundlePregressInfo.totalProgress;
                _bundleProgress.value = assetbundlePregressInfo.partsProgress;
                OnUpdateLoadFileSize(assetbundlePregressInfo.loadedFileByteSize, assetbundlePregressInfo.totalFileMbSize);
            },
            () => {
      
            }
        );

        _totalProgress.value = 1f;
        _bundleProgress.value = 1.0f;
        AssetBundleManager.Instance.IsLoaded = true;
    }    
}