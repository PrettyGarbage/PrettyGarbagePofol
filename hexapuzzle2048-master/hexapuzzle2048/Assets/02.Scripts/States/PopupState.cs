using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class PopupState : State
{
	[Header("Default")]
	[SerializeField] GameObject _popup;
	[SerializeField] float _popTime = 0.3f;
	[SerializeField] LeanTweenType _showEase = LeanTweenType.easeOutBack;
	[SerializeField] LeanTweenType _hideEase = LeanTweenType.easeInBack;

	[Header("Sounds")]
	[SerializeField] string _fxOpen = "ui_popup_open";
	[SerializeField] string _fxClose = "ui_popup_close";

	[Header("Events")]
	public UnityEvent onClose = new UnityEvent();

	public override IEnumerator OnInitialize()
	{
		_popup.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);

		yield return null;
	}

	public override IEnumerator OnEnter()
	{
		yield return new WaitForSeconds(0.1f);

		LeanTween.scaleY(_popup, 1.0f, _popTime).
			setEase(_showEase).
			setIgnoreTimeScale(true);

		SoundManager.Instance.PlayUISoundInstance(_fxOpen);

		yield return new WaitForSeconds(_popTime);
	}

	public override IEnumerator OnExit()
	{
		yield return new WaitForSeconds(0.1f);

		LeanTween.scaleY(_popup, 0.0f, _popTime).
			setEase(_hideEase).
			setIgnoreTimeScale(true);

		SoundManager.Instance.PlayUISoundInstance(_fxClose);

		yield return new WaitForSeconds(_popTime);
	}

	#region EVENTS
	public override void OnBack()
	{
		onClose.Invoke();

		base.OnBack();
	}
	#endregion EVENTS
}