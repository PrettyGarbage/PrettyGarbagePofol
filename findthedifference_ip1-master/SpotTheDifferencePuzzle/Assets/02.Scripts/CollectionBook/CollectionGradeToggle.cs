using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CollectionGradeToggle : MonoBehaviour {

    private Toggle _toggle;
    private CollectionGradeType _collectionGradeType;

    private Action<CollectionGradeType> _onValueChanged;

    public Toggle Toggle { get { return _toggle; } }

    public void SetToggle(CollectionGradeType collectionGradeType, Action<CollectionGradeType> onValueChanged)
    {
        _onValueChanged = onValueChanged;
        _toggle = GetComponent<Toggle>();
        _collectionGradeType = collectionGradeType;
        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool selected)
    {
        Debug.Log("OnValueChanged " + name + "/ selected: " + selected);
        if (selected)
        {
            SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_button_common);
            _onValueChanged(_collectionGradeType);
        }
    }
}
