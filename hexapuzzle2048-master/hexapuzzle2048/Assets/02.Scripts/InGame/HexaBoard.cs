using System;
using System.Collections.Generic;

using UnityEngine;

public class HexaBoard : MonoBehaviour
{
	public enum Type
	{
		Hexagon,
		Rectangle,
	}

	public enum VerticalAlign
	{
		Upper,
		Middle,
		Lower,
	}

	public enum HorizontalAlign
	{
		Left,
		Center,
		Right,
	}

	public enum CombineType
	{
		TypeA,
		TypeB,
		TypeC,
	}

	[Header("Tile")]
	[SerializeField] HexaTile _tilePrefab;

	[Header("Board")]
	[SerializeField] Type _type = Type.Hexagon;
	[SerializeField] HorizontalAlign _horizontalAlign = HorizontalAlign.Left;
	[SerializeField] VerticalAlign _verticalAlign = VerticalAlign.Upper;
	[SerializeField] float _spacing = 0.05f;
	[SerializeField] int _hexagonDiameter = 7;
	[SerializeField] int _rectangleWidth = 5;
	[SerializeField] int _rectangleHeight = 5;

	[Header("Options")]
	[SerializeField] bool _hideWhenAwake = true;

	[Header("Information")]
	[SerializeField] int _boardWidth;
	[SerializeField] int _boardHeight;
	[SerializeField] HexaTile[] _tiles;

	#region UNITY EVENTS
	void Awake()
	{
		gameObject.SetActive(!_hideWhenAwake);
	}

	void OnDrawGizmos()
	{
		if (_tilePrefab == null)
		{
			return;
		}

		if (isPointyBoard)
		{
			DrawPointyBoardGizmos();
		}
		else
		{
			DrawFlatBoardGizmos();
		}
	}
	#endregion UNITY EVENTS

	public virtual void Initialize()
	{
		for (int i = 0, max = _tiles.Length; i < max; ++i)
		{
			HexaTile tile = _tiles[i];
			if (tile != null)
			{
				tile.Initialize();
			}
		}
	}

	public float GetTileFlatOffsetWithSpacing()
	{
		return Hexagon.GetInnerRadius(_tilePrefab.outerRadius + _spacing * 0.5f);
	}

	public float GetTilePointyOffsetWithSpacing()
	{
		return Hexagon.GetPointyOffset(_tilePrefab.outerRadius + _spacing * 0.5f);
	}

	public Type type
	{
		get
		{
			return _type;
		}
	}

	public HexaTile[] tiles
	{
		get
		{
			return _tiles;
		}
	}

	public int width
	{
		get
		{
			return _boardWidth;
		}
	}

	public int height
	{
		get
		{
			return _boardHeight;
		}
	}

	public HexaTile[] emptyTiles
	{
		get
		{
			return Array.FindAll(_tiles, t => t != null && t.cell == null);
		}
	}

	public bool hasCells
	{
		get
		{
			return (Array.Find(_tiles, t => t != null && t.cell != null) != null);
		}
	}

	#region EDITOR
	public void GenerateTiles()
	{
		if (_tilePrefab == null)
		{
			ClearTiles();

			return;
		}

		Queue<HexaTile> buffer = (_tiles != null) ?
			new Queue<HexaTile>(Array.FindAll(_tiles, t => t != null)) :
			new Queue<HexaTile>();

		if (_type == Type.Hexagon)
		{
			if (isPointyBoard)
			{
				GeneratePointyHexagonBoard(buffer);
			}
			else
			{
				GenerateFlatHexagonBoard(buffer);
			}
		}
		else
		{
			if (isPointyBoard)
			{
				GeneratePointyRectangleBoard(buffer);
			}
			else
			{
				GenerateFlatRectangleBoard(buffer);
			}
		}

		while (buffer.Count > 0)
		{
			DestroyImmediate(buffer.Dequeue().gameObject);
		}
	}

	public void ClearTiles()
	{
		for (int i = 0, max = _tiles.Length; i < max; ++i)
		{
			if (_tiles[i] != null)
			{
				DestroyImmediate(_tiles[i].gameObject);
			}
		}

		_tiles = new HexaTile[0];
		_boardWidth = 0;
		_boardHeight = 0;
	}

	public HexaTile tilePrefab
	{
		get
		{
			return _tilePrefab;
		}
	}

	public int radialDiameter
	{
		get
		{
			return _hexagonDiameter;
		}
	}

