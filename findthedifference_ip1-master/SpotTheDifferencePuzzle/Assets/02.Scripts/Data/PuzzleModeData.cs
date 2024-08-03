using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleModeData.asset", menuName = "gbros/PuzzleModeData", order = 1)]
public class PuzzleModeData : ScriptableObject{

    [SerializeField]
	private ThemeDifficulty  _playMode;
    [SerializeField]
    private float _limitTime;
    [SerializeField]
    private int _findMinCount;
    [SerializeField]
    private int _findMaxCount;
    [SerializeField]
    private FindMixRule _findMixRule;
    [SerializeField]
    private float _roundTimeBonus;
    [SerializeField]
    private float _roundTimePenalty;
    [SerializeField]
    private float _adTimeBonus;

    public ThemeDifficulty  PlayMode
    {
        get
        {
            return _playMode;
        }
    }

    public float LimitTime
    {
        get
        {
            return _limitTime;
        }
    }

    public int FindMinCount
    {
        get
        {
            return _findMinCount;
        }
    }

    public int FindMaxCount
    {
        get
        {
            return _findMaxCount;
        }
    }

    public FindMixRule FindMixRule
    {
        get
        {
            return _findMixRule;
        }
    }

    public float RoundTimeBonus
    {
        get
        {
            return _roundTimeBonus;
        }

    }

    public float RoundTimePenalty
    {
        get
        {
            return _roundTimePenalty;
        }
    }

    public float AdTimeBonus
    {
        get
        {
            return _adTimeBonus;
        }
    }
}
