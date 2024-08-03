using System;

using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(StateManager))]
public class StateManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawStartStateSelector();
        //DrawStateList();
        //DrawLoadingScreenOptions();

        //serializedObject.ApplyModifiedProperties();
    }

    void DrawStartStateSelector()
    {
        StateManager manager = target as StateManager;

        SerializedProperty startStateName = serializedObject.FindProperty("_startStateName");
        string[] stateNames = manager.stateNames;
        if (stateNames.Length > 0)
        {
            EditorGUILayout.Space();

            int selectedIndex = Array.IndexOf(stateNames, startStateName.stringValue);
            int selectIndex = EditorGUILayout.Popup("Start State", Mathf.Max(0, selectedIndex), stateNames);
            if (selectedIndex != selectIndex)
            {
                startStateName.stringValue = stateNames[selectIndex];
            }
        }
    }

    void DrawStateList()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_states"), true);
    }

    void DrawLoadingScreenOptions()
    {
        StateManager manager = target as StateManager;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_loadingScreen"), new GUIContent("Screen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_loadingScreenAppearTime"), new GUIContent("Appear Time"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_loadingScreenMinMaintainTime"), new GUIContent("Min Maintain Time"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_blockScreen"), new GUIContent("Screen"));
    }
}