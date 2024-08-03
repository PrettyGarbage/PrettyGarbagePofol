using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using LunarConsolePlugin;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public enum Transition
	{
		None,
		Combine,
		Restore,
		Hammer,
	}

	public static int TILE_LAYER { get; private set; }
	public static int BLOCK_LAYER { get; private set; }

	#region SINGLETON
	static GameManager _instance;

	public static GameManager instance
	{
		get
		{
			return _instance;
		}
	}
	#endregion SINGLETON

	[Header("Theme")]
	[SerializeField] BoardThemeDB _themeDB;
	[SerializeField] int _selectedTheme = 0;

	[Header("Board")]
	[SerializeField] NumberedHexaCell _hexaCellPrefab;
	[SerializeField] CombinableHexaBoard _board;
	[SerializeField] HexaBlock _block;

	[SerializeField] int _ignoreBlockCellNumbers = 0;
	[SerializeField] int _singleCellBlockLimitNotGenerated = 0;
	[SerializeField] bool _displayNumberToPower = true;

	[Header("Item")]
	[SerializeField] float _restoreBoardTime = 0.2f;
	[SerializeField] float _destroyCellTime = 0.2f;

	[Header("Options")]
	[SerializeField] int _maxContinueCount = 1;
	[SerializeField] int _maxContinueExplodeRadius = 1;
	[SerializeField] bool _debug = false;

	public Action<BoardTheme> onThemeChanged;
	public Action<Transition> onTransitioning;
	public Action<ItemType> onUseItem;
	public Action onGameOver;

	Camera _cachedMainCamera;

	BoardTheme _theme;
	Queue<HexaCell> _cellBuffer;
	Vector3 _beginPosition;

	Stack<int[]> _nextBlockNumbers = new Stack<int[]>();

	Transition _transition = Transition.None;
	Coroutine _coroutineHammer;

	long _lastShowReplayInterstitialTime = 0;

	#region UNITY EVENTS
	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			Initialize();
			
			DontDestroyOnLoad(gameObject);
		}
		else if (_instance != this)
		{
			Destroy(this);
		}
	}

	void Start()
	{
		LunarConsole.RegisterAction("Theme A", ChangeThemeA);
		LunarConsole.RegisterAction("Thene B", ChangeThemeB);

		LunarVariables.dragSensitivity.AddDelegate(ChangeDragSensitivity);
		LunarVariables.debug.AddDelegate(ChangeDebugMode);
	}

	void OnDestroy()
	{
		LunarConsole.UnregisterAllActions(this);

		LunarVariables.dragSensitivity.RemoveDelegate(ChangeDragSensitivity);
		LunarVariables.debug.RemoveDelegate(ChangeDebugMode);
	}
	#endregion UNITY EVENTS

	void Initialize()
	{
		_cachedMainCamera = Camera.main;

		SingleTouchManager.instance.DragSensitivity = LunarVariables.dragSensitivity;

		LeanTween.init(800);

		float defaultRatio = GameConstants.DEFAULT_SCREEN_WIDTH / GameConstants.DEFAULT_SCREEN_HEIGHT;
		float screenRatio = (float)Screen.width / Screen.height;
		transform.localScale *= Mathf.Min(screenRatio / defaultRatio, 1.0f);

		TILE_LAYER = LayerMask.NameToLayer("Tile");
		BLOCK_LAYER = LayerMask.NameToLayer("Block");

		_theme = _themeDB.GetTheme(_selectedTheme);
		if (_theme == null)
		{
			_theme = _themeDB.GetTheme(0);
		}

		IncreaseCellBuffer(_board.tiles.Length);

		_board.Initialize();
		_block.Initialize();

		ResetReplayInterstitialTime();
	}

	public void Reset()
	{
		_nextBlockNumbers.Clear();
		_board.Clear();
		_block.Reset();		
	}

	public void CombineCells(NumberedHexaTile[] seeds, float delay)
	{
		StartCoroutine(ProcessCombineCells(seeds, delay));
	}

	public void NewBlockCellNumbers()
	{
		_block.Setup(GetNextBlockCellNumbers());
	}

	public void ReplaceBlockByTrashCan()
	{
		if (onUseItem != null)
		{
			onUseItem(ItemType.TRASH);
		}

		NewBlockCellNumbers();
	}

	public void RestoreToPrevState(BoardSnapshot snapshot)
	{
		if (snapshot == null)
		{
			return;
		}

		StartCoroutine(ProcessRestoreToPrevState(snapshot));
	}

	public void ActiveHammers(bool active)
	{
		if (active)
		{
			if (_transition == Transition.None && _board.hasCells)
			{
				_coroutineHammer = StartCoroutine(ProcessHammer());
			}
		}
		else
		{
			if (_transition == Transition.Hammer)
			{
				StopCoroutine(_coroutineHammer);

				_board.ActiveHammerIcons(false);

				transition = Transition.None;
			}
		}
	}

	public void ChangeTheme(string themeName)
	{
		BoardTheme theme = _themeDB.GetTheme(themeName);
		if (theme != null &&
			theme != _theme)
		{
			_theme = theme;
			if (onThemeChanged != null)
			{
				onThemeChanged(_theme);
			}
		}
	}

	public void ResetReplayInterstitialTime()
	{
		_lastShowReplayInterstitialTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	}

	public BoardThemeDB themeDB
	{
		get
		{
			return _themeDB;
		}
	}

	public BoardTheme theme
	{
		get
		{
			return _theme;
		}
	}

	public CombinableHexaBoard board
	{
		get
		{
			return _board;
		}
	}

	public HexaBlock block
	{
		get
		{
			return _block;
		}
	}

	public bool displayNumberToPower
	{
		get
		{
			return _displayNumberToPower;
		}
	}

	public Transition transition
	{
		get
		{
			return _transition;
		}

		private set
		{
			_transition = value;
			if (onTransitioning != null)
			{
				onTransitioning(_transition);
			}
		}
	}

	public int maxContinueCount
	{
		get
		{
			return _maxContinueCount;
		}
	}

	public int maxContinueExplodeRadius
	{
		get
		{
			return _maxContinueExplodeRadius;
		}
	}

	public bool replayInterstitialEnabled
	{
		get
		{
			long interval = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _lastShowReplayInterstitialTime;
			return (interval >= DataManager.Instance.GameData.ReplayAdIntervalSeconds);
		}
	}

	public bool debug
	{
		get
		{
			return _debug;
		}
	}

	int GetNextBlockCellCount()
	{
		HexaTile[] emptyTiles = _board.emptyTiles;
		if (emptyTiles.Length <= 0)
		{
			return 0;
		}

		if (emptyTiles.Length == 1 ||
			Array.TrueForAll(emptyTiles, t => !t.hasEmptyLinkedTile))
		{
			return 1;
		}

		int min = (_board.highestNumber <= _singleCellBlockLimitNotGenerated) ? 1 : 0;

		return Random.Range(min, GameConstants.MAX_CELLS) + 1;
	}

	int[] GetNextBlockCellNumbers()
	{
		if (_nextBlockNumbers.Count > 0)
		{
			return _nextBlockNumbers.Pop();
		}

		List<int> numbers = new List<int>(Enumerable.Range(1, _board.highestNumber));
		numbers.RemoveAll(n => (_ignoreBlockCellNumbers & (1 << n)) != 0);
		numbers.Shuffle();

		return numbers.Take(GetNextBlockCellCount()).ToArray();
	}

	#region BLOCK CONTROL
	public HexaCell GenerateHexaCell()
	{
		if (_cellBuffer.Count <= 0)
		{
			IncreaseCellBuffer(_board.tiles.Length);
		}

		HexaCell cell = _cellBuffer.Dequeue();

		cell.Initialize();
		cell.gameObject.SetActive(true);
		cell.transform.SetParent(null);

		return cell;
	}

	public bool ReleaseHexaCell(HexaCell cell)
	{
		if (cell == null ||
			_cellBuffer.Contains(cell))
		{
			return false;
		}

		cell.gameObject.SetActive(false);
		cell.transform.SetParent(transform);

		cell.Release();

		_cellBuffer.Enqueue(cell);

		return true;
	}

	void IncreaseCellBuffer(int length)
	{
		if (_cellBuffer == null)
		{
			_cellBuffer = new Queue<HexaCell>(length);
		}

		for (int i = 0; i < length; ++i)
		{
			NumberedHexaCell cell = Instantiate(_hexaCellPrefab);
			cell.gameObject.SetActive(false);
			cell.transform.SetParent(transform);

			_cellBuffer.Enqueue(cell);
		}
	}
	#endregion BLOCK CONTROL

	#region LUNAR CONSOLE
	void ChangeThemeA()
	{
		ChangeTheme(_themeDB.themes[0].name);;
	}

	void ChangeThemeB()
	{
		ChangeTheme(_themeDB.themes[1].name);;
	}

	void ChangeDragSensitivity(CVar dragSensitivity)
	{
		SingleTouchManager.instance.DragSensitivity = dragSensitivity;
	}

	void ChangeDebugMode(CVar debug)
	{
		_debug = debug;
	}
	#endregion LUNAR CONSOLE


	#region COROUTINES
	IEnumerator ProcessCombineCells(NumberedHexaTile[] seeds, float delay)
	{
		transition = Transition.Combine;

		yield return new WaitForSeconds(delay);

		yield return _board.CombineCells(seeds);

		int[] nextNumbers = GetNextBlockCellNumbers();
		if (nextNumbers.Length > 0)
		{
			_block.Setup(nextNumbers);

			HexaTile[] emptyTiles = _board.emptyTiles;
			if (nextNumbers.Length > emptyTiles.Length ||
				(nextNumbers.Length > 1 && Array.TrueForAll(emptyTiles, t => !t.hasEmptyLinkedTile)))
			{
				if (onGameOver != null)
				{
					onGameOver();
				}
			}
		}
		else
		{
			if (onGameOver != null)
			{
				onGameOver();
			}
		}

		transition = Transition.None;
	}

	IEnumerator ProcessRestoreToPrevState(BoardSnapshot snapshot)
	{
		transition = Transition.Restore;

		_nextBlockNumbers.Push(_block.numbers);

		float halfRestoreBoardTime = _restoreBoardTime * 0.5f;

		_board.highestNumber = snapshot.highestNumber;

		for (int i = 0, max = _board.tiles.Length; i < max; ++i)
		{
			NumberedHexaTile tile = _board.tiles[i] as NumberedHexaTile;
			if (tile != null && tile.cell != null)
			{
				tile.cell.LeanCancel();
				tile.cell.LeanScale(Vector3.zero, halfRestoreBoardTime).
					setEaseInBack().
					setOnComplete(() => tile.Reset());
			}
		}

		NumberedHexaCell[] cells = _block.PopCells();
		for (int i = 0, max = cells.Length; i < max; ++i)
		{
			NumberedHexaCell cell = cells[i];

			cell.LeanCancel();
			cell.LeanScale(Vector3.zero, halfRestoreBoardTime).
				setEaseInBack().
				setOnComplete(() => ReleaseHexaCell(cell));
		}

		yield return new WaitForSeconds(halfRestoreBoardTime + 0.1f);

		for (int i = 0, max = snapshot.boardNumbers.Length; i < max; ++i)
		{
			if (snapshot.boardNumbers[i] > 0)
			{
				NumberedHexaCell cell = GenerateHexaCell() as NumberedHexaCell;
				cell.Setup(snapshot.boardNumbers[i]);

				_board.tiles[i].SetCell(cell);

				cell.LeanCancel();
				cell.LeanScale(Vector3.zero, Vector3.one, halfRestoreBoardTime).
					setEaseOutBack();
			}
		}

		_block.Setup(snapshot.blockNumbers);

		if (onUseItem != null)
		{
			onUseItem(ItemType.UNDO);
		}

		transition = Transition.None;
	}

	IEnumerator ProcessHammer()
	{
		transition = Transition.Hammer;

		_board.ActiveHammerIcons(true);

		yield return new WaitForSeconds(0.1f);

		NumberedHexaTile targetTile = null;
		Action<Vector3> onSelectTile = screenPosition => {
			RaycastHit2D hit = Physics2D.Raycast(
				_cachedMainCamera.ScreenToWorldPoint(screenPosition),
				Vector3.zero,
				Mathf.Infinity,
				1 << GameManager.TILE_LAYER
			);

			if (hit.collider != null) {
				HexaTile tile = hit.collider.GetComponent<HexaTile>();
				if (tile.cell != null) {
					targetTile = tile as NumberedHexaTile;
				}
			}
		};

		SingleTouchManager.instance.onUntouched += onSelectTile;

		while (targetTile == null)
		{
			yield return new WaitForEndOfFrame();
		}

		_board.ActiveHammerIcons(false);

		if (onUseItem != null)
		{
			onUseItem(ItemType.BREAK);
		}

		NumberedHexaCell cell = targetTile.cell as NumberedHexaCell;
		cell.gameObject.LeanScale(Vector3.zero, _destroyCellTime)
			.setEaseInBack();

		SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_USE_HAMMER);

		yield return new WaitForSeconds(_destroyCellTime);

		targetTile.Reset();

		transition = Transition.None;
	}
	#endregion COROUTINES
}