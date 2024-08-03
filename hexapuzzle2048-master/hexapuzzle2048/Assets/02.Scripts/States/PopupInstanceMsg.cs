using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//massege
public class PopupInstanceMsg : State
{
    const string paramMessageKey = "massege";
    const string paramDurationKey = "duration";
    const string paramInstanceMsgTypeKey = "instanceMsgType";
    
    public override string stateName { get { return GameConstants.STATENAME_POPUPINSTANCEMAG; } }

    [SerializeField]
    private TMP_Text _messageText;
    [SerializeField]
    private Color _normalTypeColor;
    [SerializeField]
    private Color _warningTypeColor;

    private float _duration;
    private InstanceMsgType _instanceMsgType;

    public static void ShowPopupInstanceMsg(string message, float duration, InstanceMsgType instanceMsgType){
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add(paramMessageKey, message);
        dic.Add(paramDurationKey, duration);
        dic.Add(paramInstanceMsgTypeKey, instanceMsgType);
        StateManager.instance.PushState(GameConstants.STATENAME_POPUPINSTANCEMAG, dic);
    }

    public override IEnumerator OnInitialize()
    {
        yield return base.OnInitialize();
    }

    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
    {
        _messageText.text = args[paramMessageKey].ToString();
        _duration = (float)args[paramDurationKey];
        _instanceMsgType = (InstanceMsgType)args[paramInstanceMsgTypeKey];
        _messageText.color = _instanceMsgType == InstanceMsgType.WARNING ? _warningTypeColor : _normalTypeColor;
        
        yield return base.OnLoad(args);
    }

    public override IEnumerator OnEntered()
    {
        yield return base.OnEntered();

        LeanTween.delayedCall(_duration,()=>OnBack());
    }    
}
