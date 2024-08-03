using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;
using LunarConsolePlugin;
using GoogleMobileAds.Api;

using Random = UnityEngine.Random;

public class InGameState : State
{
	public const string PARAM_PLAY_DATA = "PlayData";

	[Header("In Game")]
	[SerializeField] UICounter _highscoreCounter;
	[SerializeField] UICounter _scoreCounter;
	[SerializeField] UICounter _coinCounter;
	[SerializeField] GameObject _coinEffect;
	[SerializeField] Vector3 _counterCountingScale = Vector3.one * 1.2f;
	[SerializeField] float _counterScalingTime = 0.25f;
	
	[SerializeField] TMP_Text _addedScoreText;
	[SerializeField] Vector3 _addedScoreBeginOffset = Vector3.up * 50.0f;
	[SerializeField] Vector3 _addedScoreEndOffset = Vector3.up * 100.0f;
	
	[SerializeField] float _addedScoreAppearTime = 1.0f;

	[Header("Messages")]
	[SerializeField] UICombo _comboMessage;
	[SerializeField] UIMessage _reachedLastNumberMessage;
	[SerializeField] UIMissionClear _missionClearMessage;
	[SerializeField] UIMessage _noMoreCellMssage;
	
	[Header("Snapshot")]
	[SerializeField] int _maxSnapshot = 20;

	[Header("Items")]
	[SerializeField] Color _itemCoinActivatedColor = Color.white;
	[SerializeField] Color _itemCoinInactivatedColor = Color.red;


	[Header("Items - Trash Can")]
	[SerializeField] UITrashCan _trashCan;
	[SerializeField] TMP_Text _trashCanCost;
	[SerializeField] float _trashCanDectectRadius = 1.0f;
	[SerializeField] float _throwToTrashCanDuration = 0.2f;
	[SerializeField] float _throwToTrashCanBezierOffset = 0.3f;
	[SerializeField] float _throwToTrashCanCellSize = 0.1f;

	[Header("Items - Restore")]
	[SerializeField] Button _restoreButton;
	[SerializeField] TMP_Text _restoreCostText;


	[Header("Items - Hammer")]
	[SerializeField] Button _hammerButton;
	[SerializeField] TMP_Text _hammerCostText;

	[Header("Mission")]
	[SerializeField] UIMission _mission;

	[Header("Coin Box")]
	[SerializeField] Button _coinboxButton;
	[SerializeField] UIChestBox _coinBoxIcon;
	[SerializeField] TMP_Text _coinboxCooltimeText;
	[SerializeField] float _coinBoxIconAnimateInterval = 3.0f;

	int[] _itemUsed = new int[Enum.GetValues(typeof(ItemType)).Length];

	List<BoardSnapshot> _snapshots = new List<BoardSnapshot>();

	int _savings = 0;
	int _contineCount = 0;
	int _missionTargetIndex = -1;
	bool _trashCanEntered = false;

	long _lastRemainingCooltime = 0;
	long _lastShownInterstitialAdTime = 0;

	Coroutine _missionCheckCoroutine;
	Coroutine _coroutineCoinBoxIcon;

	Camera _cachedMainCamera;

	#region UNITY EVENTS
	void OnApplicationQuit()
	{
		if (StateManager.instance != null &&
			StateManager.instance.IsStacked(this))
		{
			SavePlayData();
		}
		
	}

	void OnDrawGizmos()
	{
		if (_cachedMainCamera == null)
		{
			return;
		}

		Vector3 trashCanPosition = _cachedMainCamera.ScreenToWorldPoint(_trashCan.transform.position);
		Vector3 touchPosition = _cachedMainCamera.ScreenToWorldPoint(Input.mousePosition);

		Gizmos.color = (Vector3.Distance(trashCanPosition, touchPosition) <= _trashCanDectectRadius) ? Color.green : Color.red;
		Gizmos.DrawWireSphere(trashCanPosition, _trashCanDectectRadius);
		Gizmos.DrawSphere(trashCanPosition, 0.1f);
		Gizmos.DrawSphere(touchPosition, 0.1f);
	}
	#endregion UNITY EVENTS

