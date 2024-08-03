using UnityEngine;
using UnityEngine.UI;

public class UISoundEvent : MonoBehaviour
{
	[SerializeField] Button _button;
	[SerializeField] string _clipName;

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
		SoundManager.Instance.PlayUISoundInstance(_clipName);
	}
}