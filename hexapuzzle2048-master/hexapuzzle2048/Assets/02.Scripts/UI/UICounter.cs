using System;
using System.Text;
using System.Collections;

using UnityEngine;

using TMPro;

public class UICounter : MonoBehaviour
{
	enum State
	{
		None,
		Increasing,
		Decreasing,
	}

	static StringBuilder stringBuilder = new StringBuilder();

	[SerializeField] TMP_Text _text;
	[SerializeField] float _time = 0.1f;
	[SerializeField] float _increaseRatio = 1.1f;

	public int value { get; private set; }

	public Action<bool> onBeginCounting;
	public Action<bool> onEndCounting;

	int _displayedCount = 0;

	float _increaseValue = 0.0f;

	Coroutine _countingCoroutine;

	#region UNITY EVENTS
	void Awake()
	{
		if (_text == null)
		{
			_text = GetComponentInChildren<TMP_Text>();
		}
	}
	#endregion UNITY EVENTS

	public void SetDefault(int count)
	{
		StopAllCoroutines();

		this.value = count;
		_displayedCount = count;

		UpdateText(_displayedCount);
	}

	public void Setup(int count, float delay = 0.0f)
	{
		if (_countingCoroutine == null ||
			(this.value - _displayedCount > 0) != (count - _displayedCount) > 0)
		{
			_increaseValue = 1.0f;
		}

		StopAllCoroutines();

		this.value = count;

		if (count != _displayedCount)
		{
			_countingCoroutine = StartCoroutine(ProcessCounting(delay));
		}
	}

	public void Add(int added, float delay = 0.0f)
	{
		Setup(value + added, delay);
	}

	public bool nowCounting
	{
		get
		{
			return (_countingCoroutine != null);
		}
	}

	void UpdateText(int count)
	{
		if (_text == null)
		{
			return;
		}

		stringBuilder.Remove(0, stringBuilder.Length);
		stringBuilder.AppendFormat("{0:#,##0}", count);
		_text.text = stringBuilder.ToString();
	}

	#region COROUTINES
	IEnumerator ProcessCounting(float delay)
	{
		yield return new WaitForSeconds(delay);

		int gap = value - _displayedCount;
		float direction = (gap > 0) ? 1.0f : -1.0f;
		gap = Mathf.Abs(gap);

		if (onBeginCounting != null)
		{
			onBeginCounting(direction > 0);
		}

		while (gap > 0)
		{
			gap -= (int)_increaseValue;

			_displayedCount += (int)(_increaseValue * direction);

			UpdateText(_displayedCount);

			yield return new WaitForSeconds(_time);

			_increaseValue *= _increaseRatio;
		}

		_displayedCount = value;
		UpdateText(_displayedCount);

		if (onEndCounting != null)
		{
			onEndCounting(direction > 0);
		}

		_countingCoroutine = null;
	}
	#endregion COROUTINES
}