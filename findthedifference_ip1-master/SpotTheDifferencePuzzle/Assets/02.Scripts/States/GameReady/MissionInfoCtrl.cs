using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionInfoCtrl : MonoBehaviour {

    [SerializeField]
    private TMP_Text _findDiffCountText;

    [SerializeField]
    private GameObject _checkObject;

    public void SetMissionInfo(int findDiffCount, bool isRewarded)
    {
        _findDiffCountText.text = "x " + findDiffCount.ToString();
        _checkObject.SetActive(isRewarded);
    }

	
}
