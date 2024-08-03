using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using LunarConsolePlugin;

public class HexaBlock : MonoBehaviour
{
	public enum State
	{
		Released,
		TouchDrag 		= 1 << 0,
		TouchRotateCW	= 1 << 1,
		TouchRotateCCW 	= 1 << 2,
	}

	const float TRASHCAN_TRANSITION_TIME = 0.1f;
	const float CELL_SETUP_SCALE_TIME = 0.2f;
	const float CELL_SETUP_SCALE_INTERVAL = 0.1f;
	const float ROTATE_MARKER_APPEAR_TIME = 0.5f;
	const float ROTATE_MARKER_DISAPPEAR_TIME = 0.2f;
	const float TRASHCAN_ENTERED_OFFSET = 0.0f;
	readonly Vector3 TRASHCAN_ENTERED_SCALE = Vector3.one * 0.5f;
	readonly Vector3 CELL_PLACED_SCALE = Vector3.one * 0.85f;
	const float CELL_PLACED_TIME = 0.1f;
	const float CELL_PLACED_DELAY = 0.05f;

	[SerializeField] Transform _cellRoot;
	[SerializeField] CircleCollider2D _areaDrag;
	[SerializeField] Collider2D _areaRotateCCW;
	[SerializeField] Collider2D _areaRotateCW;
	[SerializeField] GameObject _rotateMarker;

	[SerializeField] float _rotateDuration = 0.05f;
	[SerializeField] float _cellRotateDuration = 0.1f;
	[SerializeField] float _dragOffset = 1.5f;
	
	public Action onPlaced;

	bool _initialized = false;
	bool _interactable = true;

	List<NumberedHexaCell> _cells = new List<NumberedHexaCell>();
	List<NumberedHexaTile> _hoveredTiles = new List<NumberedHexaTile>();

	public State state { get; private set; }

	Camera _cachedMainCamera;

	Vector3 _originPosition;
	int _rotateStep = 0;
	float _angle = 0.0f;

	Vector3 _beginPosition;

	bool _moving = false;
	float _currentDragOffset;

	#region UNITY EVENTS
	void Awake()
	{
		gameObject.SetActive(false);
	}

	void Start()
	{
		_cachedMainCamera = Camera.main;

		LunarVariables.dragBlockOffset.AddDelegate(SetDragOffset);
	}

	void OnDistroy()
	{
		LunarVariables.dragBlockOffset.RemoveDelegate(SetDragOffset);
	}
	#endregion UNITY EVENTS

	public void Initialize()
	{
		if (_initialized)
		{
			return;
		}

		_initialized = true;

		_originPosition = _cellRoot.position;

		SingleTouchManager.instance.onTouched += OnTouched;
		SingleTouchManager.instance.onDragging += OnDragging;
		SingleTouchManager.instance.onUntouched += OnUntouched;
	}

	public void Release()
	{
		if (!_initialized)
		{
			return;
		}

		_initialized = false;

		SingleTouchManager.instance.onTouched -= OnTouched;
		SingleTouchManager.instance.onDragging -= OnDragging;
		SingleTouchManager.instance.onUntouched -= OnUntouched;
	}
	
	void ShowRotateMarker()
	{
		_rotateMarker.LeanCancel();
		_rotateMarker.LeanScale(Vector3.one, ROTATE_MARKER_APPEAR_TIME).
			setEaseOutBack();
	}

	void HideRotateMarker()
	{
		_rotateMarker.LeanCancel();
		_rotateMarker.LeanScale(Vector3.zero, ROTATE_MARKER_DISAPPEAR_TIME);
	}

	public void Reset()
	{
		for (int i = 0, max = _cells.Count; i < max; ++i)
		{
			GameManager.instance.ReleaseHexaCell(_cells[i]);
		}

		_cells.Clear();

		for (int i = 0, max = _hoveredTiles.Count; i < max; ++i)
		{
			_hoveredTiles[i].Unhovered();
		}

		_hoveredTiles.Clear();
	}

