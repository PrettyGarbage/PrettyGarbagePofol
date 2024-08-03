using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITrashCan : MonoBehaviour
{
	const string ANIM_PARAM_OPEN = "Open";

	[SerializeField] Button _button;
	[SerializeField] GameObject _enabled;
	[SerializeField] GameObject _disabled;
	[SerializeField] Animator _animator;

	bool _interactable = true;

	public void Load()
	{
		_animator.SetBool(ANIM_PARAM_OPEN, false);
	}

	public void Unload()
	{
	}

	public void Open()
	{
		_animator.SetBool(ANIM_PARAM_OPEN, true);
	}

	public void Close()
	{
		_animator.SetBool(ANIM_PARAM_OPEN, false);
	}

	public bool interactable
	{
		get
		{
			return _interactable;
		}

		set
		{
			if (_interactable != value)
			{
				_animator.SetBool(ANIM_PARAM_OPEN, false);
			}

			_interactable = value;
			
			_button.interactable = value;

			_enabled.SetActive(_interactable);
			_disabled.SetActive(!_interactable);
		}
	}
}