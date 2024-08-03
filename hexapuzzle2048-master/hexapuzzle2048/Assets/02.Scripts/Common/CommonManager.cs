using System.Collections;
using System.Collections.Generic;
using LunarConsolePlugin;
using TMPro;
using UnityEngine;

public class CommonManager : MonoBehaviour {

	private static CommonManager _instance;
    public static CommonManager Instance { get { return _instance; } }

    public bool IsInitComplete
    {
        get
        {
            return _isInitComplete;
        }
    }

    private bool _isInitComplete;

    void Awake()
    {
        if(CommonManager.Instance){
            Destroy(gameObject);
        }else {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
    }

	public IEnumerator Init(){

        LunarConsole.RegisterAction("Forced GameOver", ()=>{
            GameManager.instance.onGameOver();
        });

        if(_isInitComplete) yield break;
        Debug.Log("CommonManager Init Start");
		yield return StartCoroutine(BuildManager.Instance.Init());
        yield return StartCoroutine(DataManager.Instance.Init());
        yield return StartCoroutine(SoundManager.Instance.Init());
        yield return StartCoroutine(NetworkManager.Instance.Init());
        Debug.Log("CommonManager Init End");
        _isInitComplete = true;
	}

}
