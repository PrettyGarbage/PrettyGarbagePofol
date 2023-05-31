using System;
using System.Collections;
using System.Collections.Generic;
using EzNetLibrary;
using UniRx;
using UnityEngine;

public class EzNetTest : MonoBehaviour
{
    EzNetObject netObject;
    
    void Awake()
    {
        netObject = GetComponent<EzNetObject>();

        Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            EzNet.Add("Test", "Hello, EzNet!");
            netObject.SendBroadcast(EzNet.WhereSpace.AllSpace, nameof(Test));
        }).AddTo();
    }
    
    public void Test()
    {
        var message = EzNet.Read<string>("Test");
        Debug.Log(message);
    }
}
