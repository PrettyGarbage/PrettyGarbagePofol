using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleAnimationPlayer : BaseObject
{

    private Animator _animation;

    //public float _delay = 0f;
    public float _speed = 1f;
    public bool _isRamdom = true;

    private void Awake()
    {
        _animation = GetComponent<Animator>();
        _animation.speed = _isRamdom ? UnityEngine.Random.Range(0.3f, 1.5f) : _speed;

        //DelayCall.Instance.Call(_delay, new Callback((object[] p) => {

        //}));

    }

    private void OnEnable()
    {

    }

    public override void Dispose()
    {

    }

}