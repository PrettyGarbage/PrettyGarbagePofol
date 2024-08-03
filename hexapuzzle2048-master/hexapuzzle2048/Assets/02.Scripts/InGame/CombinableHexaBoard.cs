using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using LunarConsolePlugin;

class CombineInfo
{
	public NumberedHexaTile tile { get; private set; }
	public int depth { get; private set; }

	public CombineInfo(NumberedHexaTile tile, int depth)
	{
		this.tile = tile;
		this.depth = depth;
	}
}

public class CombinableHexaBoard : HexaBoard
{
	const int COMBINE_EFFECT_BUFFER_SIZE = 6;
	readonly Vector3 CELL_MOVE_OFFSET = Vector3.back * 0.2f;

	delegate IEnumerator Combine(Stack<CombineInfo> linkedTiles);

	[Header("Combine Options")]
	[SerializeField] CombineType _combineType = CombineType.TypeB;
	[SerializeField] int _minCombineCellSize = 3;
	[SerializeField] float _cellCombineDuration = 0.5f;
	[SerializeField] float _cellCombineInterval = 0.2f;

	[Header("Effects")]
	[SerializeField] CombineEffect _combineEffectPrefab;
	[SerializeField] ParticleSystem _explodeEffectPrefab;

	[Header("Etc")]
	[SerializeField] bool _useRemoveHighestNumber = true;

	public Action<int, int, int> onCombineCells; // last number, count, combo
	public Action<Vector3, int, int> onCompleteCombine; // position, last number, score
	public Action onReachedLastNumber;

	Dictionary<CombineType, Combine> _combines = new Dictionary<CombineType, Combine>();

	SimpleComponentBuffer<CombineEffect> _combineEffectBuffer;
	SimpleComponentBuffer<ParticleSystem> _explodeEffectBuffer;

	int _score = 0;
	int _combineCount = 0;
	int _lastCombinedNumber = 0;
	Vector3 _lastCombinedPosition;

	int _highestNumber = GameConstants.LOWEST_CELL_NUMBER;

	#region UNITY EVENTS
	void Start()
	{
		LunarConsole.RegisterAction("Combine Type A", ChangeCombineTypeA);
		LunarConsole.RegisterAction("Combine Type B", ChangeCombineTypeB);
		LunarConsole.RegisterAction("Combine Type C", ChangeCombineTypeC);

		LunarVariables.combineDuration.AddDelegate(SetCellCombineDuration);
		LunarVariables.combineInterval.AddDelegate(SetCellCombineInterval);
	}

	void OnDestroy()
	{
		LunarConsole.UnregisterAllActions(this);

		LunarVariables.combineDuration.RemoveDelegate(SetCellCombineDuration);
		LunarVariables.combineInterval.RemoveDelegate(SetCellCombineInterval);
	}
	#endregion UNITY EVENTS

	public override void Initialize()
	{
		base.Initialize();

		_combines.Add(CombineType.TypeA, CombineTypeA);
		_combines.Add(CombineType.TypeB, CombineTypeB);
		_combines.Add(CombineType.TypeC, CombineTypeC);

		_combineEffectBuffer = new SimpleComponentBuffer<CombineEffect>(
			_combineEffectPrefab,
			COMBINE_EFFECT_BUFFER_SIZE,
			transform,
			e => e.Initialize()
		);

		_explodeEffectBuffer = new SimpleComponentBuffer<ParticleSystem>(
			_explodeEffectPrefab,
			Array.FindAll(tiles, t => t != null).Length,
			transform
		);
	}

	public void Clear()
	{
		Debug.Assert(tiles != null);

		for (int i = 0, max = tiles.Length; i < max; ++i)
		{
			NumberedHexaTile tile = tiles[i] as NumberedHexaTile;
			if (tile != null)
			{
				tile.Reset();
			}
		}

		_highestNumber = GameConstants.LOWEST_CELL_NUMBER;

		_combineEffectBuffer.Withdraw();
		_explodeEffectBuffer.Withdraw();
	}

