using UnityEngine;
using UnityEngine.UI;

public class UISoundEvent : MonoBehaviour
{
	[SerializeField] Button _button;
    [SerializeField] AudioDataKey _audioDataKey = AudioDataKey.ui_button_common;

    void Awake()
	{
		if (_button == null)
		{
			_button = GetComponent<Button>();
		}

		if (_button != null)
		{
			_button.onClick.AddListener(OnClick);
		}
	}

	void OnClick()
	{
		SoundManager.Instance.PlayUISoundInstance(_audioDataKey);
	}
}