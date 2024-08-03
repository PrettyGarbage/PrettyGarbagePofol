using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupUpdateDownloadStateData : BaseStateData
{
    public double updateFileMbSize;
    public Action onNext;
    public PopupUpdateDownloadStateData() { }
    public PopupUpdateDownloadStateData(double updateFileMbSize, Action onNext)
    {
        this.updateFileMbSize = updateFileMbSize;
        this.onNext = onNext;
    }
}

public class PopupUpdateDownloadState : PopupState
{
    [Header("UpdateDownload")]    
    [SerializeField]
    private TMP_Text _totalSizeText;
    
    private Vector3 _startPoint, _endPoint;

    private Action _onClosed;

    public static void Open(double updateFileMbSize, Action onNext)
    {  
        StateManager.Instance.OpenPopupState(typeof(PopupUpdateDownloadState), new PopupUpdateDownloadStateData(updateFileMbSize, onNext));
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        PopupUpdateDownloadStateData popupUpdateDownloadStateData = GetData<PopupUpdateDownloadStateData>();
        _onClosed = popupUpdateDownloadStateData.onNext;

        _totalSizeText.text = string.Format("{0}MB", Math.Round(popupUpdateDownloadStateData.updateFileMbSize,2));

    }

    public void OnNext()
    {
        OnBack();
    }

    public void OnCancel()
    {
        SystemUtil.QuitGame();
    }

    public override void OnPostClosed()
    {
        base.OnPostClosed();
        if (_onClosed != null)
            _onClosed();
    }

}
