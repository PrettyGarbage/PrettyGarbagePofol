using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadingCharCtrl : MonoBehaviour {

    [SerializeField]
    private GameObject[] _charObjects;

    private void OnEnable()
    {
        for (int i = 0; i < _charObjects.Length; i++)
        {
            _charObjects[i].SetActive(false);
        }
        _charObjects[Random.Range(0, _charObjects.Length)].SetActive(true); 
    }

}
