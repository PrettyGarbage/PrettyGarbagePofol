using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaCell : Hexagon
{
	[SerializeField] SpriteRenderer _spriteRenderer;
	[SerializeField] Sprite _defaultSprite;

	#region UNITY EVENTS
    protected virtual void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

		if (_defaultSprite == null && _spriteRenderer != null)
		{
			_defaultSprite = _spriteRenderer.sprite;
		}
    }
    #endregion UNITY EVENTS

	public virtual void Initialize()
    {
    }

    public virtual void Release()
    {
    }

	public virtual void Placed()
    {
    }

	public virtual bool IsEqual(HexaCell cell)
	{
		return true;
	}

	public SpriteRenderer spriteRenderer
	{
		get
		{
			return _spriteRenderer;
		}
	}

	public Sprite defaultSprite
	{
		get
		{
			return _defaultSprite;
		}
	}
}