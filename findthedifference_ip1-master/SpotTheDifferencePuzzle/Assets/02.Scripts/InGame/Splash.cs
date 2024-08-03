using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Splash : MonoBehaviour
{
	[SerializeField] RectTransform _root;
	[SerializeField] RectTransform _anchorBegin;
	[SerializeField] RectTransform _anchorEnd;
	[SerializeField] RectTransform _background;
	[SerializeField] GameObject _blockScreen;
	[SerializeField] float _backgroundScaleTime = 0.3f;
	[SerializeField] float _messageMoveTime = 0.3f;
	[SerializeField] float _messageShowTime = 0.5f;

	[SerializeField] TMP_Text _messageText;
	[SerializeField] RectTransform[] _registeredMessages;

	RectTransform _messageTextRectTransform;
	Dictionary<string, RectTransform> _messages = new Dictionary<string, RectTransform>();
	Queue<RectTransform> _messageQueue = new Queue<RectTransform>();
	Queue<string> _textMessageQueue = new Queue<string>();
	Coroutine _updateCoroutine;
	
	public void Initialize()
	{
		if (_messageText != null)
		{
			_messageTextRectTransform = _messageText.GetComponent<RectTransform>();
			_messageTextRectTransform.SetParent(_root);

			_messageText.gameObject.SetActive(false);
		}

		for (int i = 0, max = _registeredMessages.Length; i < max; ++i)
		{
			RectTransform message = _registeredMessages[i];
			if (message != null)
			{
				message.SetParent(_root);
				message.gameObject.SetActive(false);

				_messages.Add(message.name.ToLower(), message);
			}
		}

		_root.gameObject.SetActive(false);

		if (_background != null)
		{
			_background.gameObject.SetActive(false);
		}

		if (_blockScreen != null)
		{
			_blockScreen.gameObject.SetActive(false);
		}
	}

	public void Release()
	{
		if (_updateCoroutine != null)
		{
			StopCoroutine(_updateCoroutine);

			_updateCoroutine = null;
		}

		FlushMessageQueue();

		_root.gameObject.SetActive(false);

		if (_background != null)
		{
			_background.LeanCancel();
			_background.gameObject.SetActive(false);
		}

		if (_blockScreen != null)
		{
			_blockScreen.gameObject.SetActive(false);
		}
	}

	public void FlushMessageQueue()
	{
		while (_messageQueue.Count > 0)
		{
			RectTransform message = _messageQueue.Dequeue();
			message.LeanCancel();
			message.gameObject.SetActive(false);
		}
	}

	public bool ShowMessage(string messageKey, bool useBlockScreen)
	{
		if (string.IsNullOrEmpty(messageKey))
		{
			return false;
		}

		messageKey = messageKey.ToLower();
		if (!_messages.ContainsKey(messageKey))
		{
			Debug.LogWarning("Not registered " + messageKey + " Message Key");
		}

		_messageQueue.Enqueue(_messages[messageKey]);

		if (_updateCoroutine == null)
		{
			_updateCoroutine = StartCoroutine(OnUpdate(useBlockScreen));
		}

		return true;
	}

	public bool ShowMessage(RectTransform message, bool useBlockScreen)
	{
		if (message == null)
		{
			return false;
		}

		if (!_messageQueue.Contains(message))
		{
			message.gameObject.SetActive(false);
			message.SetParent(_root);
			message.localScale = Vector3.one;
		}

		_messageQueue.Enqueue(message);

		if (_updateCoroutine == null)
		{
			_updateCoroutine = StartCoroutine(OnUpdate(useBlockScreen));
		}

		return true;
	}

	public bool ShowMessageText(string message, bool useBlockScreen)
	{
		if (_messageTextRectTransform == null ||
			string.IsNullOrEmpty(message))
		{
			return false;
		}

		_textMessageQueue.Enqueue(message);
		_messageQueue.Enqueue(_messageTextRectTransform);

		if (_updateCoroutine == null)
		{
			_updateCoroutine = StartCoroutine(OnUpdate(useBlockScreen));
		}

		return true;
	}

	public bool nowProgress
	{
		get
		{
			return (_updateCoroutine != null);
		}
	}

	IEnumerator OnUpdate(bool useBlockScreen)
	{
		_root.gameObject.SetActive(true);

		if (_blockScreen != null && useBlockScreen)
		{
			_blockScreen.SetActive(true);
		}

		yield return ShowBackground();

		while (_messageQueue.Count > 0)
		{
			RectTransform message = _messageQueue.Peek();

			if (message == _messageTextRectTransform && _textMessageQueue.Count > 0)
			{
				_messageText.text = _textMessageQueue.Dequeue();
			}

			message.gameObject.SetActive(true);
			message.localPosition = _anchorBegin.localPosition;
			message.localScale = Vector3.one;
			message.SetAsLastSibling();

			message.LeanMoveLocal(_root.localPosition, _messageMoveTime).
				setEaseOutBack();

			yield return new WaitForSeconds(_messageMoveTime + _messageShowTime);

			message.LeanMoveLocal(_anchorEnd.localPosition, _messageMoveTime).
				setEaseInBack();

			yield return new WaitForSeconds(_messageMoveTime);

			_messageQueue.Dequeue();
		}

		yield return HideBackground();
		
		if (_blockScreen != null)
		{
			_blockScreen.SetActive(false);
		}

		_root.gameObject.SetActive(false);

		_updateCoroutine = null;
	}

	IEnumerator ShowBackground()
	{
		if (_background == null)
		{
			yield break;
		}

		_background.gameObject.SetActive(true);

		Vector3 backgroundScale = _background.localScale;
		backgroundScale.y = 0.0f;
		_background.localScale = backgroundScale;
		
		_background.LeanCancel();
		_background.LeanScaleY(1.0f, _backgroundScaleTime).
			setEaseOutBack();

		yield return new WaitForSeconds(_backgroundScaleTime);
	}

	IEnumerator HideBackground()
	{
		if (_background == null)
		{
			yield break;
		}

		_background.LeanScaleY(0.0f, _backgroundScaleTime).
			setEaseInBack();

		yield return new WaitForSeconds(_backgroundScaleTime);

		_background.gameObject.SetActive(true);
	}
}