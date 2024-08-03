using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderEmissionCtrl : BaseObject
{
    enum Fade
    {
        None,
        In,
        Out,
    }

    const string EMISSION_KEY = "_EMISSION";
    const string EMISSION_COLOR_KEY = "_EmissionColor";

    public float emissionFromValue = 0f;
    public float emissionToValue = 2f;
    public float fadeTime = 1f;

    Renderer _renderer;
    Material _material;
    Color _baseColor;

    Fade _fade = Fade.None;
    float _emissionValue = 0.0f;
    bool _isLoop = false;

    #region UNITY EVENTS
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _baseColor = _material.GetColor(EMISSION_COLOR_KEY);

        //
        UpdateEmission(emissionFromValue);

        _material.EnableKeyword(EMISSION_KEY);
    }

    private void Update()
    {
        switch (_fade)
        {
            case Fade.In: { UpdateFadeIn(); } break;
            case Fade.Out: { UpdateFadeOut(); } break;
        }
    }
    #endregion UNITY EVENTS

    public override void Dispose()
    {
    }

    public void FadeIn(Color baseColor)
    {
        _fade = Fade.In;
        _baseColor = baseColor;
        _emissionValue = emissionFromValue;
    }

    public void FadeIn()
    {
        _fade = Fade.In;
        _emissionValue = emissionFromValue;
    }

    public void FadeOut()
    {
        _fade = Fade.Out;
        _emissionValue = emissionToValue;
    }

    public void Stop()
    {
        _fade = Fade.None;
    }

    public bool isLoop
    {
        get
        {
            return _isLoop;
        }

        set
        {
            _isLoop = value;
        }
    }

    void UpdateFadeIn()
    {
        _emissionValue += Time.deltaTime * 1.0f / fadeTime;

        UpdateEmission(_emissionValue);

        if (_emissionValue < emissionToValue)
        {
            return;
        }

        _fade = Fade.None;

        if (isLoop)
        {
            FadeOut();
        }
    }

    void UpdateFadeOut()
    {
        _emissionValue -= Time.deltaTime * 1.0f / fadeTime;

        UpdateEmission(_emissionValue);

        if (_emissionValue >= emissionFromValue)
        {
            return;
        }

        _fade = Fade.None;

        if (isLoop)
        {
            FadeIn();
        }
    }

    void UpdateEmission(float value)
    {
        _material.SetColor("_EmissionColor", _baseColor * Mathf.LinearToGammaSpace(value));
    }
}