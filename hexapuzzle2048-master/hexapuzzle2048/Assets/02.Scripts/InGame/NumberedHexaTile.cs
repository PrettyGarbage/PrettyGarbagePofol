using System;

using UnityEngine;

public class NumberedHexaTile : HexaTile
{
	[SerializeField] SpriteRenderer _explodeRenderer;
	[SerializeField] GameObject _hammer;

	public override void Initialize()
	{
		_hammer.SetActive(false);

		OnSwapTheme(GameManager.instance.theme);

		GameManager.instance.onThemeChanged += OnSwapTheme;
	}

	public override void Release()
	{
		GameManager.instance.onThemeChanged -= OnSwapTheme;
	}

	public void Reset()
	{
		_explodeRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		SetCell(null);
	}

	public override bool SetCell(HexaCell cell)
	{
		if (!base.SetCell(cell))
		{
			return false;
		}

		Unhovered();

		return true;
	}

	public void Hovered(int number)
	{
		if (spriteRenderer == null)
		{
			return;
		}

		spriteRenderer.sprite = GameManager.instance.theme.GetCellSprite(number).sprite;
		spriteRenderer.color = Color.white * 0.7f;
	}

	public void Unhovered()
	{
		if (spriteRenderer == null)
		{
			return;
		}

		spriteRenderer.sprite = GameManager.instance.theme.tileSprite;
		spriteRenderer.color = Color.white;
	}

	public void ActivateHammer(bool active)
	{
		if (cell == null)
		{
			return;
		}

		_hammer.SetActive(active);
	}

	public void Increase()
	{
		NumberedHexaCell numberedCell = cell as NumberedHexaCell;
		if (numberedCell == null)
		{
			return;
		}

		numberedCell.Increase();
	}

	public void Explode(float duration)
	{
		_explodeRenderer.gameObject.LeanAlpha(1.0f, duration).
			setOnComplete(() => Reset());
	}

	public int number
	{
		get
		{
			if (!hasCell)
			{
				return 0;
			}

			NumberedHexaCell numberedCell = cell as NumberedHexaCell;

			return numberedCell.number;
		}
	}

	void OnSwapTheme(BoardTheme theme)
	{
		if (theme == null)
		{
			return;
		}

		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = theme.tileSprite;
		}
		
		_explodeRenderer.sprite = theme.explosionSprite;
	}
}