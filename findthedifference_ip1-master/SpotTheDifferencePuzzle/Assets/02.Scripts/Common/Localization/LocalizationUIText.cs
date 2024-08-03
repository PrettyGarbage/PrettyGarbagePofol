using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class LocalizationUIText : MonoBehaviour {

    [SerializeField]
    private LangType _langType;
    
    [SerializeField]
    private string _localizationTextKeyString = string.Empty;

    private TMP_Text _text;
    private string[] _textParams;

    private string[] _keyParams;

    public LangType LangType { get { return _langType; } set { _langType = value; } }

    public string LocalizationTextKeyString { get { return _localizationTextKeyString; } set { _localizationTextKeyString = value; } }

    public void SetText(string key)
    {
        try
        {
            LocalizationTextKey localizationTextKey = (LocalizationTextKey)Enum.Parse(typeof(LocalizationTextKey), key);
            if (_keyParams != null)
            {
                SetTextWithKeys(localizationTextKey, _keyParams);
            }
            else
            {
                SetText(localizationTextKey, _textParams);
            }
        }
        catch (Exception)
        {
            Debug.Log("Not Found key : " + key);
            _localizationTextKeyString = string.Empty;
        }
        
    }

    public void SetTextWithKeys(LocalizationTextKey localizationTextKey, string[] args)
    {
        _keyParams = args;
        string[] strArgs = new string[_keyParams.Length];
        for (int i = 0; i < _keyParams.Length; i++)
        {
            strArgs[i] = LocalizationManager.Instance.GetText(_keyParams[i]);
        }
        SetText(localizationTextKey, strArgs);
    }

    public void SetText(LocalizationTextKey localizationTextKey, params string[] args)
    {
        if (!LocalizationManager.Instance.IsInitailized()) return;

        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }

        _localizationTextKeyString = localizationTextKey.ToString();
        _text.font = LocalizationManager.Instance.GetFontAsset();
        string text =  LocalizationManager.Instance.GetText(localizationTextKey);

        _textParams = args;
        if (_textParams != null)
        {
            for (int i = 0; i < _textParams.Length; i++)
            {
                text = text.Replace("{" + i + "}", _textParams[i]);
            }
        }
        
        _text.text = text;
    }

    public void SetTextForEditor(LangType langType, LocalizationTextKey localizationTextKey)
    {

        if (Application.isPlaying) return;

        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }
        _localizationTextKeyString = localizationTextKey.ToString();
        _text.font = LocalizationManager.Instance.GetFontAsset(langType);
        LocalizationManager.Instance.GetText(langType, localizationTextKey, text=> {
            _text.text = text;
        });
    }

    private void OnEnable()
    {

        SetText(_localizationTextKeyString);
        EventPool.Listen(GetInstanceID(), EventNames.ON_CHANGE_LANG_TYPE, OnChageLangType);
    }

    private void OnDisable()
    {
        EventPool.Remove(GetInstanceID(), EventNames.ON_CHANGE_LANG_TYPE, OnChageLangType);
    }

    private void OnChageLangType(object[] args)
    {
        SetText(_localizationTextKeyString);
    }
}
