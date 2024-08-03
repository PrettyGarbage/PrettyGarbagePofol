using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class GizmosUtilities
{
    public static void DrawString(string text, Vector3 position, Color? color = null)
    {
#if UNITY_EDITOR
        Handles.BeginGUI();

        GUI.color = color ?? Color.white;

        SceneView view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPosition = view.camera.WorldToScreenPoint(position);
        Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(text));
        Vector2 labelPosition = new Vector2(screenPosition.x - (labelSize.x * 0.5f), -screenPosition.y + view.position.height);

        GUI.Label(new Rect(labelPosition, labelSize), text);

        Handles.EndGUI();
#endif
    }

    public static void DrawWireHexagon(Vector3 position, Hexagon.Orientation orientation, float outerRadius, Color? color = null)
    {
        Gizmos.color = color ?? Color.white;

        Vector2[] points = Hexagon.GetCorners(orientation, outerRadius);
        for (int i = 0; i < points.Length; ++i)
        {
            Gizmos.DrawLine(position + (Vector3)points[i], position + (Vector3)points[(i + 1) % points.Length]);
        }

    }
}