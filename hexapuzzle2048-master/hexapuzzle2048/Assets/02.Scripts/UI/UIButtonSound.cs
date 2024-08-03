using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : UISound
{
	[SerializeField] Button _button;
	[SerializeField] string _key;

	bool _initialized = false;

	void OnEnable()
	{
		if (!_initialized)
		{
			if (_button == null)
			{
				_button = GetComponent<Button>();
			}

			_initialized = true;
		}

		if (_button != null)
		{
			_button.onClick.AddListener(PlayUISound);
		}
	}

	void OnDisable()
	{
		if (_button != null)
		{
			_button.onClick.RemoveListener(PlayUISound);
		}
	}

	void PlayUISound()
	{
		PlayUISound(_key);
	}
}