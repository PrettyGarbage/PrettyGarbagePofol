using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class PopupState : State
{
	[Header("Default")]
	[SerializeField] protected GameObject _popup;
    protected float _popTime = 0.2f;
	[SerializeField] protected LeanTweenType _showEase = LeanTweenType.easeOutBack;
	[SerializeField] protected LeanTweenType _hideEase = LeanTweenType.easeInBack;

	[Header("Sounds")]
    [SerializeField] protected AudioDataKey _sfxOpenKey = AudioDataKey.ui_popup_open;
    [SerializeField] protected AudioDataKey _sfxCloseKey = AudioDataKey.ui_popup_close;

    [Header("Events")]
	public UnityEvent onClose = new UnityEvent();

    private Vector3 _transitionOriginScale =  Vector3.one * 0.8f;

	public override IEnumerator OnInitialize()
	{
        _stateType = StateType.POPUP;
        _popup.transform.localScale = _transitionOriginScale;
        
		yield return null;
	}

	public override IEnumerator OnOpening()
	{
        
        _popup.transform.localScale = _transitionOriginScale;
        
        SoundManager.Instance.PlayUISoundInstance(_sfxOpenKey);
        LTDescr ltDescr = LeanTween.scale(_popup, Vector3.one, _popTime).setEase(_showEase);

        while (LeanTween.isTweening(ltDescr.uniqueId))
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
        
    }

	public override IEnumerator OnClosing()
	{
        if (StateManager.Instance.StateManagerStatus == StateManagerStatus.CLOSE_STATE)
        {

            SoundManager.Instance.PlayUISoundInstance(_sfxCloseKey);
            
            yield return null;
        }        
    }

	#region EVENTS
	public override void OnBack()
	{
        onClose.Invoke();
        StateManager.Instance.CloseState(GetType());
    }
	#endregion EVENTS
}