	public void Setup(int[] numbers)
	{
		Debug.Assert(tiles != null);

		if (numbers == null)
		{
			Clear();			

			return;
		}

		for (int i = 0, max = Mathf.Min(tiles.Length, numbers.Length); i < max; ++i)
		{
			HexaTile tile = tiles[i];
			if (tile != null)
			{
				NumberedHexaCell cell = null;
				if (numbers[i] > 0)
				{
					cell = GameManager.instance.GenerateHexaCell() as NumberedHexaCell;
					if (cell != null)
					{
						cell.Setup(numbers[i]);
					}
				}

				tile.SetCell(cell);
			}
		}

		_highestNumber = GetCurrentHighestNumberOnBoard();

		_combineEffectBuffer.Withdraw();
		_explodeEffectBuffer.Withdraw();
	}

	public void ActiveHammerIcons(bool active)
	{
		for (int i = 0, max = tiles.Length; i < max; ++i)
		{
			NumberedHexaTile tile = tiles[i] as NumberedHexaTile;
			if (tile != null)
			{
				tile.ActivateHammer(active);
			}
		}
	}
	
	public IEnumerator CombineCells(NumberedHexaTile[] seeds)
	{
		_score = 0;
		_combineCount = 0;
		_lastCombinedNumber = 0;
		_lastCombinedPosition = Vector3.zero;

		Array.Sort(seeds, (lhs, rhs) => lhs.number.CompareTo(rhs.number));

		for (int i = 0, max = seeds.Length; i < max; ++i)
		{
			yield return ProcessCombineCell(seeds[i], seeds.Skip(i + 1).ToArray());
		}

		if (_score > 0 && onCompleteCombine != null)
		{
			onCompleteCombine(_lastCombinedPosition, _lastCombinedNumber, _score * _combineCount);
		}
	}
	
	public IEnumerator Explode(HexaTile seed, int radius, params int[] ignoreNumbers)
	{
		NumberedHexaTile[] targetTiles = FindExplodeTiles(seed, Mathf.Max(0, radius - 1), ignoreNumbers);

		for (int i = 0, max = targetTiles.Length; i < max; ++i)
		{
			NumberedHexaTile targetTile = targetTiles[i];
			targetTile.Explode(0.5f);

			ParticleSystem explodeEffect = _explodeEffectBuffer.Pop();
			explodeEffect.transform.position =
				targetTile.transform.position + Vector3.back * 0.15f;
			explodeEffect.Play();
		}

		yield return new WaitForSeconds(0.5f);

		SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_CELL_EXPLODE);

