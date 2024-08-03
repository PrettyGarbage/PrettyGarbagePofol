using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
	GameManager _manager;

	void OnEnable()
	{
		_manager = target as GameManager;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_themeDB"));
		DrawSelectedThemeField(serializedObject.FindProperty("_selectedTheme"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_hexaCellPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_board"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_block"));
		DrawSingleCellBlockLimitsField(serializedObject.FindProperty("_singleCellBlockLimitNotGenerated"));
		DrawIgnoreBlockCellField(serializedObject.FindProperty("_ignoreBlockCellNumbers"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayNumberToPower"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_restoreBoardTime"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_destroyCellTime"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxContinueCount"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxContinueExplodeRadius"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_debug"));

		if (serializedObject.ApplyModifiedProperties())
		{
			EditorUtility.SetDirty(_manager.gameObject);
		}
	}

	void DrawSelectedThemeField(SerializedProperty property)
	{
		BoardTheme[] themes = _manager.themeDB.themes;
		List<string> themeNames = new List<string>();
		for (int i = 0, max = themes.Length; i < max; ++i)
		{
			themeNames.Add(themes[i].name);
		}

		int selectTheme =
			EditorGUILayout.Popup("Theme", property.intValue, themeNames.ToArray());
		if (selectTheme != property.intValue)
		{
			property.intValue = selectTheme;

			if (Application.isPlaying)
			{
				_manager.ChangeTheme(themeNames[selectTheme]);
			}
		}
	}

	void DrawSingleCellBlockLimitsField(SerializedProperty property)
	{
		int[] numbers = Enumerable.Range(
			GameConstants.LOWEST_CELL_NUMBER,
			GameConstants.HIGHEST_CELL_NUMBER - GameConstants.LOWEST_CELL_NUMBER + 1
		).ToArray();

		List<string> numberStrings = new List<string>();
		numberStrings.Add("None");
		numberStrings.AddRange(numbers.Select(x => Mathf.Pow(2, x).ToString()).ToArray());
		
		int selectedIndex = Array.FindIndex(numbers, v => property.intValue == v);
		if (selectedIndex < 0)
		{
			selectedIndex = 0;
		}
		else
		{
			++selectedIndex;
		}

		int selectIndex =
			EditorGUILayout.Popup("Single Block Limit Not Generated", selectedIndex, numberStrings.ToArray());
		if (selectIndex != selectedIndex)
		{
			--selectIndex;

			property.intValue = (selectIndex < 0) ? 0 : numbers[selectIndex];
		}
	}

	void DrawIgnoreBlockCellField(SerializedProperty property)
	{
		int[] numbers = Enumerable.Range(
			GameConstants.LOWEST_CELL_NUMBER + 1,
			GameConstants.HIGHEST_CELL_NUMBER - GameConstants.LOWEST_CELL_NUMBER
		).ToArray();

		string[] numberStrings = numbers.Select(x => Mathf.Pow(2, x).ToString()).ToArray();

		int ignoredMask = (property.intValue >> (GameConstants.LOWEST_CELL_NUMBER + 1));
		int ignoreMask =
			EditorGUILayout.MaskField("Ignore Block Cells", ignoredMask, numberStrings.ToArray());
		if (ignoredMask != ignoreMask)
		{
			property.intValue = (ignoreMask << (GameConstants.LOWEST_CELL_NUMBER + 1));
		}
	}
}
