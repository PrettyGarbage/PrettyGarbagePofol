using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorUtilities
{
	public static void DrawHorizontalLine(Color color, int thickness = 1, int padding = 10)
	{
		Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness + padding));
		rect.y += padding;
		rect.height = thickness;		

		EditorGUI.DrawRect(rect, color);
	}
}