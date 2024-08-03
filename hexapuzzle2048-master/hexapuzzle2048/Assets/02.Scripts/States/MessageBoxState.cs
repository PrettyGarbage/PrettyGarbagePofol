using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class MessageBoxState : PopupState
{
	public const string PARAM_TITLE = "Title";
	public const string PARAM_MESSAGE = "Message";
	public const string PARAM_CALLBACK_OK = "CallbackOK";
	public const string PARAM_CALLBACK_CANCEL = "CallbackCancel";

	[Header("Message Box")]
	[SerializeField] TMP_Text _title;
	[SerializeField] TMP_Text _message;
	[SerializeField] GameObject _OkButton;
	[SerializeField] GameObject _cancelButton;

	Action _onOK;
	Action _onCancel;

	public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_title.text = GetParam(PARAM_TITLE, "");
		_message.text = GetParam(PARAM_MESSAGE, "");
		_onOK = GetParam(PARAM_CALLBACK_OK, (Action)null);
		_onCancel = GetParam(PARAM_CALLBACK_CANCEL, (Action)null);

		if (_onCancel == null)
		{
			_cancelButton.SetActive(false);
		}
	}

	public override string stateName
	{
		get
		{
			return "Message Box";
		}
	}

	#region EVENTS
	public void OnOK()
	{
		if (_onOK != null)
		{
			_onOK();
		}

		OnBack();
	}

	public void OnCancel()
	{
		if (_onCancel != null)
		{
			_onCancel();
		}

		OnBack();
	}
	#endregion EVENTS

	#region UTILITIES
	public static void ShowWorkingOnMessage()
	{
		ShowMessageBox("Notice", "Sorry. We will work hard and add features soon.");
	}

	public static void ShowMessageBox(string title, string message, Action onOK = null, Action onCancel = null)
	{
		Dictionary<string, object> args = new Dictionary<string, object>() {
			{ PARAM_TITLE, title },
			{ PARAM_MESSAGE, message },
			{ PARAM_CALLBACK_OK, onOK },
			{ PARAM_CALLBACK_CANCEL, onCancel }
		};

		StateManager.instance.PushState("Message Box", args);
	}
	#endregion UTILITIES
}