	public override IEnumerator OnInitialize()
	{
		yield return base.OnInitialize();

		_cachedMainCamera = Camera.main;

		GameManager.instance.board.gameObject.SetActive(false);
		GameManager.instance.block.gameObject.SetActive(false);

		InitializeCounter(_scoreCounter);
		InitializeCounter(_highscoreCounter);
		InitializeCounter(_coinCounter);

		_scoreCounter.onEndCounting += increase => _mission.Emphasize();
		_coinCounter.onBeginCounting += increase => {
			if (increase) {
				_coinEffect.SetActive(true);
			} else {
				SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_DECREASE_COINS);
			}
		};

		_comboMessage.Initialize();
		_reachedLastNumberMessage.Initialize();
		_noMoreCellMssage.Initialize();
		_missionClearMessage.Initialize();

		_coinEffect.SetActive(false);
	}

	public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		LunarVariables.debug.AddDelegate(ChangeDebugMode);

		ResetCounter(_scoreCounter);
		ResetCounter(_highscoreCounter);
		ResetCounter(_coinCounter);

		_savings = 0;
		_contineCount = 0;

		_snapshots.Clear();

		GameManager.instance.board.gameObject.SetActive(true);
		GameManager.instance.block.gameObject.SetActive(true);

		GameManager.instance.Reset();

		for (int i = 0, max = _itemUsed.Length; i < max; ++i)
		{
			_itemUsed[i] = 0;
		}

		_trashCan.Load();

		_addedScoreText.alpha = 0.0f;

		//
		int score = 0;
		PlayData playData = GetParam<PlayData>(PARAM_PLAY_DATA, null);
		if (playData != null)
		{
			score = playData.Score;
			_savings = playData.Coin;
			_contineCount = playData.AdContinueWatchCount;
			
			if (playData.Cells != null)
			{
				int[] numbers = new int[playData.Cells.Length];
				for (int y = 0, height = playData.Cells.GetLength(0); y < height; ++y)
				{
					for (int x = 0, width = playData.Cells.GetLength(1); x < width; ++x)
					{
						numbers[y * width + x] = playData.Cells[y, x];
					}
				}

				GameManager.instance.board.Setup(numbers);
			}
			else
			{
				GameManager.instance.board.Clear();
			}

			int[] itemUsed = playData.ItemUseCountInfo.ItemUseCountArray;
			for (int i = 0, max = Mathf.Min(_itemUsed.Length, itemUsed.Length); i < max; ++i)
			{
				_itemUsed[i] = itemUsed[i];
			}
		}

		//
		_mission.Reset();

		_scoreCounter.SetDefault(score);
		_highscoreCounter.SetDefault(Mathf.Max(score, DataManager.Instance.UserData.Score));
		_coinCounter.SetDefault(DataManager.Instance.UserData.Coin);
		_coinEffect.SetActive(false);

		UpdateCoinbox(true);
		UpdateItemButtons();

		SoundManager.Instance.PlayBgm(GameConstants.BGM_INGAME);
	}

	public override IEnumerator OnEnter()
	{
		yield return base.OnEnter();

		SingleTouchManager.instance.onDragging += OnDragging;
		SingleTouchManager.instance.onUntouched += OnUntouched;

		GameManager.instance.board.onCombineCells += OnCombineCells;
		GameManager.instance.board.onCompleteCombine += OnCompleteCombine;
		GameManager.instance.board.onReachedLastNumber += OnReachedLastNumber;

		GameManager.instance.block.interactable = true;
		GameManager.instance.block.onPlaced += GenerateSnapshot;

		GameManager.instance.onUseItem += OnUseItem;
		GameManager.instance.onGameOver += OnGameOver;

		PlayData playData = GetParam<PlayData>(PARAM_PLAY_DATA, null);
		if (playData != null &&
			playData.LastParts != null &&
			playData.LastParts.Cells.Length > 0)
		{
			GameManager.instance.block.Setup(
				playData.LastParts.Cells,
				playData.LastParts.AngleValue
			);
		}
		else
		{
			GameManager.instance.NewBlockCellNumbers();
		}

		int[] missionTargetScores = DataManager.Instance.GameData.MissionRewardScores;
		_missionTargetIndex = Array.FindIndex(missionTargetScores, s => s > _scoreCounter.value);
		_mission.Show(_missionTargetIndex);

		AnalyticsManager.Instance.EventLog(GameConstants.EVENTLOG_BASICMODE_START);
	}

	public override void OnUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnPauseGame();
		}

		UpdateCoinbox();
	}

	public override void OnPause()
	{
		GameManager.instance.block.interactable = false;
	}

	public override IEnumerator OnResume()
    {
        yield return base.OnResume();

		GameManager.instance.block.interactable = true;
    }

	public override void OnAppPause()
	{
		SavePlayData();
	}

	public override IEnumerator OnUnload()
	{
		LunarVariables.debug.AddDelegate(ChangeDebugMode);

		SavePlayData();

		SoundManager.Instance.StopBgm(GameConstants.BGM_INGAME);

		GameManager.instance.board.onCombineCells -= OnCombineCells;
		GameManager.instance.board.onCompleteCombine -= OnCompleteCombine;
		GameManager.instance.board.onReachedLastNumber += OnReachedLastNumber;

		GameManager.instance.block.onPlaced -= GenerateSnapshot;

		GameManager.instance.onUseItem -= OnUseItem;
		GameManager.instance.onGameOver -= OnGameOver;

		GameManager.instance.board.gameObject.SetActive(false);
		GameManager.instance.block.gameObject.SetActive(false);

		_trashCan.Unload();

		yield return base.OnUnload();
	}

	public void GenerateSnapshot()
	{
		BoardSnapshot snapshot = new BoardSnapshot(
			GameManager.instance.board,
			GameManager.instance.block,
			_scoreCounter.value,
			_savings
		);

		_snapshots.Add(snapshot);
		if (_snapshots.Count > _maxSnapshot)
		{
			_snapshots.RemoveAt(0);
		}
	}

	public override string stateName
	{
		get
		{
			return GameConstants.STATENAME_INGAME;
		}
	}

	void InitializeCounter(UICounter counter)
	{
		GameObject counterGO = counter.gameObject;

		counter.onBeginCounting += increase => {
			counterGO.LeanCancel();
			counterGO.LeanScale(_counterCountingScale, _counterScalingTime);
		};
		counter.onEndCounting += increase => {
			counterGO.LeanCancel();
			counterGO.LeanScale(Vector3.one, _counterScalingTime);
		};
	}

	void ResetCounter(UICounter counter)
	{
		counter.LeanCancel();
		counter.transform.localScale = Vector3.one;
	}

	void SavePlayData()
	{
		//
		CombinableHexaBoard board = GameManager.instance.board;
		int[] numbers = board.numbers;
		int[,] cells = new int[board.width, board.height];
		for (int i = 0, imax = board.height; i < imax; ++i)
		{
			for (int j = 0, jmax = board.width; j < jmax; ++j)
			{
				cells[i, j] = numbers[i * board.width + j];
			}
		}

		//
		HexaBlock block = GameManager.instance.block;
		LastBlocks lastBlocks =
			new LastBlocks(block.numbers, block.rotateStep);

		//
		DataManager.Instance.SavePlayData(
			_scoreCounter.value,
			_savings,
			cells,
			lastBlocks,
			_itemUsed,
			_contineCount
		);
	}

	int GetItemCost(ItemType itemType)
	{
		GameData gameData = DataManager.Instance.GameData;

		return (int)Mathf.Pow(2, _itemUsed[(int)itemType]) * gameData.InitItemCost;
	}

	string GetItemCostString(ItemType itemType)
	{
		stringBuilder.Remove(0, stringBuilder.Length);
		stringBuilder.Append("<sprite name=\"coin\">");
		stringBuilder.Append(GetItemCost(itemType));

		return stringBuilder.ToString();
	}

	void UpdateScore()
	{
		DataManager manager = DataManager.Instance;

		_highscoreCounter.Setup(Mathf.Max(_scoreCounter.value, manager.UserData.Score));

		if (_missionCheckCoroutine != null)
		{
			return;
		}

		_missionCheckCoroutine = StartCoroutine(ProcessCheckMission());
	}

	void UpdateCoin()
	{
		if (_coinCounter.value == DataManager.Instance.UserData.Coin)
		{
			return;
		}

		_coinCounter.Setup(DataManager.Instance.UserData.Coin, 0.2f);

		UpdateItemButtons();
	}

	bool HasEnoughCoin(ItemType itemType)
	{
		return (DataManager.Instance.UserData.Coin >= GetItemCost(itemType) || GameManager.instance.debug);
	}

	void UpdateItemButtons()
	{
		//
		bool restoreButtonActivated = HasEnoughCoin(ItemType.UNDO);
		_restoreButton.interactable = restoreButtonActivated;
		_restoreCostText.text = GetItemCostString(ItemType.UNDO);
		_restoreCostText.color =
			restoreButtonActivated ? _itemCoinActivatedColor : _itemCoinInactivatedColor;

		//
		bool hammerButtonActivated = HasEnoughCoin(ItemType.BREAK);
		_hammerButton.interactable = hammerButtonActivated;
		_hammerCostText.text = GetItemCostString(ItemType.BREAK);
		_hammerCostText.color
			= hammerButtonActivated ? _itemCoinActivatedColor : _itemCoinInactivatedColor;

		//
		bool trashCanButtonActivated = HasEnoughCoin(ItemType.TRASH);
		_trashCan.interactable = trashCanButtonActivated;
		_trashCanCost.text = GetItemCostString(ItemType.TRASH);
		_trashCanCost.color
			= trashCanButtonActivated ? _itemCoinActivatedColor : _itemCoinInactivatedColor;
	}

	void UpdateCoinbox(bool forced = false)
	{
		long remainingCooltime =
			DataManager.Instance.UserData.UserRewardInfo.GetCoinBoxCoolTimeSeconds();
		if (_lastRemainingCooltime == remainingCooltime && !forced)
		{
			return;
		}

		bool coinboxActivated = (remainingCooltime <= 0);
		bool prevActivation = _coinboxButton.interactable;
		_coinboxButton.interactable = coinboxActivated;
		_coinboxCooltimeText.gameObject.SetActive(!coinboxActivated);
		
		if (coinboxActivated)
		{
			if (prevActivation != coinboxActivated || forced)
			{
				if (_coroutineCoinBoxIcon != null)
				{
					StopCoroutine(_coroutineCoinBoxIcon);
				}

				_coroutineCoinBoxIcon = StartCoroutine(ProcessCoinBoxButton());
			}
		}
		else
		{
			if (prevActivation != coinboxActivated || forced)
			{
				if (_coroutineCoinBoxIcon != null)
				{
					StopCoroutine(_coroutineCoinBoxIcon);
				}

				_coinBoxIcon.PlayIdle();
			}

			TimeSpan cooltime = TimeSpan.FromSeconds((double)remainingCooltime);

			stringBuilder.Remove(0, stringBuilder.Length);
			if (cooltime.Hours > 0)
			{
				stringBuilder.AppendFormat("{0:00}:", cooltime.Hours);
			}
			stringBuilder.AppendFormat("{0:00}:{1:00}", cooltime.Minutes, cooltime.Seconds);

			_coinboxCooltimeText.text = stringBuilder.ToString();
		}

		_lastRemainingCooltime = remainingCooltime;
	}

	#region EVENTS
	public void OnPauseGame()
	{
		SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_TOUCH_BUTTON_PAUSE);
		StateManager.instance.PushState(GameConstants.STATENAME_POPUPPAUSE);
	}

	public void OnDailyCoinBox()
	{
		SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_TOUCH_BUTTON_DEFAULT);

		Dictionary<string, object> args = new Dictionary<string, object>() {
			{ PopupCoinBox.PARAM_REWARDED_CALLBACK, (Action)(() => UpdateCoin()) }
		};

		StateManager.instance.PushState(GameConstants.STATENAME_POPUPCOINBOX, args);
	}

	public void OnRestoreToPrevState()
	{
		if (!(HasEnoughCoin(ItemType.UNDO) || GameManager.instance.debug) ||
			_snapshots.Count <= 0 ||
			GameManager.instance.transition != GameManager.Transition.None)
		{
			SoundManager.Instance.PlayUISoundInstance(
				GameConstants.UIFX_TOUCH_LOCKED_BUTTON_ITEM
			);

			return;
		}

		SoundManager.Instance.PlayUISoundInstance(
			GameConstants.UIFX_TOUCH_BUTTON_ITEM
		);

		BoardSnapshot snapshot = _snapshots[_snapshots.Count - 1];
		_snapshots.Remove(snapshot);

		_scoreCounter.Setup(snapshot.score);
		_savings = snapshot.savings;

		GameManager.instance.RestoreToPrevState(snapshot);
	}

	public void OnToggleHammer()
	{
		GameManager manager = GameManager.instance;

		switch (manager.transition)
		{
			case GameManager.Transition.None:
				{
					if (!manager.board.hasCells ||
						manager.block.state != HexaBlock.State.Released ||
						!(HasEnoughCoin(ItemType.BREAK) || manager.debug))
					{
						SoundManager.Instance.PlayUISoundInstance(
							GameConstants.UIFX_TOUCH_LOCKED_BUTTON_ITEM
						);

						return;
					}

					manager.ActiveHammers(true);
				}
				break;

			case GameManager.Transition.Hammer:
				{
					manager.ActiveHammers(false);
				}
				break;
		}

		SoundManager.Instance.PlayUISoundInstance(
			GameConstants.UIFX_TOUCH_BUTTON_ITEM
		);
	}

	public void OnThrowBlockToTrashCan()
	{
		if (!(HasEnoughCoin(ItemType.TRASH) || GameManager.instance.debug) ||
			GameManager.instance.transition != GameManager.Transition.None)
		{
			SoundManager.Instance.PlayUISoundInstance(
				GameConstants.UIFX_TOUCH_LOCKED_BUTTON_ITEM
			);

			return;
		}

		if (GameManager.instance.block.numbers.Length <= 0 ||
			GameManager.instance.block.state != HexaBlock.State.Released)
		{
			return;
		}

		GenerateSnapshot();

		StartCoroutine(ProcessThrowBlockToTrashCan());
	}

	IEnumerator ProcessThrowBlockToTrashCan()
	{
		NumberedHexaCell[] cells = GameManager.instance.block.PopCells();

		for (int i = 0, max = cells.Length; i < max; ++i)
		{
			Vector2 begin = cells[i].transform.position;
			Vector2 end = _cachedMainCamera.ScreenToWorldPoint(_trashCan.transform.position);
			Vector2 direction = end - begin;
			Vector3 center = begin + direction * 0.5f;
			float angle = (direction.x < 0) ? -90.0f : 90.0f;
			Quaternion rotation = Quaternion.Euler(Vector3.forward * angle);
			Vector3 anchor = center + rotation * direction * _throwToTrashCanBezierOffset;

			LTBezierPath bezierPath = new LTBezierPath(
				new Vector3[] { begin, anchor, anchor, end }
			);

			cells[i].LeanMove(bezierPath, _throwToTrashCanDuration);
			cells[i].LeanScale(Vector3.one * _throwToTrashCanCellSize, _throwToTrashCanDuration);
		}

		_trashCan.Open();

		yield return new WaitForSeconds(_throwToTrashCanDuration);

		SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_THROW_TO_TRASH_CAN);

		_trashCan.Close();

		for (int i = 0, max = cells.Length; i < max; ++i)
		{
			GameManager.instance.ReleaseHexaCell(cells[i]);
		}

		yield return new WaitForSeconds(0.2f);

		GameManager.instance.ReplaceBlockByTrashCan();
	}
	#endregion EVENTS

	#region LUNAR CONSOLE
	void ChangeDebugMode(CVar debug)
	{
		UpdateItemButtons();
	}
	#endregion LUNAR CONSOLE

	#region CALLBACKS
	void OnDragging(Vector3 screenPosition)
	{
		if (!GameManager.instance.block.HasTouchState(HexaBlock.State.TouchDrag))
		{
			return;
		}

		if (!_trashCan.interactable)
		{
			if (_trashCanEntered)
			{
				_trashCanEntered = false;

				GameManager.instance.block.ExitTrashCan();
			}

			return;
		}

		Vector3 position = _cachedMainCamera.ScreenToWorldPoint(screenPosition);
		Vector3 trashCanPosition = _cachedMainCamera.ScreenToWorldPoint(_trashCan.transform.position);
		bool isInTrashCan = Vector3.Distance(position, trashCanPosition) <= _trashCanDectectRadius;
		if (!_trashCanEntered)
		{
			if (isInTrashCan && HasEnoughCoin(ItemType.TRASH))
			{
				_trashCanEntered = true;
				_trashCan.Open();

				GameManager.instance.block.EnterTrashCan();
			}
		}
		else
		{
			if (!isInTrashCan)
			{
				_trashCanEntered = false;
				_trashCan.Close();

				GameManager.instance.block.ExitTrashCan();
			}
		}
	}

	void OnUntouched(Vector3 screenPosition)
	{
		if (_trashCanEntered)
		{
			_trashCan.Close();

			Vector3 position = _cachedMainCamera.ScreenToWorldPoint(screenPosition);
			Vector3 trashCanPosition = _cachedMainCamera.ScreenToWorldPoint(_trashCan.transform.position);
			if (Vector3.Distance(position, trashCanPosition) <= _trashCanDectectRadius)
			{
				OnThrowBlockToTrashCan();
			}

			_trashCanEntered = false;
		}
	}

	void OnCombineCells(int number, int count, int combo)
	{
		_savings += number * count;

		if (combo <= 0)
		{
			return;
		}

		_comboMessage.Show(combo);
	}

	void OnCompleteCombine(Vector3 position, int lastNumber, int addedScore)
	{
		_scoreCounter.Add(addedScore);

		stringBuilder.Remove(0, stringBuilder.Length);
		stringBuilder.AppendFormat("+{0:#,##0}", addedScore);

		_addedScoreText.color =
			GameManager.instance.theme.GetCellSprite(lastNumber).color;

		ShowHUD(position, stringBuilder.ToString(), UpdateScore);
	}

	void OnReachedLastNumber()
	{
		_reachedLastNumberMessage.Show();
	}

	const float HUD_APPEAR_RATIO = 0.2f;
	const float HUD_DISAPPEAR_DELAY = 0.5f;

	void ShowHUD(Vector3 position, string message, Action onComplete = null)
	{
		position = _cachedMainCamera.WorldToScreenPoint(position);

		Vector3 begin = position + _addedScoreBeginOffset;
		Vector3 end = position + _addedScoreEndOffset;

		_addedScoreText.text = message;

		float appearTime = _addedScoreAppearTime * HUD_APPEAR_RATIO;
		float disappearTime = _addedScoreAppearTime * (1.0f - HUD_APPEAR_RATIO);

		_addedScoreText.LeanCancel();

		LTSeq sequence = LeanTween.sequence();
		sequence.append(() => _addedScoreText.LeanAlpha(0.0f, 1.0f, appearTime));
		sequence.append(_addedScoreText.LeanScale(Vector3.zero, Vector3.one, appearTime).setEaseOutBack());
		sequence.append(HUD_DISAPPEAR_DELAY);
		sequence.append(() => _addedScoreText.LeanAlpha(0.0f, disappearTime));
		sequence.append(_addedScoreText.LeanMove(begin, end, disappearTime));

		sequence.append(() => {
			if (onComplete != null) {
				onComplete();
			}
		});
	}

	void OnUseItem(ItemType itemType)
	{
		if (!GameManager.instance.debug)
		{
			DataManager.Instance.UserData.AddCoin(-GetItemCost(itemType));
		}

		++_itemUsed[(int)itemType];

		switch (itemType)
		{
			case ItemType.UNDO:
				{
					UpdateScore();
				}
				break;

			case ItemType.BREAK:
				{
					GenerateSnapshot();
				}
				break;
		}

		UpdateCoin();
	}

	void OnGameOver()
	{
		StartCoroutine(ProcessGameOver());
	}

	void OnShowResult()
	{
		bool bestScoreUpdated = (_scoreCounter.value > DataManager.Instance.UserData.Score);

		Dictionary<string, object> resultArgs = new Dictionary<string, object>() {
			{ PopupGameover.PARAM_SCORE, _scoreCounter.value },
			{ PopupGameover.PARAM_BEST_SCORE_UPDTATED, bestScoreUpdated },
			{ PopupGameover.PARAM_REWARDED_COIN, _savings },
			{ PopupGameover.PARAM_COIN_COUNTER, _coinCounter }
		};
		DataManager.Instance.SaveGameResult(_scoreCounter.value, _savings, _itemUsed);
		StateManager.instance.PushState(GameConstants.STATENAME_POPUPGAMEOVER, resultArgs);
	}
	#endregion CALLBACKS

	#region COROUTINES
	IEnumerator ProcessCheckMission()
	{
		int reward = DataManager.Instance.GameData.MissionRewardCoin;
		int[] missionScores = DataManager.Instance.GameData.MissionRewardScores;
		while (_missionTargetIndex >= 0 &&
			   _missionTargetIndex < missionScores.Length &&
			   _scoreCounter.value >= missionScores[_missionTargetIndex])
		{
			_mission.Hide();
			_missionClearMessage.Show(reward);

			yield return new WaitForSeconds(_missionClearMessage.time);

			++_missionTargetIndex;

			DataManager.Instance.UserData.AddCoin(reward);

			UpdateCoin();

			_mission.Show(_missionTargetIndex);

			yield return new WaitForSeconds(_missionClearMessage.time);
		}

		_missionCheckCoroutine = null;
	}

	IEnumerator ProcessCoinBoxButton()
	{
		while (true)
		{
			yield return new WaitForSeconds(_coinBoxIconAnimateInterval);

			_coinBoxIcon.PlayOpenBox2();
		}
	}

	IEnumerator ProcessGameOver()
	{
		_noMoreCellMssage.Show();

		yield return new WaitForSeconds(_noMoreCellMssage.time);

		if (_contineCount < GameManager.instance.maxContinueCount)
		{
			yield return ProcessContinue();
		}
		else
		{
			OnShowResult();
		}
	}

	IEnumerator ProcessContinue()
	{
		bool opened = true;
		bool rewarded = false;

		Action onRewarded = () => {
			opened = false;
			rewarded = true;
		};

		Action onCanceled = () => {
			opened = false;
		};

		Dictionary<string, object> continueArgs = new Dictionary<string, object>() {
			{ PopupContinue.PARAM_REWARDED_CALLBACK, onRewarded },
			{ PopupContinue.PARAM_CANCEL_CALLBACK, onCanceled }
		};

		StateManager.instance.PushState(GameConstants.STATENAME_POPUPCONTINUE, continueArgs);

		while (opened || StateManager.instance.nowTransitioning)
		{
			yield return new WaitForEndOfFrame();
		}

		if (rewarded)
		{
			++_contineCount;

			_snapshots.Clear();

			HexaTile[] tiles = Array.FindAll(
				GameManager.instance.board.tiles,
				t => t != null && Array.TrueForAll(t.neighbors, n => n != null)
			);

			int targetIndex = Random.Range(0, tiles.Length);
			HexaTile seed = tiles[targetIndex];
			
			yield return GameManager.instance.board.Explode(
				seed,
				GameManager.instance.maxContinueExplodeRadius,
				GameConstants.HIGHEST_CELL_NUMBER);

			GameManager.instance.NewBlockCellNumbers();
		}
		else
		{
			OnShowResult();
		}
	}
	#endregion COROUTINES
}