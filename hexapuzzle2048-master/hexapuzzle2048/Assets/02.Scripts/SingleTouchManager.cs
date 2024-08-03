using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SingleTouchManager : MonoBehaviour
{
	public enum State
	{
		Released,
		Touched,
		Untouched,
		Dragging,
	}

	#region SINGLETON
	static SingleTouchManager _instance;

	public static SingleTouchManager instance
	{
		get
		{
			if (_instance == null)
			{
                _instance = ObjectUtil.CreateInstance<SingleTouchManager>("SingleTouchManager");
                DontDestroyOnLoad(_instance.gameObject);                				
			}

			return _instance;
		}
	}
	#endregion SINGLETON

	[SerializeField] float _dragSensitivity = 100f;

	public Action<Vector3> onTouched;
	public Action<Vector3> onUntouched;
	public Action<Vector3> onDragBegan;
	public Action<Vector3> onDragging;

	State _state = State.Released;

	Vector3 _beginScreenPosition;
	Vector3 _lastScreenPosition;
	int _fingerID = 0;

	#region UNITY EVENTS
	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;

			DontDestroyOnLoad(gameObject);
		}
		else if (_instance != this)
		{
			Destroy(this);
		}
	}

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touchCount == 0)
			{
				return;
			}

			Touch touch = Input.GetTouch(0);
			if (_state == State.Released)
			{
				_fingerID = touch.fingerId;
			}
			else
			{
				bool found = false;
				for (int i = 0, max = Input.touchCount; i < max; ++i)
				{
					if (Input.GetTouch(i).fingerId == _fingerID)
					{
						touch = Input.GetTouch(i);

						found = true;

						break;
					}
				}

				if (!found)
				{
					_state = State.Released;
					if (onUntouched != null)
					{
						onUntouched(_lastScreenPosition);
					}

					return;
				}
			}

			_lastScreenPosition = touch.position;

			switch (touch.phase)
			{
				case TouchPhase.Began:
					{
						_state = State.Touched;
						_beginScreenPosition = touch.position;
						
						if (onTouched != null)
						{
							onTouched(_lastScreenPosition);
						}
					}
					break;

				case TouchPhase.Moved:
					{
						if (_state == State.Touched)
						{
							if (Vector3.Distance(_beginScreenPosition, _lastScreenPosition) >= _dragSensitivity)
							{
								_state = State.Dragging;

								if (onDragBegan != null)
								{
									onDragBegan(_lastScreenPosition);
								}
							}

							return;
						}

						if (onDragging != null)
						{
							onDragging(_lastScreenPosition);
						}
					}
					break;

				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					{
						_state = State.Released;

						if (onUntouched != null)
						{
							onUntouched(_lastScreenPosition);
						}
					}
					break;
			}
		}
		else
		{
			if (_state == State.Released)
			{
				if (Input.GetMouseButtonDown(0))
				{
					_state = State.Touched;
					_beginScreenPosition = Input.mousePosition;
					_lastScreenPosition = Input.mousePosition;

					if (onTouched != null)
					{
						onTouched(_beginScreenPosition);
					}
				}
			}
			else
			{
				_lastScreenPosition = Input.mousePosition;

				if (Input.GetMouseButtonUp(0))
				{
					_state = State.Released;

					if (onUntouched != null)
					{
						onUntouched(_lastScreenPosition);
					}

					return;
				}

				if (_state == State.Touched)
				{
					if (Vector3.Distance(_beginScreenPosition, _lastScreenPosition) >= _dragSensitivity)
					{
						_state = State.Dragging;

						if (onDragBegan != null)
						{
							onDragBegan(_lastScreenPosition);
						}
					}

					return;
				}

				if (onDragging != null)
				{
					onDragging(_lastScreenPosition);
				}
			}
		}
	}
	#endregion UNITY EVENTS

	public float DragSensitivity
	{
		get
		{
			return _dragSensitivity;
		}

		set
		{
			_dragSensitivity = value;
		}
	}
}