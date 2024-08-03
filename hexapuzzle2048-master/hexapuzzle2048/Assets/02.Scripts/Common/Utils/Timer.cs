using UnityEngine;
using System.Collections;
using System;

public class Timer : BaseObject
{

    private Action OnTimerEvent;

    private float _intervalTime;
    private float _initIntervalTime;

    private float _curTime = 0f;

    private bool _isRunning = false;

    private bool _isOnce = true;

    public bool IsStop
    {
        get
        {
            return !_isRunning;
        }
    }

    public float CurTime
    {
        get
        {
            return _curTime;
        }
    }

    void Update()
    {

        if (_isRunning)
        {
            if (_curTime > _intervalTime)
            {
                if (_isOnce)
                {
                    _isRunning = false;
                }
                else
                {
                    Reset();
                }

                if (OnTimerEvent != null) OnTimerEvent();
            }
            else
            {
                _curTime += Time.deltaTime;
            }
        }
    }

    public void Set(float intervalTime, Action callback = null, bool isOnce = false)
    {
        _intervalTime = intervalTime;
        _initIntervalTime = intervalTime;
        OnTimerEvent = callback;
        _curTime = 0f;
        _isOnce = isOnce;
    }

    public void Reset()
    {
        _curTime = 0f;
        _isRunning = true;
    }

    public void StartTimer(bool isReset = false)
    {
        //Debug.Log("start timer");
        if (isReset)
        {
            Reset();
        }
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ChangeIntervalTime(float time)
    {
        _intervalTime = time;
    }

    public float GetCurIntervalTime()
    {
        return _intervalTime;
    }

    public void ReturnIntervalTime()
    {
        _intervalTime = _initIntervalTime;
    }

    public override void Dispose()
    {
        OnTimerEvent = null;
    }
}