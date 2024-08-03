using System;
using System.Collections;
using System.Collections.Generic;
using LunarConsolePlugin;
using UnityEngine;

public class TestLunarConsole : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LunarConsole.RegisterAction("My Action 1", Action1);
        
	}

    private void Action1()
    {
        NetworkManager.Instance.SignOut();
    }

}
