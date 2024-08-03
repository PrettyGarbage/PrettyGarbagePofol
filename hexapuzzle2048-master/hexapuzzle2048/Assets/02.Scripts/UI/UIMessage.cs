using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessage : MonoBehaviour
{
	protected static readonly Vector3 BEGIN_SCALE = new Vector3(1.0f, 0.0f, 1.0f);

	[SerializeField] string _soundKey;
	[SerializeField] float _time = 0.5f;

	public virtual void Initialize()
	{
		gameObject.SetActive(false);
	}

	public virtual void Show(params object[] args)
	{
		gameObject.SetActive(true);

		transform.localScale = BEGIN_SCALE;

		SoundManager.Instance.PlayUISoundInstance(_soundKey);

		gameObject.LeanCancel();

		float scaleTime = _time * 0.3f;

		LTSeq sequence = LeanTween.sequence();
		sequence.append(gameObject.LeanScale(Vector3.one, scaleTime).setEaseOutBack());
		sequence.append(_time * 0.1f);
		sequence.append(gameObject.LeanScale(BEGIN_SCALE, scaleTime).setEaseInOutBack());
		sequence.append(() => gameObject.SetActive(false));
	}

	public float time
	{
		get
		{
			return _time;
		}
	}
}
