using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteFont))]
public class SpriteFontEditor : Editor
{
    void OnEnable()
    {
        
    }
    public override void OnInspectorGUI()
    {
        SpriteFont myTarget = (SpriteFont)target;
        myTarget.Text = EditorGUILayout.TextField("text", myTarget.Text);
        myTarget.WordSpace = EditorGUILayout.FloatField("wordSpace", myTarget.WordSpace);
        myTarget.FontSize = EditorGUILayout.FloatField("fontSize", myTarget.FontSize);
        myTarget.Color = EditorGUILayout.ColorField("Color", myTarget.Color);
        myTarget.AlignType = (SpriteFont.SpriteFontAlignType)EditorGUILayout.EnumPopup("AlignType", myTarget.AlignType);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sprte Info");
        int size = EditorGUILayout.IntField("Size", myTarget.SpriteInfoArray.Length);

        EditorUtility.SetDirty(myTarget);

        if (size!= myTarget.SpriteInfoArray.Length)
        {
            SpriteInfo[] newSpriteInfoArray = new SpriteInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (myTarget.SpriteInfoArray.Length > i)
                {
                    newSpriteInfoArray[i] = myTarget.SpriteInfoArray[i];
                }
            }

            myTarget.SpriteInfoArray = newSpriteInfoArray;
        }

        for (int i = 0; i < myTarget.SpriteInfoArray.Length; i++)
        {
            myTarget.SpriteInfoArray[i].text = EditorGUILayout.TextField( i + " text ", myTarget.SpriteInfoArray[i].text);
            myTarget.SpriteInfoArray[i].sprite = (Sprite)EditorGUILayout.ObjectField(i + " sprite " , myTarget.SpriteInfoArray[i].sprite, typeof(Sprite));
        }

    }
}
