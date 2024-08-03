using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFadeCtrl : BaseObject
{
    public float fadeSpeed = 1f;

    private Light _light;
    private float _initIntensity;
    private float _curIntensity;

    private bool _isFadeIn = false;
    private bool _isFadeOut = false;

    private void Start()
    {
        _light = GetComponent<Light>();
        _initIntensity = _light.intensity;
    }

    private void Update()
    {
        if (_isFadeIn)
        {
            if (_curIntensity < _initIntensity)
            {
                _curIntensity += Time.deltaTime * fadeSpeed;
                _light.intensity = _curIntensity;
            }
            else
            {
                _isFadeIn = false;
            }
        }
        else if (_isFadeOut)
        {
            if (_curIntensity > 0)
            {
                _curIntensity -= Time.deltaTime * fadeSpeed;
                _light.intensity = _curIntensity;
            }
            else
            {
                _isFadeOut = false;
            }
        }
    }

    public void FadeIn()
    {
        _curIntensity = 0;
        _isFadeIn = true;
    }

    public void FadeOut()
    {
        _curIntensity = _initIntensity;
        _isFadeOut = true;
    }

    public void SetColor(Color color)
    {
        _light.color = color;
    }

    public override void Dispose()
    {

    }
}