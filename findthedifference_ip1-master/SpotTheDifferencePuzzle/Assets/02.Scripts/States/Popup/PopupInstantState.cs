using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInstantStateData : BaseStateData
{
    public PopupInstantType popupInstantType;
    public string themeName;
    public Action onClosed;

    public PopupInstantStateData(){}

    public PopupInstantStateData(PopupInstantType popupInstantType, string themeName, Action onClosed)
    {
        this.popupInstantType = popupInstantType;
        this.themeName = themeName;
        this.onClosed = onClosed;
    }
}

public class PopupInstantState : PopupState
{
    [Header("PopupInstantState")]
    [SerializeField]
    private float _duration = 2f;
    [SerializeField]
    private LocalizationUIText _titleText;
    [SerializeField]
    private LocalizationUIText _messageText;

    private Vector3 _startPoint, _endPoint;

    private Action _onClosed;

    public static void Open(PopupInstantType popupInstantType, string themeName=null, Action onClosed=null)
    {
        StateManager.Instance.OpenPopupState(typeof(PopupInstantState), new PopupInstantStateData(popupInstantType, themeName, onClosed));
    }

    public override IEnumerator OnInitialize()
    {
        _stateType = StateType.POPUP;
        _startPoint = new Vector3(2000, _popup.transform.localPosition.y, 0);
        _endPoint = new Vector3(-2000, _popup.transform.localPosition.y, 0);
        _popup.transform.localPosition = _startPoint;
        yield return null;
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        StateManager.Instance.ActiveBlockScreen(true);

        PopupInstantStateData instantStateData =  GetData<PopupInstantStateData>();
        _onClosed = instantStateData.onClosed;
        SetContent(instantStateData);

    }

    private void SetContent(PopupInstantStateData instantStateData)
    {
        _messageText.gameObject.SetActive(true);
        switch (instantStateData.popupInstantType)
        {
            case PopupInstantType.THEME_OPEN_MASTER:
                _titleText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_MASTERUNLOCK);
                _messageText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_ALLCLEAR_TITLE);
                break;
            case PopupInstantType.THEME_COMPLETE:
                _titleText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_GETALLSTARCOIN);
                _messageText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_GETALLSTARCOIN_TITLE);
                break;
            case PopupInstantType.THEME_UNLOCK:
                _titleText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_THEMEUNLOCK, instantStateData.themeName);
                _messageText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_ALLCLEAR_TITLE);
                break;
            case PopupInstantType.COLLECTION_UNLOCK:
                _messageText.gameObject.SetActive(false);
                _titleText.SetText(LocalizationTextKey.LOBBY_ACTIONTXT_GETCOLLECTION);
                break;
            case PopupInstantType.PURCHSE_SUCCESS:
                _messageText.gameObject.SetActive(false);
                _titleText.SetText(LocalizationTextKey.STORE_PURCHASEPOPUP_SUCESS_TITLE);
                break;
            default:
                break;
        }
    }

    public override IEnumerator OnOpening()
    {
        yield return new WaitForSeconds(0.1f);

        _popup.transform.localPosition = _startPoint;
        LeanTween.moveLocalX(_popup, 0f, _popTime).
             setEase(_showEase).
            setIgnoreTimeScale(true);

        SoundManager.Instance.PlayUISoundInstance(_sfxOpenKey);

        yield return new WaitForSeconds(_popTime);
    }

    public override IEnumerator OnPostOpen()
    {
        yield return base.OnPostOpen();

        yield return new WaitForSeconds(_duration);

        OnBack();
    }

    public override IEnumerator OnClosing()
    {
        yield return new WaitForSeconds(0.1f);

        LeanTween.moveLocalX(_popup, _endPoint.x, _popTime).
             setEase(_showEase).
            setIgnoreTimeScale(true);

        SoundManager.Instance.PlayUISoundInstance(_sfxCloseKey);

        yield return new WaitForSeconds(_popTime);

        StateManager.Instance.ActiveBlockScreen(false);
    }

    public override void OnPostClosed()
    {
        base.OnPostClosed();
        if (_onClosed != null)
            _onClosed();
    }

}
