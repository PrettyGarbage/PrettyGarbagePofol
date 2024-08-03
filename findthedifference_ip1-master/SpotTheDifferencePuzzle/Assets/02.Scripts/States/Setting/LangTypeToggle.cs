using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangTypeToggle : MonoBehaviour {

    [SerializeField]
    private LangType _langType;

    [SerializeField]
    private Toggle _toggle;

    [SerializeField]
    private GameObject _selectedObject;

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool isSelected)
    {
        Debug.Log("OnValueChanged : " + name + " / isSelected : " + isSelected);
        _selectedObject.SetActive(isSelected);

        if (isSelected)
        {
            SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_toggle_on);
            SettingManager.Instance.SetLangType(_langType);
        }

    }

    private void OnEnable()
    {
        _toggle.isOn = SettingManager.Instance.LangType == _langType;
    }


}
