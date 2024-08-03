using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "BoardThemeDB.asset", menuName = "gbros/Theme/Board Theme DB", order = 1)]
#endif
[Serializable]
public class BoardThemeDB : ScriptableObject
{
#if UNITY_EDITOR
	public const string PATH = "Assets/Resources/Data/Theme/BardThemeDB.asset";

	public static BoardThemeDB CreateAsset()
	{
		BoardThemeDB themeDB = CreateInstance<BoardThemeDB>();

		AssetDatabase.CreateAsset(themeDB, PATH);
		AssetDatabase.Refresh();

		return themeDB;
	}
#endif

	[SerializeField] BoardTheme[] _themes;

	public BoardTheme GetTheme(string name)
	{
		return Array.Find(_themes, t => string.Compare(t.name, name) == 0);
	}

	public BoardTheme GetTheme(int index)
	{
		return (index >= 0 && index < _themes.Length) ? _themes[index] : null;
	}

	public BoardTheme[] themes
	{
		get
		{
			return _themes;
		}
	}
}