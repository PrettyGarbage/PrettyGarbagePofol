using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleImageData))]
public class PuzzleImageDataEditor : Editor {

    SerializedObject serializedObject;
    
    void OnEnable()
    {
        PuzzleImageData myTarget = (PuzzleImageData)target;
        serializedObject = new UnityEditor.SerializedObject(myTarget);
    }

    // public override void OnInspectorGUI()
    // {
    //     serializedObject.Update();

    //     SerializedProperty idProperty  = serializedObject.FindProperty("_id");
    //     SerializedProperty puzzleThemeDataProperty  = serializedObject.FindProperty("_puzzleThemeData");
    //     SerializedProperty imageSpriteProperty  = serializedObject.FindProperty("_imageSprite");
    //     SerializedProperty puzzleImagePartsProperty  = serializedObject.FindProperty("_puzzleImageParts");
        
    //     Sprite sp = (Sprite)imageSpriteProperty.objectReferenceValue;
        
    //     //EditorGUILayout.RectField(new Rect(0,0,0,0));
    //     // Rect rect = GUILayoutUtility.GetLastRect();
    //     // rect.width = sp.texture.width/3;
    //     // rect.height = sp.texture.height * (rect.width/sp.texture.height);
    //     // EditorGUI.DrawPreviewTexture(rect, sp.texture);
        

    //     idProperty.stringValue = EditorGUILayout.TextField("ID", idProperty.stringValue);
        
    //     puzzleThemeDataProperty.objectReferenceValue = EditorGUILayout.ObjectField("ThemeData", puzzleThemeDataProperty.objectReferenceValue, typeof(PuzzleThemeData));
    //     // GUILayoutOption[] options = new [] {
    //     //     GUILayout.ExpandWidth(true)
    //     // };
        
    //     imageSpriteProperty.objectReferenceValue = EditorGUILayout.ObjectField("MainSprite", imageSpriteProperty.objectReferenceValue, typeof(Sprite));
        
    //     // myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
    //     // EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
    //     for (int i = 0; i < puzzleImagePartsProperty.CountInProperty(); i++)
    //     {
    //         var partData = puzzleImagePartsProperty.GetArrayElementAtIndex(i);
           
    //     }

    //     EditorGUILayout.PropertyField(puzzleImagePartsProperty, true);
        
    //     serializedObject.ApplyModifiedProperties();
    // }
}
