using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BirthdayInputItem : MonoBehaviour {

    [SerializeField]
    private TMP_Text _text;

    private string _number;
    private Action<string> _onSelected;

    public void SetItem(string number, Action<string> onSelected)
    {
        _number = number;
        _text.text = _number.ToString();
        _onSelected = onSelected;
    }


    public void OnSelected()
    {
        if (_onSelected != null)
            _onSelected(_number);
    }

}
