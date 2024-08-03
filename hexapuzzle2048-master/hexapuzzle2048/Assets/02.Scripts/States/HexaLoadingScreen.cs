using UnityEngine;
using UnityEngine.UI;

public class HexaLoadingScreen : LoadingScreen
{
	[SerializeField] CanvasGroup _canvasGroup;

	public override void OnInitialize()
	{
		if (_canvasGroup == null)
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		_canvasGroup.alpha = 0.0f;
	}

    public override void OnShow(float time, float delay)
	{
		_canvasGroup.alpha = 0.0f;

		_canvasGroup.LeanCancel();
		_canvasGroup.LeanAlpha(1.0f, time).
			setDelay(delay);
	}

    public override void OnHide(float time, float delay)
	{
		_canvasGroup.LeanCancel();
		_canvasGroup.LeanAlpha(0.0f, time * _canvasGroup.alpha).
			setDelay(delay);
	}
}