	public int rectangleWidth
	{
		get
		{
			return _rectangleWidth;
		}
	}

	public int rectangleHeight
	{
		get
		{
			return _rectangleHeight;
		}
	}

	public float spacing
	{
		get
		{
			return _spacing;
		}	
	}

	void GeneratePointyHexagonBoard(Queue<HexaTile> buffer)
	{
		_boardWidth = _hexagonDiameter;
		_boardHeight = GetPointyHexagonBoardSize();
		_tiles = new HexaTile[_boardHeight * _boardWidth];

		int halfBoardHeight = Mathf.FloorToInt(_boardHeight * 0.5f);
		int extraIndex = Mathf.Abs((halfBoardHeight % 2) - 1);
		float width = GetTileFlatOffsetWithSpacing();
		float height = GetTilePointyOffsetWithSpacing();
		Vector3 boardOffset = GetPointyHexagonBoardAlignOffset();

		for (int i = 0; i < _boardHeight; ++i)
		{
			int beginIndex = Mathf.Abs(i - halfBoardHeight);
			float evenLineOffset = (i % 2) * width;
			for (int j = beginIndex; j < _boardWidth; ++j)
			{
				int x = j - Mathf.FloorToInt(beginIndex * 0.5f) - extraIndex * (i % 2);

				Vector3 position = boardOffset;
				position.x += width * 2.0f * x + evenLineOffset;
				position.y -= height * i;

				SetTile(buffer, position, x, i);
			}
		}
		
		LinkPointyTiles();
	}

	void GenerateFlatHexagonBoard(Queue<HexaTile> buffer)
	{
		_boardWidth = GetPointyHexagonBoardSize();
		_boardHeight = _hexagonDiameter;
		_tiles = new HexaTile[_boardHeight * _boardWidth];

		int halfBoardWidth = Mathf.FloorToInt(_boardWidth * 0.5f);
		int extraIndex = Mathf.Abs((halfBoardWidth % 2) - 1);
		float width = GetTilePointyOffsetWithSpacing();
		float height = GetTileFlatOffsetWithSpacing();
		Vector3 boardOffset = GetFlatHexagonBoardAlignOffset();

		for (int i = 0; i < _boardWidth; ++i)
		{
			int beginIndex = Mathf.Abs(i - halfBoardWidth);
			float evenLineOffset = (i % 2) * height;
			for (int j = beginIndex; j < _boardHeight; ++j)
			{
				int y = j - Mathf.FloorToInt(beginIndex * 0.5f) - extraIndex * (i % 2);

				Vector3 position = boardOffset;
				position.x += width * i;
				position.y -= height * 2.0f * y + evenLineOffset;

				SetTile(buffer, position, i, y);
			}
		}

		LinkFlatTiles();
	}

	void GeneratePointyRectangleBoard(Queue<HexaTile> buffer)
	{
		_boardWidth = _rectangleWidth;
		_boardHeight = _rectangleHeight;
		_tiles = new HexaTile[_boardHeight * _boardWidth];

		float width = GetTileFlatOffsetWithSpacing();
		float height = GetTilePointyOffsetWithSpacing();
		Vector3 boardOffset = GetPointyRectangleBoardAlignOffset();

		for (int i = 0; i < _boardHeight; ++i)
		{
			float evenLineOffset = (i % 2) * width;
			for (int j = 0; j < _boardWidth; ++j)
			{
				Vector3 position = boardOffset;
				position.x += width * 2.0f * j + evenLineOffset;
				position.y -= height * i;

				SetTile(buffer, position, j, i);
			}
		}

		LinkPointyTiles();
	}

	void GenerateFlatRectangleBoard(Queue<HexaTile> buffer)
	{
		_boardWidth = _rectangleWidth;
		_boardHeight = _rectangleHeight;
		_tiles = new HexaTile[_boardHeight * _boardWidth];

		float width = GetTilePointyOffsetWithSpacing();
		float height = GetTileFlatOffsetWithSpacing();
		Vector3 boardOffset = GetFlatRectangleBoardAlignOffset();

		for (int i = 0; i < _boardHeight; ++i)
		{
			for (int j = 0; j < _boardWidth; ++j)
			{
				Vector3 position = boardOffset;
				position.x += width * j;
				position.y -= height * 2.0f * i + (j % 2) * height;

				SetTile(buffer, position, j, i);
			}
		}

		LinkFlatTiles();
	}