	public void Setup(int[] numbers, int rotateStep = 0)
	{
		Reset();

		state = State.Released;
		_moving = false;

		_rotateStep = rotateStep;
		_angle = rotateStep * GameConstants.HEXA_STEP_ANGLE;

		_cellRoot.localRotation = Quaternion.Euler(0.0f, 0.0f, _angle);

		float innerRadius =
			GameManager.instance.board.GetTileFlatOffsetWithSpacing();

		Vector3 offsetX = Vector3.left * innerRadius * (numbers.Length - 1);
		
		for (int i = 0; i < numbers.Length; ++i)
		{
			NumberedHexaCell cell =
				GameManager.instance.GenerateHexaCell() as NumberedHexaCell;

			cell.transform.SetParent(_cellRoot);
			cell.transform.localRotation = Quaternion.identity;
			cell.transform.localPosition =
				offsetX + Vector3.right * i * innerRadius * 2.0f;
			cell.transform.localScale = Vector3.zero;

			cell.LeanCancel();
			cell.LeanScale(Vector3.one, CELL_SETUP_SCALE_TIME).
				setEaseOutBack().
				setDelay(CELL_SETUP_SCALE_INTERVAL * i);

			cell.Setup(numbers[i], _rotateStep * -1);

			_cells.Add(cell);
		}

		ShowRotateMarker();
	}

	public NumberedHexaCell[] PopCells()
	{
		gameObject.LeanCancel();

		NumberedHexaCell[] cells = _cells.ToArray();
		for (int i = 0, max = cells.Length; i < max; ++i)
		{
			cells[i].transform.SetParent(null);
		}

		_cells.Clear();

		Reset();

		_cellRoot.position = _originPosition;
		_cellRoot.localScale = Vector3.one;

		return cells;
	}

	public void EnterTrashCan()
	{
		gameObject.LeanCancel();
		gameObject.LeanValue(_currentDragOffset, TRASHCAN_ENTERED_OFFSET, TRASHCAN_TRANSITION_TIME).
			setOnUpdate(v => _currentDragOffset = v);

		_cellRoot.LeanCancel();
		_cellRoot.LeanScale(TRASHCAN_ENTERED_SCALE, TRASHCAN_TRANSITION_TIME);
	}

	public void ExitTrashCan()
	{
		gameObject.LeanCancel();
		gameObject.LeanValue(_currentDragOffset, _dragOffset, TRASHCAN_TRANSITION_TIME).
			setOnUpdate(v => _currentDragOffset = v);

		_cellRoot.LeanCancel();
		_cellRoot.LeanScale(Vector3.one, TRASHCAN_TRANSITION_TIME);
	}

	public bool HasTouchState(State state)
	{
		return ((this.state & state) > 0);
	}

	public int[] numbers
	{
		get
		{
			List<int> numbers = new List<int>();
			for (int i = 0, max = _cells.Count; i < max; ++i)
			{
				numbers.Add(_cells[i].number);
			}

			return numbers.ToArray();
		}
	}

	public int rotateStep
	{
		get
		{
			return _rotateStep;
		}
	}

	public bool interactable
	{
		get
		{
			return _interactable;
		}

		set
		{
			if (_interactable != value && !value)
			{
				ReplaceToPlace();
			}

			_interactable = value;
		}
	}

	void ReplaceToPlace()
	{
		_cellRoot.LeanCancel();
		_cellRoot.LeanMove(_originPosition, 0.1f).
			setOnComplete(() => {
				if (_cells.Count > 0) {
					ShowRotateMarker();
				}
			});
		_cellRoot.LeanScale(Vector3.one, 0.1f);

		SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_BLOCK_BACK_TO_PLACE);

