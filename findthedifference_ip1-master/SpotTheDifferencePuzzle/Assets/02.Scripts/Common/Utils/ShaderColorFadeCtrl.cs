using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderColorFadeCtrl : BaseObject
{

    public string colorProperty;
    public Color originColor;
    public Color fadeColor;
    private Color _curColor;
    public float fadeTime = 1f;

    public float fadeDelay = 0f;
    public int matIndex = 0;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _curColor = originColor;
    }
    //_SpecColor
    private void Start()
    {
        //FadeIn();

        //DelayCall.Instance.Call(3f, new Callback((object[] p) => {
        //    FadeOut();
        //}));
    }

    public void FadeIn()
    {
        //iTween.Stop(gameObject);
        //iTween.ValueTo(gameObject, iTween.Hash("from", _curColor, "to", fadeColor, "time", fadeTime, "easetype", "easeInCubic", "onUpdate", (Action<object>)(newColor => UpdateColor((Color)newColor, matIndex)), "onUpdateParams", colorProperty));
        //iTween.ColorTo(gameObject, iTween.Hash("color", fadeColor, "time", fadeTime, "namedcolorvalue", colorProperty, "easetype", "easeInCubic", "delay", fadeDelay));
    }

    public void FadeOut()
    {
        //iTween.Stop(gameObject);
        //iTween.ValueTo(gameObject, iTween.Hash("from", _curColor, "to", originColor, "time", fadeTime, "easetype", "easeInCubic", "onUpdate", (Action<object>)(newColor => UpdateColor((Color)newColor, matIndex)), "onUpdateParams", colorProperty));
        //iTween.ColorTo(gameObject, iTween.Hash("color", originColor, "time", fadeTime, "namedcolorvalue", colorProperty, "easetype", "easeInCubic", "delay", fadeDelay));
    }

    private void UpdateColor(Color newColor, int pmatIndex)
    {
        if (this.matIndex == pmatIndex)
        {
            _curColor = newColor;
            ObjectUtil.ChangeMeshRendererColor(_renderer, this.matIndex, newColor, colorProperty);
        }

    }

    public override void Dispose()
    {

    }

}