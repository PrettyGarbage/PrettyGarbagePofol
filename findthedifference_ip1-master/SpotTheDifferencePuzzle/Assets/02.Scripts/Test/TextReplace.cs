using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextReplace : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string pp = "Theme {0} Normal Clear";

        pp = pp.Replace("{0}", "gggg");

        Debug.Log(pp);

    }
	
}