		ResetHoveredTiles();
	}

	void ResetHoveredTiles()
	{
		for (int i = 0, max = _hoveredTiles.Count; i < max; ++i)
		{
			_hoveredTiles[i].Unhovered();
		}

		_hoveredTiles.Clear();
	}

	void CheckBoard()
	{
		if (_hoveredTiles.Count == _cells.Count)
		{
			if (onPlaced != null)
			{
				onPlaced();
			}

			for (int i = 0, max = _hoveredTiles.Count; i < max; ++i)
			{
				NumberedHexaCell cell = _cells[i];

				_hoveredTiles[i].SetCell(cell);

				LTSeq sequence = LeanTween.sequence();
				sequence.append(cell.LeanScale(CELL_PLACED_SCALE, CELL_PLACED_TIME));
				sequence.append(CELL_PLACED_DELAY);
				sequence.append(cell.LeanScale(Vector3.one, CELL_PLACED_TIME));
			}

			_cells.Clear();

			SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_BLOCK_DROP);

			_cellRoot.position = _originPosition;

			GameManager.instance.CombineCells(_hoveredTiles.ToArray(), 0.25f);

			ResetHoveredTiles();
		}
		else
		{
			ReplaceToPlace();
		}
	}

	State CheckTouchState(Vector3 position)
	{
		RaycastHit2D[] hits = Physics2D.RaycastAll(
			position,
			Vector3.zero,
			Mathf.Infinity,
			1 << GameManager.BLOCK_LAYER
		);

		State state = State.Released;

		if (hits.Length > 0)
		{
			if (ContainTouchArea(hits, _areaDrag))
			{
				state |= State.TouchDrag;
			}

			if (ContainTouchArea(hits, _areaRotateCCW))
			{
				state |= State.TouchRotateCCW;
			}

			if (ContainTouchArea(hits, _areaRotateCW))
			{
				state |= State.TouchRotateCW;
			}
		}

		return state;
	}

	bool ContainTouchArea(RaycastHit2D[] hits, Collider2D target)
	{
		return (Array.FindIndex(hits, h => h.collider == target) >= 0);
	}

	#region ROTATE
	public void RotateCW()
	{
		--_rotateStep;

		Rotate();

		for (int i = 0, max = _cells.Count; i < max; ++i)
		{
			_cells[i].RotateCCW(_cellRotateDuration);
		}
	}

	public void RotateCCW()
	{
		++_rotateStep;

		Rotate();

		for (int i = 0, max = _cells.Count; i < max; ++i)
		{
			_cells[i].RotateCW(_cellRotateDuration);
		}
	}

	void Rotate()
	{
		SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_BLOCK_ROTATE);

		float to = _rotateStep * GameConstants.HEXA_STEP_ANGLE;

		_cellRoot.LeanCancel();
		_cellRoot.LeanValue(_angle, to, _rotateDuration).
			setOnUpdate(angle => {
				_angle = angle;
				_cellRoot.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
			});
	}
	#endregion ROTATE

	#region EVENTS
	void OnTouched(Vector3 screenPosition)
	{
		if (!_interactable ||
			_cells.Count <= 0 ||
			GameManager.instance.transition != GameManager.Transition.None)
		{
			return;
		}

		_beginPosition = _cachedMainCamera.ScreenToWorldPoint(screenPosition);

		state = CheckTouchState(_beginPosition);
	}

	void OnDragging(Vector3 screenPosition)
	{
		if (!_interactable || !HasTouchState(State.TouchDrag))
		{
			return;
		}

		if (!_moving)
		{
			_moving = true;
			_currentDragOffset = _dragOffset;

			HideRotateMarker();

			SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_BLOCK_GRAP);
		}

		Vector3 position = _cachedMainCamera.ScreenToWorldPoint(screenPosition);
		position.y += _currentDragOffset;
		position.z = _cellRoot.position.z;
		_cellRoot.position = position;

		List<NumberedHexaTile> hoveredTiles = new List<NumberedHexaTile>();
		for (int i = 0, max = _cells.Count; i < max; ++i)
		{
			RaycastHit2D hit = Physics2D.Raycast(
				_cells[i].transform.position,
				Vector3.zero,
				Mathf.Infinity,
				1 << GameManager.TILE_LAYER
			);

			if (hit.collider != null)
			{
				NumberedHexaTile tile = hit.collider.GetComponent<NumberedHexaTile>();
				if (tile.cell == null)
				{
					tile.Hovered(_cells[i].number);
					hoveredTiles.Add(tile);
				}
			}
		}

		for (int i = 0, max = _hoveredTiles.Count; i < max; ++i)
		{
			if (!hoveredTiles.Contains(_hoveredTiles[i]))
			{
				_hoveredTiles[i].Unhovered();
			}
		}

		_hoveredTiles = hoveredTiles;
	}

	void OnUntouched(Vector3 screenPosition)
	{
		if (state == State.Released)
		{
			return;
		}

		if (_moving)
		{
			_moving = false;

			CheckBoard();
			ResetHoveredTiles();

			state = State.Released;

			return;
		}
		
		if (_cells.Count <= 1)
		{
			state = State.Released;

			return;
		}

		RaycastHit2D[] hits = Physics2D.RaycastAll(
			_cachedMainCamera.ScreenToWorldPoint(screenPosition),
			Vector3.zero,
			Mathf.Infinity,
			1 << GameManager.BLOCK_LAYER
		);

		if (HasTouchState(State.TouchRotateCCW) &&
			ContainTouchArea(hits, _areaRotateCCW))
		{
			state = State.Released;

			RotateCCW();

			return;
		}

		if (HasTouchState(State.TouchRotateCW) &&
			ContainTouchArea(hits, _areaRotateCW))
		{
			state = State.Released;

			RotateCW();

			return;
		}
	}
	#endregion EVENTS

	#region LUNAR CONSOLE
	void SetDragOffset(CVar dragOFfset)
	{
		_dragOffset = dragOFfset;
	}
	#endregion LUNAR CONSOLE
}