		yield return new WaitForSeconds(0.2f);
	}

	NumberedHexaTile[] FindExplodeTiles(HexaTile seed, int depth, params int[] ignoreNumbers)
	{
		if (seed == null)
		{
			return new NumberedHexaTile[0];
		}

		HexaTile[] neighbors = seed.neighbors;
		List<NumberedHexaTile> tiles = new List<NumberedHexaTile>();

		if (depth > 0)
		{
			for (int i = 0, max = neighbors.Length; i < max; ++i)
			{
				tiles.AddRange(FindExplodeTiles(neighbors[i], depth - 1, ignoreNumbers));
			}
		}

		NumberedHexaTile tile = seed as NumberedHexaTile;
		if (tile != null &&
			!tiles.Contains(tile) &&
			Array.TrueForAll(ignoreNumbers, n => n != tile.number))
		{
			tiles.Add(tile);
		}

		for (int i = 0, max = neighbors.Length; i < max; ++i)
		{
			NumberedHexaTile neighbor = neighbors[i] as NumberedHexaTile;
			if (neighbor != null &&
				!tiles.Contains(neighbor) &&
				Array.TrueForAll(ignoreNumbers, n => n != neighbor.number))
			{
				tiles.Add(neighbor);
			}
		}

		return tiles.ToArray();
	}

	public int[] numbers
	{
		get
		{
			int max = tiles.Length;
			int[] numbers = new int[max];

			for (int i = 0; i < max; ++i)
			{
				NumberedHexaTile tile = tiles[i] as NumberedHexaTile;
				if (tile != null)
				{
					numbers[i] = tile.number;
				}
			}

			return numbers;
		}
	}

	public int highestNumber
	{
		get
		{
			return _highestNumber;
		}

		set
		{
			_highestNumber = Mathf.Clamp(
				value,
				GameConstants.LOWEST_CELL_NUMBER,
				GameConstants.HIGHEST_CELL_NUMBER
			);
		}
	}

	Stack<CombineInfo> GetCombineTree(NumberedHexaTile seed, NumberedHexaTile[] ignoreTiles)
	{
		Stack<CombineInfo> linkedTiles = new Stack<CombineInfo>();

		if (seed != null && seed.hasCell)
		{
			CombineInfo seedInfo = new CombineInfo(seed, 0);
			linkedTiles.Push(seedInfo);

			BuildCombineTree(seedInfo, ignoreTiles, ref linkedTiles);
		}

		return linkedTiles;
	}

	void BuildCombineTree(CombineInfo seedInfo, NumberedHexaTile[] ignoreTiles, ref Stack<CombineInfo> stack)
	{
		HexaTile[] neighbors = seedInfo.tile.neighbors;
		for (int i = 0, max = neighbors.Length; i < max; ++i)
		{
			NumberedHexaTile neighbor = neighbors[i] as NumberedHexaTile;
			if (neighbor != null &&
				neighbor.hasCell &&
				neighbor.number == seedInfo.tile.number &&
				Array.TrueForAll(stack.ToArray(), st => st.tile != neighbor) &&
				Array.TrueForAll(ignoreTiles, it => it.number != neighbor.number))
			{
				CombineInfo neighborInfo = new CombineInfo(neighbor, seedInfo.depth + 1);

				stack.Push(neighborInfo);

				BuildCombineTree(neighborInfo, ignoreTiles, ref stack);
			}
		}
	}

	int GetCurrentHighestNumberOnBoard()
	{
		int highestNumber = GameConstants.LOWEST_CELL_NUMBER;
		for (int i = 0, max = tiles.Length; i < max; ++i)
		{
			NumberedHexaTile tile = tiles[i] as NumberedHexaTile;
			if (tile != null && tile.number > highestNumber)
			{
				highestNumber = tile.number;
			}
		}

		return highestNumber;
	}

	#region LUNAR CONSOLE
	void ChangeCombineTypeA()
	{
		_combineType = CombineType.TypeA;
	}

	void ChangeCombineTypeB()
	{
		_combineType = CombineType.TypeB;
	}

	void ChangeCombineTypeC()
	{
		_combineType = CombineType.TypeC;
	}

	void SetCellCombineDuration(CVar duration)
	{
		_cellCombineDuration = duration;
	}

	void SetCellCombineInterval(CVar interval)
	{
		_cellCombineInterval = interval;
	}
	#endregion LUNAR CONSOLE

	#region COROUTINES
	IEnumerator ProcessCombineCell(NumberedHexaTile seed, NumberedHexaTile[] ignoreTiles)
	{
		if (seed == null || !seed.hasCell)
		{
			yield break;
		}

		Stack<CombineInfo> linkedTiles = GetCombineTree(seed, ignoreTiles);
		if (linkedTiles.Count < _minCombineCellSize)
		{
			yield break;
		}

		_lastCombinedNumber = seed.number;
		_lastCombinedPosition = seed.transform.position;

		_score += (int)Mathf.Pow(2, seed.number) * linkedTiles.Count;

		Combine combine = _combines[_combineType];

		yield return combine(linkedTiles);

		if (onCombineCells != null)
		{
			onCombineCells(_lastCombinedNumber, linkedTiles.Count, _combineCount);
		}

		if (_combineCount > 0)
		{
			int index = Mathf.Min(_combineCount, GameConstants.FX_COMBO.Length) - 1;
			SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_COMBO[index]);
		}

		seed.cell.LeanScale(Vector3.zero, _cellCombineInterval * 0.5f).
			setEaseInBack();

		yield return new WaitForSeconds(_cellCombineInterval * 0.5f);

		CombineEffect combineEffect = _combineEffectBuffer.Pop();
		combineEffect.transform.position = seed.transform.position + Vector3.back * 0.1f;
		combineEffect.SetColor(GameManager.instance.theme.GetCellSprite(seed.number).color);

		seed.Increase();

		seed.cell.LeanScale(Vector3.one, _cellCombineInterval * 0.5f).
			setEaseOutBack();

		yield return new WaitForSeconds(_cellCombineInterval * 0.5f);

		++_combineCount;

		if (seed.number <= GameConstants.HIGHEST_CELL_NUMBER)
		{
			if (seed.number > _highestNumber)
			{
				_highestNumber = seed.number;
			}

			SoundManager.Instance.PlayUISoundInstance(
				(seed.number < GameConstants.HIGHEST_CELL_NUMBER) ? 
					GameConstants.FX_CELL_COMBINED :
					GameConstants.FX_CELL_COMBINED_LAST_NUMBER
			);

			yield return ProcessCombineCell(seed, ignoreTiles);
		}
		else
		{
			SoundManager.Instance.PlayUISoundInstance(
				GameConstants.FX_CELL_COMBINED_LAST_NUMBER
			);

			yield return new WaitForSeconds(0.25f);

			yield return Explode(seed, 1);

			if (onReachedLastNumber != null)
			{
				onReachedLastNumber();
			}

			if (_useRemoveHighestNumber)
			{
				_highestNumber = GetCurrentHighestNumberOnBoard();
			}
		}
	}

	IEnumerator CombineTypeA(Stack<CombineInfo> linkedTileInfos)
	{
		HexaTile seed = linkedTileInfos.ToArray()[linkedTileInfos.Count - 1].tile;
		Vector3 to = seed.transform.position + CELL_MOVE_OFFSET;

		while (linkedTileInfos.Count > 1)
		{
			NumberedHexaTile linkedTile = linkedTileInfos.Pop().tile;
			HexaCell cell = linkedTile.cell;
			Vector3 from = cell.transform.position + CELL_MOVE_OFFSET;
			cell.LeanMove(from, to, _cellCombineDuration).
				setEaseInBack().
				setOnComplete(() => linkedTile.Reset());
		}

		SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_CELL_COMBINING);

		yield return new WaitForSeconds(_cellCombineDuration);
	}

	IEnumerator CombineTypeB(Stack<CombineInfo> linkedTileInfos)
	{
		float durationPerCell = _cellCombineDuration / linkedTileInfos.Count;

		while (linkedTileInfos.Count > 1)
		{
			NumberedHexaTile linkedTile = linkedTileInfos.Pop().tile;
			NumberedHexaTile targetTile = Array.Find(
				linkedTileInfos.ToArray(),
				ci => ci.tile.IsNeighbor(linkedTile)).tile;

			HexaCell cell = linkedTile.cell;
			Vector3 from = cell.transform.position + CELL_MOVE_OFFSET;
			Vector3 to = targetTile.transform.position + CELL_MOVE_OFFSET;
			cell.LeanMove(from, to, durationPerCell).
				setEaseInBack().
				setOnComplete(() => {
					SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_CELL_COMBINING);
					linkedTile.Reset();
				});

			yield return new WaitForSeconds(durationPerCell);
		}
	}

	IEnumerator CombineTypeC(Stack<CombineInfo> linkedTileInfos)
	{
		int maxDepth = 0;
		CombineInfo[] listInfos = linkedTileInfos.ToArray();
		for (int i = 0, max = listInfos.Length; i < max; ++i)
		{
			if (listInfos[i].depth > maxDepth)
			{
				maxDepth = listInfos[i].depth;
			}
		}

		float durationPerDepth = _cellCombineDuration / maxDepth;

		while (linkedTileInfos.Count > 1)
		{
			CombineInfo info = linkedTileInfos.Pop();
			NumberedHexaTile linkedTile = info.tile;
			NumberedHexaTile targetTile = Array.Find(
				linkedTileInfos.ToArray(),
				ci => ci.tile.IsNeighbor(linkedTile)).tile;

			HexaCell  cell = linkedTile.cell;
			Vector3 from = cell.transform.position + CELL_MOVE_OFFSET;
			Vector3 to = targetTile.transform.position + CELL_MOVE_OFFSET;
			cell.LeanMove(from, to, durationPerDepth).
				setEaseInBack().
				setDelay((maxDepth - info.depth) * durationPerDepth).
				setOnComplete(() => linkedTile.Reset());
		}

		for (int i = 0; i < maxDepth; ++i)
		{
			yield return new WaitForSeconds(durationPerDepth);

			SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_CELL_COMBINING);
		}
	}
	#endregion COROUTINES
}