using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexaBoard))]
public class HexaBoardEditor : Editor
{
	HexaBoard _board;

	void OnEnable()
	{
		_board = target as HexaBoard;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_tilePrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_type"));

		switch (_board.type)
		{
			case CombinableHexaBoard.Type.Hexagon:
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_hexagonDiameter"));
				}
				break;

			case CombinableHexaBoard.Type.Rectangle:
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_rectangleWidth"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("_rectangleHeight"));
				}
				break;
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_horizontalAlign"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_verticalAlign"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_spacing"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("_hideWhenAwake"));

		GUI.enabled = false;
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_boardWidth"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_boardHeight"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_tiles"), true);

		EditorGUILayout.Space();

		GUI.enabled = _board.tilePrefab != null && !Application.isPlaying;

		EditorGUILayout.BeginHorizontal();
		{
			Color prevColor = GUI.color;

			GUI.color = Color.green;
			if (GUILayout.Button("Generate", EditorStyles.miniButtonLeft))
			{
				_board.GenerateTiles();

				EditorUtility.SetDirty(_board.gameObject);
			}

			GUI.color = Color.red;
			if (GUILayout.Button("Clear", EditorStyles.miniButtonRight))
			{
				_board.ClearTiles();

				EditorUtility.SetDirty(_board.gameObject);
			}

			GUI.color = prevColor;
		}
		EditorGUILayout.EndHorizontal();

		GUI.enabled = true;

		EditorGUILayout.Space();

		if (serializedObject.ApplyModifiedProperties())
		{
			_board.GenerateTiles();

			EditorUtility.SetDirty(_board.gameObject);
		}
	}
}