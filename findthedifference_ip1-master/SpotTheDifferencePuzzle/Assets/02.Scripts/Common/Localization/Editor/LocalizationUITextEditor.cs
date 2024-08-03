using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationUIText))]
public class LocalizationUITextEditor : Editor
{

    public string searchKey = string.Empty;
    string[] _keyNames;
    List<LocalizationTextKey> _searchKeyResult;

    private void OnEnable()
    {
        //LocalizationManager.Instance.GetText(LangType.KR, LocalizationTextKey.COMMON_ALERT_TITLE,null);
    }

    public override void OnInspectorGUI()
    {
        LocalizationUIText localizationUIText = (LocalizationUIText)target;
        LangType langType = (LangType) EditorGUILayout.EnumPopup("Lang Type", localizationUIText.LangType);
        EditorGUI.BeginDisabledGroup(true);
        string localizationTextKeyString = EditorGUILayout.TextField("LocalizationTextKey", localizationUIText.LocalizationTextKeyString);
        EditorGUI.EndDisabledGroup();

        searchKey = EditorGUILayout.TextField("Key Search", searchKey);
        SearchKey();

        if (GUILayout.Button("재설정"))
        {
            localizationUIText.SetTextForEditor(langType, (LocalizationTextKey)Enum.Parse(typeof(LocalizationTextKey), localizationTextKeyString));
        }

        //검색 표시.
        for (int i = 0; i < _searchKeyResult.Count; i++)
        {
            string text = LocalizationManager.Instance.GetText(_searchKeyResult[i]);
            EditorGUILayout.BeginHorizontal();
            string buttonText = _searchKeyResult[i].ToString() + " | " + text.Substring(0, text.Length>15?15 : text.Length);
            if (GUILayout.Button(buttonText))
            {
                localizationUIText.LocalizationTextKeyString = _searchKeyResult[i].ToString();
                localizationTextKeyString = _searchKeyResult[i].ToString();
                localizationUIText.SetTextForEditor(langType, _searchKeyResult[i]);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        //문자 표시.
        if (localizationUIText.LangType != langType )
        {  
            localizationUIText.SetTextForEditor(langType, (LocalizationTextKey)Enum.Parse(typeof(LocalizationTextKey), localizationTextKeyString));            
        }
        localizationUIText.LangType = langType;
        localizationUIText.LocalizationTextKeyString = localizationTextKeyString;

    }

    public void SearchKey()
    {
        if (_keyNames == null)
        {
            _keyNames =  Enum.GetNames(typeof(LocalizationTextKey));
            _searchKeyResult = new List<LocalizationTextKey>();
        }

        _searchKeyResult.Clear();

        if (searchKey.Length <= 0) return;

        for (int i = 0; i < _keyNames.Length; i++)
        {
            if (_keyNames[i].Contains(searchKey.ToUpper()))
            {
                _searchKeyResult.Add((LocalizationTextKey)Enum.Parse(typeof(LocalizationTextKey), _keyNames[i]));
            }
        }
    }

}