	void LinkPointyTiles()
	{
		for (int i = 0; i < _boardHeight; ++i)
		{
			for (int j = 0; j < _boardWidth; ++j)
			{
				HexaTile tile = GetTile(j, i);
				if (tile != null)
				{
					int x = j - Mathf.Abs((i % 2) - 1);

					tile.SetNeighbor(0, GetTile(x + 0, i - 1));
					tile.SetNeighbor(1, GetTile(j - 1, i + 0));
					tile.SetNeighbor(2, GetTile(x + 0, i + 1));
					tile.SetNeighbor(3, GetTile(x + 1, i + 1));
					tile.SetNeighbor(4, GetTile(j + 1, i + 0));
					tile.SetNeighbor(5, GetTile(x + 1, i - 1));
				}
			}
		}
	}

	void LinkFlatTiles()
	{
		for (int i = 0; i < _boardHeight; ++i)
		{
			for (int j = 0; j < _boardWidth; ++j)
			{
				HexaTile tile = GetTile(j, i);
				if (tile != null)
				{
					int y = i - Mathf.Abs((j % 2) - 1);

					tile.SetNeighbor(0, GetTile(j + 0, i - 1));
					tile.SetNeighbor(1, GetTile(j - 1, y + 0));
					tile.SetNeighbor(2, GetTile(j - 1, y + 1));
					tile.SetNeighbor(3, GetTile(j + 0, i + 1));
					tile.SetNeighbor(4, GetTile(j + 1, y + 1));
					tile.SetNeighbor(5, GetTile(j + 1, y + 0));
				}
			}
		}
	}

	void SetTile(Queue<HexaTile> buffer, Vector3 position, int x, int y)
	{
		HexaTile tile = (buffer.Count > 0) ? buffer.Dequeue() : Instantiate(_tilePrefab);

		tile.gameObject.layer = LayerMask.NameToLayer("Tile");
		tile.transform.SetParent(transform);
		tile.transform.localScale = Vector3.one;
		tile.transform.localRotation = Quaternion.identity;
		tile.transform.localPosition = position;

		tile.SetCoords(x, y);
		tile.AddCollier();

		_tiles[y * _boardWidth + x] = tile;
	}

	HexaTile GetTile(int x, int y)
	{
		return (x >= 0 && x < _boardWidth && y >= 0 && y < _boardHeight) ? _tiles[y * _boardWidth + x] : null;
	}

	void DrawPointyBoardGizmos()
	{
		float width = GetTileFlatOffsetWithSpacing();
		float height = GetTilePointyOffsetWithSpacing();

		Vector3 boardOffset = transform.position;
		if (_type == Type.Hexagon)
		{
			int boardHeight = GetPointyHexagonBoardSize();
			int boardWidthOffset = (Mathf.FloorToInt(boardHeight * 0.5f) % 2);
			boardWidthOffset -= Mathf.Abs((Mathf.CeilToInt(boardHeight * 0.5f) % 2) - 1);
			boardOffset.x += boardWidthOffset * width;

			boardOffset += GetPointyHexagonBoardAlignOffset();
		}
		else
		{
			boardOffset += GetPointyRectangleBoardAlignOffset();
		}

		for (int i = 0; i < _boardHeight; ++i)
		{
			float lineOffset = (i % 2) * width;
			for (int j = 0; j < _boardWidth; ++j)
			{
				Vector3 position = boardOffset;
				position.x += width * 2.0f * j + lineOffset;
				position.y -= height * i;

				GizmosUtilities.DrawWireHexagon(
					position,
					Hexagon.Orientation.Pointy,
					_tilePrefab.outerRadius,
					Color.magenta
				);

				GizmosUtilities.DrawString(
					string.Format("{0}, {1}", i, j),
					position
				);
			}
		}
	}
	void DrawFlatBoardGizmos()
	{
		float width = GetTilePointyOffsetWithSpacing();
		float height = GetTileFlatOffsetWithSpacing();

		Vector3 boardOffset = transform.position;
		if (_type == Type.Hexagon)
		{
			int boardWidth = GetPointyHexagonBoardSize();
			int boardHeightOffset = (Mathf.FloorToInt(boardWidth * 0.5f) % 2);
			boardHeightOffset -= Mathf.Abs((Mathf.CeilToInt(boardWidth * 0.5f) % 2) - 1);
			boardOffset.y += boardHeightOffset * height;

			boardOffset += GetFlatHexagonBoardAlignOffset();
		}
		else
		{
			boardOffset += GetFlatRectangleBoardAlignOffset();
		}

		for (int i = 0; i < _boardHeight; ++i)
		{
			for (int j = 0; j < _boardWidth; ++j)
			{
				Vector3 position = boardOffset;
				position.x += width * j;
				position.y -= height * 2.0f * i + (j % 2) * height;

				GizmosUtilities.DrawWireHexagon(
					position,
					Hexagon.Orientation.Flat,
					_tilePrefab.outerRadius,
					Color.magenta
				);

				GizmosUtilities.DrawString(
					string.Format("{0}, {1}", i, j),
					position
				);
			}
		}
	}

