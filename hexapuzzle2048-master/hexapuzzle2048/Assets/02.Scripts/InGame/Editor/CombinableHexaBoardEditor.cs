using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CombinableHexaBoard))]
public class CombinableHexaBoardEditor : HexaBoardEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorUtilities.DrawHorizontalLine(Color.grey, 1, 0);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_combineType"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_minCombineCellSize"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_cellCombineDuration"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_cellCombineInterval"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_combineEffectPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_explodeEffectPrefab"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_useRemoveHighestNumber"));

		if (serializedObject.ApplyModifiedProperties())
		{
			EditorUtility.SetDirty(target);
		}
	}
}