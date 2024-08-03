using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupBirthdayInputStateData : BaseStateData
{
    public Action<bool> callback;
    public PopupBirthdayInputStateData() { }

    public PopupBirthdayInputStateData(Action<bool> callback)
    {
        this.callback = callback;
    }
}

public class PopupBirthdayInputState : PopupState
{
    [Header("BirthdayInputState")]
    [SerializeField]
    private TMP_Text _yearText;
    [SerializeField]
    private TMP_Text _monthText;

    [SerializeField]
    private GameObject _yearSelectObject;
    [SerializeField]
    private GameObject _monthSelectObject;
    [SerializeField]
    private Transform _yearSelectObjectContent;
    [SerializeField]
    private Transform _monthSelectObjectContent;

    [SerializeField]
    private BirthdayInputItem _pfBirthdayInputItem;

    Action<bool> _callback;

    private bool _isSummitSuccess;

    public override IEnumerator OnInitialize()
    {

        for (int i = 2018; i >= 1919; i--)
        {
            BirthdayInputItem birthdayInputItem = Instantiate<BirthdayInputItem>(_pfBirthdayInputItem, _yearSelectObjectContent);
            birthdayInputItem.SetItem(i.ToString(), OnInputYearSelected);
        }

        for (int i = 1; i <= 12; i++)
        {
            BirthdayInputItem birthdayInputItem = Instantiate<BirthdayInputItem>(_pfBirthdayInputItem, _monthSelectObjectContent);
            birthdayInputItem.SetItem(i<10? "0"+ i.ToString()  : i.ToString(), OnInputMonthSelected);
        }

        yield return base.OnInitialize();
    }

    public static void Open(Action<bool> callback)
    {
        StateManager.Instance.OpenPopupState(typeof(PopupBirthdayInputState), new PopupBirthdayInputStateData(callback));
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);
        _yearSelectObject.SetActive(false);
        _monthSelectObject.SetActive(false);

        PopupBirthdayInputStateData stateData = GetData<PopupBirthdayInputStateData>();
        _callback = stateData.callback;
    }

    public override IEnumerator OnPostOpen()
    {
        return base.OnPostOpen();       
    }

    public override void OnBack()
    {
        base.OnBack();
    }

    public override void OnPostClosed()
    {
        _callback(_isSummitSuccess);
    }

    public void OnInputYear()
    {
        _yearSelectObject.SetActive(true);
    }

    public void OnInputMonth()
    {
        _monthSelectObject.SetActive(true);
    }

    private void OnInputMonthSelected(string month)
    {
        _monthSelectObject.SetActive(false);
        _monthText.text = month;
    }

    private void OnInputYearSelected(string year)
    {
        _yearSelectObject.SetActive(false);
        _yearText.text = year;
    }

    public void OnSummit()
    {
        if(_yearText.text.Length != 4 || _monthText.text.Length != 2)
        {
            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.SHOP_JP_NO_BIRTHDAY);
            return;
        }

        StateManager.Instance.ActiveBlockScreen(true);
        ApiManager.Instance.PostBirthDay(_yearText.text + _monthText.text + "01", apiResult => {

            _isSummitSuccess = apiResult.isSuccess;
            if (apiResult.isSuccess)
            {
                OnBack();                
            }
            else
            {
                MessageBoxState.Open(apiResult.errorInfo);
            }
            StateManager.Instance.ActiveBlockScreen(false);
        });
    }

    public void OnNoticeWebview()
    {
        Debug.Log("OnNoticeWebview");
        WebViewManager.Instance.Show(DataManager.Instance.AppInfo.jppurchasePolicyUrl);
    }

}