	float GetPointySideSize(int count)
	{
		return GetTilePointyOffsetWithSpacing() * (count - 1);
	}

	float GetFlatSideSize(int count)
	{
		return GetTileFlatOffsetWithSpacing() * (count - 1) * 2;
	}

	Vector3 GetPointyHexagonBoardAlignOffset()
	{
		int boardHeight = GetPointyHexagonBoardSize();

		float width = 0.0f;
		if (_horizontalAlign != HorizontalAlign.Left)
		{
			width = GetFlatSideSize(_hexagonDiameter);
			if (_horizontalAlign == HorizontalAlign.Center)
			{
				width *= 0.5f;
			}
		}

		if (_hexagonDiameter > 1 &&
			((boardHeight - boardHeight / 2) % 2) != 1)
		{
			width += GetTileFlatOffsetWithSpacing();
		}
		
		float height = 0.0f;
		if (_verticalAlign != VerticalAlign.Upper)
		{
			height = GetPointySideSize(boardHeight);
			if (_verticalAlign == VerticalAlign.Middle)
			{
				height *= 0.5f;
			}
		}

		return new Vector3(-width, height);
	}

	Vector3 GetFlatHexagonBoardAlignOffset()
	{
		int boardWidth = GetPointyHexagonBoardSize();
		
		float width = 0.0f;
		if (_horizontalAlign != HorizontalAlign.Left)
		{
			width = GetPointySideSize(boardWidth);
			if (_horizontalAlign == HorizontalAlign.Center)
			{
				width *= 0.5f;
			}
		}

		float height = 0.0f;
		if (_verticalAlign != VerticalAlign.Upper)
		{
			height = GetFlatSideSize(_hexagonDiameter);
			if (_verticalAlign == VerticalAlign.Middle)
			{
				height *= 0.5f;
			}
		}

		if (_hexagonDiameter > 1 &&
			((boardWidth - boardWidth / 2) % 2) != 1)
		{
			height += GetTileFlatOffsetWithSpacing();
		}

		return new Vector3(-width, height);
	}

	Vector3 GetPointyRectangleBoardAlignOffset()
	{
		float width = 0.0f;
		if (_horizontalAlign != HorizontalAlign.Left)
		{
			width = GetFlatSideSize(_rectangleWidth);
			if (_rectangleHeight > 1)
			{
				width += GetTileFlatOffsetWithSpacing();
			}

			if (_horizontalAlign == HorizontalAlign.Center)
			{
				width *= 0.5f;
			}
		}

		float height = 0.0f;
		if (_verticalAlign != VerticalAlign.Upper)
		{
			height = GetPointySideSize(_rectangleHeight);
			if (_verticalAlign == VerticalAlign.Middle)
			{
				height *= 0.5f;
			}
		}

		return new Vector3(-width, height);
	}

	Vector3 GetFlatRectangleBoardAlignOffset()
	{
		float width = 0.0f;
		if (_horizontalAlign != HorizontalAlign.Left)
		{
			width = GetPointySideSize(_rectangleWidth);
			if (_horizontalAlign == HorizontalAlign.Center)
			{
				width *= 0.5f;
			}
		}

		float height = 0.0f;
		if (_verticalAlign != VerticalAlign.Upper)
		{
			height = GetFlatSideSize(_rectangleHeight);
			if (_verticalAlign == VerticalAlign.Middle)
			{
				height *= 0.5f;
			}

			if (_rectangleWidth > 1)
			{
				height += GetTileFlatOffsetWithSpacing() * 0.5f;
			}
		}

		return new Vector3(-width, height);
	}

	int GetPointyHexagonBoardSize()
	{
		return (_hexagonDiameter % 2 == 0) ? _hexagonDiameter + 1 : _hexagonDiameter;
	}

	bool isPointyBoard
	{
		get
		{
			return (_tilePrefab.orientation == Hexagon.Orientation.Pointy);
		}
	}
	#endregion EDITOR
}