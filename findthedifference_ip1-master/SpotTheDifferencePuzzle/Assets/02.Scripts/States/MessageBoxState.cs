using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBoxStateData : BaseStateData
{
    public LocalizationTextKey titleKey;
    public LocalizationTextKey messageKey;
    public LocalizationTextKey okButtonKey = LocalizationTextKey.COMMON_BUTTON_CONFIRM;
    public LocalizationTextKey cancelButtonKey = LocalizationTextKey.COMMON_BUTTON_CANCEL;
    public Action onOk;
    public Action onCancel;

    public MessageBoxStateData()
    {
    }

    public MessageBoxStateData(LocalizationTextKey titleKey, LocalizationTextKey messageKey, Action onOk = null, Action onCancel = null)
    {
        this.titleKey = titleKey;
        this.messageKey = messageKey;
        this.onOk = onOk;
        this.onCancel = onCancel;        
    }
}

public class MessageBoxState : PopupState
{
	[Header("Message Box")]
	[SerializeField] LocalizationUIText _title;
	[SerializeField] LocalizationUIText _message;
    [SerializeField] LocalizationUIText _okButtonText;
    [SerializeField] LocalizationUIText _cancelButtonText;
    [SerializeField] GameObject _OkButton;
	[SerializeField] GameObject _cancelButton;

	Action _onOK;
	Action _onCancel;
    Action _onCallback;

    private static MessageBoxStateData messageBoxStateData;

    public static void Open(GameErrorInfo errorInfo, Action onOk = null, Action onCancel = null)
    {
        Open(errorInfo.GetTitleLocalizationTextKey(), errorInfo.GetMessageLocalizationTextKey(), onOk, onCancel);
    }

    public static void Open(LocalizationTextKey titleKey, LocalizationTextKey messageKey, LocalizationTextKey okButtonKey, Action onOk = null)
    {
        if (messageBoxStateData == null)
        {
            messageBoxStateData = new MessageBoxStateData(titleKey, messageKey, onOk);
        }
        else
        {
            messageBoxStateData.titleKey = titleKey;
            messageBoxStateData.messageKey = messageKey;
            messageBoxStateData.onOk = onOk;
            messageBoxStateData.okButtonKey = okButtonKey;
        }
        StateManager.Instance.OpenPopupState(typeof(MessageBoxState), messageBoxStateData);
    }

    public static void Open(LocalizationTextKey titleKey, LocalizationTextKey messageKey, Action onOk = null, Action onCancel = null)
    {
        if (messageBoxStateData == null)
        {
            messageBoxStateData = new MessageBoxStateData(titleKey, messageKey, onOk, onCancel);
        }
        else
        {
            messageBoxStateData.titleKey = titleKey;
            messageBoxStateData.messageKey = messageKey;
            messageBoxStateData.onOk = onOk;
            messageBoxStateData.onCancel = onCancel;
        }
        StateManager.Instance.OpenPopupState(typeof(MessageBoxState), messageBoxStateData);
    }

	public override IEnumerator OnPreOpen<T>(T args = default(T))
	{
		yield return base.OnPreOpen(args);

        MessageBoxStateData messageBoxStateData = GetData<MessageBoxStateData>();
        _title.SetText(messageBoxStateData.titleKey);
        _message.SetText(messageBoxStateData.messageKey);
        _onOK = messageBoxStateData.onOk;
        _onCancel = messageBoxStateData.onCancel;
        _okButtonText.SetText(messageBoxStateData.okButtonKey);
        _cancelButtonText.SetText(messageBoxStateData.cancelButtonKey);


        if (_onCancel == null)
		{
			_cancelButton.SetActive(false);
		}
	}

	#region EVENTS
	public void OnOK()
	{
        Debug.Log("OnOK !!");
        _onCallback = _onOK;
        OnBack();
	}

	public void OnCancel()
	{
        _onCallback = _onCancel;
        OnBack();
	}

    public override void OnPostClosed()
    {
        base.OnPostClosed();
        Debug.Log("OnPostUnload !!");
        if (_onCallback != null)
            _onCallback();
    }

    #endregion EVENTS

}