using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestEventButton : MonoBehaviour {

    [SerializeField]
    private Button _button;
    [SerializeField]
    private TMP_Text _titleText;

    public void SetEvent(TestEventInfo testEventInfo)
    {
        _titleText.text = testEventInfo.title;
        _button.onClick.AddListener(testEventInfo.unityAction);

        gameObject.SetActive(true);
    }
}
