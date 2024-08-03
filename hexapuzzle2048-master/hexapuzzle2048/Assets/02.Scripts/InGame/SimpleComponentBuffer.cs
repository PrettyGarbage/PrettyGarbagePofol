using System;
using System.Collections.Generic;

using UnityEngine;

class SimpleComponentBuffer<T> where T : Component
{
	public SimpleComponentBuffer(T prefab, int size, Transform root, Action<T> initializer = null)
	{
		_prefab = prefab;
		_root = root;
		_initializer = initializer;

		Mint(size);
	}

	public void Mint(int size)
	{
		if (_prefab == null)
		{
			return;
		}

		for (int i = 0, max = Mathf.Max(1, size); i < max; ++i)
		{
			T t = GameObject.Instantiate(_prefab);
			t.name = _prefab.name;

			t.gameObject.SetActive(false);

			t.transform.SetParent(_root);
			t.transform.localRotation = Quaternion.identity;
			t.transform.localPosition = Vector3.zero;
			t.transform.localScale = Vector3.one;

			if (_initializer != null)
			{
				_initializer(t);
			}

			_components.Add(t);
		}
	}

	public void Withdraw()
	{
		for (int i = 0, max = _components.Count; i < max; ++i)
		{
			_components[i].gameObject.SetActive(false);
		}
	}

	public T Pop()
	{
		if (_components.Count <= 0)
		{
			return null;
		}

		T component = _components[_index % _components.Count];
		component.gameObject.SetActive(true);

		++_index;

		return component;
	}

	T _prefab;
	Transform _root;
	Action<T> _initializer;

	List<T> _components = new List<T>();
	int _index = 0;
}
