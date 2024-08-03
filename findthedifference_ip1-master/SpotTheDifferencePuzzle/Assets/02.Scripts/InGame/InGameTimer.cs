using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class InGameTimer : MonoBehaviour
{
	const string FX_INCREASE = "eff_time_refill";
	const string FX_DECREASE = "eff_time_penalty";

	const float WARNING_TRANSITION_TIME = 0.2f;

	[SerializeField] Image _timeGauge;
	[SerializeField] Color _defaultGaugeColor = Color.green;
	[SerializeField] Color _warningGaugeColor = Color.red;
	[SerializeField, Range(0.0f, 100.0f)] float _warningPercentage = 10.0f;
	public bool ignoreTimeScale = false;
    public bool _isContinue;

    [Header("Timer UI")]
    [SerializeField] SpriteFont _timeText;
    [SerializeField] GameObject _extraTimeObj;

	[Header("Increase")]
	[SerializeField] Image _increaseGauge;
	[SerializeField] float _maxIncreaseTime = 1.0f;
	[SerializeField] float _increasingDelay = 0.1f;
    [SerializeField] SpriteFont _increaseText;
    [SerializeField] GameObject _increaseTxtObj;

	[Header("Decrease")]
	[SerializeField] Image _decreaseGauge;
	[SerializeField] float _maxDecreaseTime = 1.0f;
	[SerializeField] float _decreasingDelay = 0.1f;
    [SerializeField] SpriteFont _decreaseText;
    [SerializeField] GameObject _decreaseTxtObj;

    //[Header("ExtraTime")]
    //[SerializeField] GameObject _extraTimeObj;


    [Header("Events")]
	public UnityEvent onTimeOver = new UnityEvent();
	[HideInInspector] public UnityEvent onTimeWarning = new UnityEvent();
	[HideInInspector] public UnityEvent onTimeWarningReleased = new UnityEvent();

	public float remainingTime { get; private set; }
    public float extraTime { get; private set; }
	public float maxTime { get; private set; }
	public bool isPaused { get; private set; }

	int _processedGaugeCount = 0;
	float _prevGaugeRatio = 1.0f;
    bool _isExtraTime = false;
    float _extraTimeGaugeRatio;
    float _timeSpan = 0;
    int _correctStack = 0;

	#region UNITY EVENTS
	void Update()
	{
		if (isPaused || nowChanging)
		{
			return;
		}

        if (_isExtraTime)
        {
            extraTime -= Time.deltaTime;

            _extraTimeObj.GetComponentInChildren<SpriteFont>().Text = ((int)extraTime).ToString();

            if (extraTime <= 0)
            {
                _isExtraTime = false;
                _extraTimeObj.SetActive(false);
            }

        }

        if (remainingTime > 0.0f && !_isExtraTime)
		{
            _timeText.Text = ((int)remainingTime).ToString();

            remainingTime -= ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            _timeSpan += Time.deltaTime;

            float gaugeRatio = remainingTime / maxTime;

            RandomVoice(gaugeRatio);

            if (_prevGaugeRatio > warningGaugeRatio &&
				gaugeRatio <= warningGaugeRatio)
			{
				_timeGauge.CrossFadeColor(_warningGaugeColor, WARNING_TRANSITION_TIME, true, false);

				onTimeWarning.Invoke();
			}

			_prevGaugeRatio = gaugeRatio;
			_timeGauge.fillAmount = gaugeRatio;

			if (remainingTime <= 0.0f)
			{
				onTimeOver.Invoke();
			}
		}
	}
	#endregion UNITY EVENTS

	public void Initialize(float maxTime, float beginTime, bool ignoreTimeScale = false)
	{
		_timeGauge.LeanCancel();
        //_increaseGauge.LeanCancel();
        //      //_extraTimeObj.SetActive(false);

        this.maxTime = maxTime;
        this.remainingTime = beginTime;
        this.ignoreTimeScale = ignoreTimeScale;

        //_timeGauge.color = _defaultGaugeColor;
        //      _timeGauge.fillAmount = 1.0f; //(this.remainingTime / this.maxTime);
        //_increaseGauge.fillAmount = 0.0f;

        _timeGauge.fillAmount = 0;
        _increaseGauge.fillAmount = 0;
        _decreaseGauge.fillAmount = 0;
        remainingTime = 0;
        _extraTimeObj.GetComponentInChildren<SpriteFont>().Text = "0";
        _correctStack = 0;
        _timeSpan = 0;

        _timeText.Text = ((int)remainingTime).ToString();
        _isContinue = false;
    }

	public void Resume()
	{
        isPaused = false;
    }

	public void Pause()
	{
		isPaused = true;
    }

    public float IncreaseTime(float increaseTime)
    {
        isPaused = true;

        float timeVal = increaseTime + remainingTime;

        _timeText.Text = ((int)timeVal).ToString();

        _timeGauge.LeanCancel();
        _increaseGauge.LeanCancel();

        _increaseGauge.fillAmount = 0;

        float increaseRate = increaseTime / maxTime;
        _increaseText.Text = increaseTime.ToString();
        float txtPos = _increaseGauge.rectTransform.rect.height * increaseRate;

        _increaseTxtObj.transform.localPosition = new Vector3(_increaseTxtObj.transform.localPosition.x,
            txtPos - _increaseGauge.rectTransform.rect.height, 0);
        _increaseTxtObj.SetActive(true);


        _increaseGauge.fillAmount = increaseRate;

        float increaseDuration =
            (_increaseGauge.fillAmount - _timeGauge.fillAmount) * _maxIncreaseTime;

        LeanTween.value(_timeGauge.gameObject, (value) =>
        {
            _timeGauge.fillAmount = value;
        }, _timeGauge.fillAmount, _increaseGauge.fillAmount, 1f)
        .setOnStart(() => SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_time_refill))
        .setOnComplete(() =>
        {
            _increaseTxtObj.SetActive(false);
            _increaseGauge.fillAmount = 0;
        });

        this.remainingTime = increaseTime;

        //_timeGauge.fillAmount = increaseTime / maxTime;

        //if (_timeGauge.fillAmount <= warningGaugeRatio)
        //{
        //    _timeGauge.CrossFadeColor(_defaultGaugeColor, WARNING_TRANSITION_TIME, true, false);

        //    onTimeWarningReleased.Invoke();
        //}

        //_processedGaugeCount = 1;

        //float increaseDuration =
        //    (_increaseGauge.fillAmount - _timeGauge.fillAmount) * _maxIncreaseTime;

        //_timeGauge.LeanValue(_timeGauge.fillAmount, _increaseGauge.fillAmount, increaseDuration).
        //    setIgnoreTimeScale(true).
        //    setDelay(_increasingDelay).
        //    setOnStart(() => SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_time_refill)).
        //    setOnUpdate(v => _timeGauge.fillAmount = v).
        //    setOnComplete(() =>
        //    {
        //        _increaseGauge.fillAmount = 0.0f;
        //        --_processedGaugeCount;
        //    });

        //this.remainingTime = increaseTime;

        //if (_decreaseGauge.fillAmount <= _increaseGauge.fillAmount)
        //{
        //    _decreaseGauge.fillAmount = 0.0f;

        //    return increaseDuration;
        //}

        //float decreaseDuration =
        //    (_decreaseGauge.fillAmount - _increaseGauge.fillAmount) * _maxDecreaseTime;

        //++_processedGaugeCount;

        //_decreaseGauge.LeanValue(_decreaseGauge.fillAmount, _increaseGauge.fillAmount, decreaseDuration).
        //    setIgnoreTimeScale(true).
        //    setDelay(_decreasingDelay).
        //    setOnUpdate(v => _decreaseGauge.fillAmount = v).
        //    setOnComplete(() =>
        //    {
        //        _decreaseGauge.fillAmount = 0.0f;
        //        --_processedGaugeCount;
        //    });

        return Mathf.Max(increaseDuration, 1.0f);
    }

    public void IncreaseExtraTime(float extraTime)
    {
        _isExtraTime = true;

        _extraTimeObj.GetComponentInChildren<SpriteFont>().Text = extraTime.ToString();
        _extraTimeObj.SetActive(_isExtraTime);

        this.extraTime = extraTime;

        
    }

    public void DecreaseTime(float decreaseTime)
    {
        _timeGauge.LeanCancel();
        _increaseGauge.LeanCancel();

        if(_isExtraTime)
        {
            extraTime = Mathf.Max(extraTime - decreaseTime, 0.0f);
            if (isPaused)
            {
                _extraTimeObj.GetComponentInChildren<SpriteFont>().Text = ((int)extraTime).ToString();
            }

            if (extraTime <= 0.0f)
            {
                _extraTimeObj.SetActive(false);
                _isExtraTime = false;
            }

            return;
        }

        _decreaseGauge.fillAmount = remainingTime / maxTime;

        remainingTime = Mathf.Max(remainingTime - decreaseTime, 0.0f);

        float remainRate = remainingTime / maxTime;

        _timeGauge.fillAmount = remainingTime / maxTime;

        LeanTween.value(_decreaseGauge.gameObject, (value) =>
        {
            _decreaseGauge.fillAmount = value;
        }, _decreaseGauge.fillAmount, _timeGauge.fillAmount, 0.6f)
        .setOnStart(() => SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_time_penalty))
        .setOnComplete(() =>
        {
            _decreaseTxtObj.SetActive(false);
            _decreaseGauge.fillAmount = 0;
        });

        _timeText.Text = ((int)remainingTime).ToString();

        //if (_isExtraTime)
        //{
        //    this.extraTime -= decreaseTime;
        //    _extraTimeObj.GetComponent<Image>().fillAmount = extraTime / maxTime;
        //    return;
        //}

        //if (_increaseGauge.fillAmount > 0.0f)
        //{
        //    if (_increaseGauge.fillAmount < _decreaseGauge.fillAmount)
        //    {
        //        _increaseGauge.fillAmount = remainingTime / maxTime;

        //        _processedGaugeCount = 2;

        //        float increaseDuration =
        //            (_increaseGauge.fillAmount - _timeGauge.fillAmount) * _maxIncreaseTime;

        //        _timeGauge.LeanValue(_timeGauge.fillAmount, _increaseGauge.fillAmount, increaseDuration).
        //            setIgnoreTimeScale(true).
        //            setDelay(_increasingDelay).
        //            setOnStart(() => SoundManager.Instance.PlayUISoundInstance(FX_DECREASE)).
        //            setOnUpdate(v => _timeGauge.fillAmount = v).
        //            setOnComplete(() =>
        //            {
        //                _increaseGauge.fillAmount = 0.0f;
        //                --_processedGaugeCount;
        //            });

        //        float decreaseDuration =
        //            (_decreaseGauge.fillAmount - _increaseGauge.fillAmount) * _maxDecreaseTime;

        //        _decreaseGauge.LeanValue(_decreaseGauge.fillAmount, _increaseGauge.fillAmount, decreaseDuration).
        //            setIgnoreTimeScale(true).
        //            setDelay(_decreasingDelay).
        //            setOnUpdate(v => _decreaseGauge.fillAmount = v).
        //            setOnComplete(() =>
        //            {
        //                _decreaseGauge.fillAmount = 0.0f;
        //                --_processedGaugeCount;
        //            });

        //        return;
        //    }

        //    _timeGauge.fillAmount = remainingTime / maxTime;
        //    _decreaseGauge.fillAmount = _increaseGauge.fillAmount;
        //    _increaseGauge.fillAmount = 0.0f;
        //}
        //else
        //{
        //    _timeGauge.fillAmount = remainingTime / maxTime;
        //}

        //	_processedGaugeCount = 1;

        //	float duration = (_decreaseGauge.fillAmount - _timeGauge.fillAmount) * _maxDecreaseTime;

        //	_decreaseGauge.LeanValue(_decreaseGauge.fillAmount, _timeGauge.fillAmount, duration).
        //		setIgnoreTimeScale(true).
        //		setDelay(_decreasingDelay).
        //		setOnStart(() => SoundManager.Instance.PlayUISoundInstance(FX_DECREASE)).
        //		setOnUpdate(v => _decreaseGauge.fillAmount = v).
        //		setOnComplete(() => {
        //			_decreaseGauge.fillAmount = 0.0f;
        //			--_processedGaugeCount;
        //		});

        if (remainingTime <= 0.0f)
        {
            onTimeOver.Invoke();
        }
    }

    //public Vector2 GetGaugeHeight()
    //{
    //    _decreaseGauge.

    //    return;
    //}

    public bool nowChanging
	{
		get
		{
			return (_processedGaugeCount > 0);
		}
	}

    public float maxIncreaseTime
    {
        get
        {
            return _maxIncreaseTime;
        }

        set
        {
            _maxIncreaseTime = Mathf.Max(value, 0.0f);
        }
    }

    public float maxDecreaseTime
    {
        get
        {
            return _maxDecreaseTime;
        }

        set
        {
            _maxDecreaseTime = Mathf.Max(value, 0.0f);
        }
    }

    float warningGaugeRatio
	{
		get
		{
			return _warningPercentage * 0.01f;
		}
	}

    public bool IsContinue
    {
        get
        {
            return _isContinue;
        }
        set
        {
            _isContinue = value;
        }
    }

    public void StopTimer()
    {
        _isExtraTime = false;

        remainingTime = 0f;

        _extraTimeObj.SetActive(false);
    }

    public void InputCorrectStack()
    {
        _correctStack++;
    }

    private void RandomVoice(float ratio)
    {
        if(_timeSpan < 1f)
        {
            return;
        }

        if ((int)remainingTime == maxTime * 0.3)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE70);
        }
        else if ((int)remainingTime == maxTime * 0.4)
        {
            if (_correctStack == 2)
            {
                SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE_CORRECT_TWO);
            }
            else if (_correctStack == 3)
            {
                SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE_CORRECT_THREE);
            }
        }
        else if ((int)remainingTime == maxTime * 0.5 && _correctStack == 0)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE_CORRECT_ZERO);
        }
        else if ((int)remainingTime == maxTime * 0.6)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE40);
        }
        else if ((int)remainingTime == maxTime * 0.7 && _correctStack == 1)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE_CORRECT_ONE);
        }
        else if ((int)remainingTime == maxTime * 0.85)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.TIMESINCE15);
        }

        _timeSpan = 0f;
    }
}