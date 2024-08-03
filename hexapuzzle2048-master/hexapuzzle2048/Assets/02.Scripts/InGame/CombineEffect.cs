using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineEffect : MonoBehaviour
{
	[SerializeField] SpriteRenderer _spriteRenderer;

	bool _initialized = false;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(1.0f);

		Initialize();
		SetColor(Color.red);
	}

	public void SetColor(Color color)
	{
		if (_spriteRenderer != null)
		{
			_spriteRenderer.color = color;
		}
	}

	public void Initialize()
	{
		if (_initialized)
		{
			return;
		}

		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		_initialized = true;
	}

	
}