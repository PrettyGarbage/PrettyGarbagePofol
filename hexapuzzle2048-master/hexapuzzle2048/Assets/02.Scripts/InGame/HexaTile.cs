using System;

using UnityEngine;

public class HexaTile : Hexagon
{
    public const int INVALID_DIRECTION = -1;

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] HexaTile[] _neighbors = new HexaTile[6];

    public int x { get; private set; }
	public int y { get; private set; }
	public HexaCell cell { get; private set; }

    public virtual void Initialize()
	{
	}

	public virtual void Release()
	{
	}

    public virtual bool SetCell(HexaCell cell)
	{
		if (this.cell == cell)
		{
			return false;
		}

		if (this.cell != cell)
		{
			GameManager.instance.ReleaseHexaCell(this.cell);
		}

		this.cell = cell;

		if (cell != null)
		{
			cell.transform.SetParent(transform);
			cell.transform.localScale = Vector3.one;
			cell.transform.localPosition = Vector3.back * 0.1f;
			cell.transform.localRotation = Quaternion.identity;

			cell.Placed();
		}

        return true;
	}

    public int GetNeighborDirection(HexaTile tile)
	{
		return (tile != null) ? Array.FindIndex(_neighbors, n => n == tile) : INVALID_DIRECTION;
	}

	public bool IsNeighbor(HexaTile tile)
	{
		return (GetNeighborDirection(tile) != INVALID_DIRECTION);
	}

    public SpriteRenderer spriteRenderer
    {
        get
        {
			if (_spriteRenderer == null)
			{
				_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			}

            return _spriteRenderer;
        }
    }

    public HexaTile[] neighbors
	{
		get
		{
			return _neighbors;
		}
	}

    public bool hasEmptyLinkedTile
	{
		get
		{
			return (Array.Find(_neighbors, n => n != null && !n.hasCell) != null);
		}
	}

	public bool hasCell
	{
		get
		{
			return (cell != null);
		}
	}

    #region EDITOR
	public void SetCoords(int x, int y)
	{
		this.x = x;
		this.y = y;
		
		name = string.Format("Tile_{1:D2}_{0:D2}", x, y);

		_neighbors = new HexaTile[6];
	}

	public void SetNeighbor(int direction, HexaTile neighbor)
	{
		_neighbors[direction % _neighbors.Length] = neighbor;
	}

	public HexaTile GetNeighbor(int direction)
	{
		return _neighbors[direction % _neighbors.Length];
	}
	#endregion EDITOR
}