using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboTimer : MonoBehaviour {

    [SerializeField] float _limitTime = 5.0f;

    [Header("CoolTime UI")]
    [SerializeField] GameObject _leftCoolTimeObj;
    [SerializeField] GameObject _rightCoolTimeObj;
    [SerializeField] Image _leftCoolTimeImg;
    [SerializeField] Image _rightCoolTimeImg;

    [Header("Combo UI")]
    [SerializeField] GameObject _leftComboObj;
    [SerializeField] GameObject _rightComboObj;
    [SerializeField] Image _leftComboImg;
    [SerializeField] Image _rightComboImg;
    [SerializeField] Image[] _comboInfoImgs;
    [SerializeField] SpriteFont _leftComboTxt;
    [SerializeField] SpriteFont _rightComboTxt;

    [Header("Score UI")]
    [SerializeField] SpriteFont _leftScoreTxt;
    [SerializeField] SpriteFont _rightScoreTxt;
    [SerializeField] GameObject _leftScoreInfoObj;
    [SerializeField] GameObject _rightScoreInfoObj;

    bool _isPause = false;

    //Animatior Play Time
    [SerializeField] float _comboPlayTime = 0.7f;

    //pos
    Vector2 _leftPos;
    Vector2 _rightPos;

    float _timer = 0.0f;
    bool _isTimerStart = false;
    int _combo = 0;
    int _comboScore;
    public int _totalScore { get; private set; }
    List<int> _comboScoreList = new List<int>();

    #region UNITY EVENTS	
    // Update is called once per frame
    void Update() {

        if(_isPause)
        {
            return;
        }

        if (_isTimerStart)
        {
            _timer += Time.deltaTime;
            if (_timer > _limitTime)
            {
                ComboStop();
            }
        }
    }
    #endregion

    public void Initialize()
    {
        _comboScore = 0;
        _totalScore = 0;

        _isPause = false;

        _comboScoreList.Clear();
        _leftCoolTimeObj.SetActive(false);
        _rightCoolTimeObj.SetActive(false);
        _leftScoreInfoObj.gameObject.SetActive(false);
        _rightScoreInfoObj.gameObject.SetActive(false);
        _leftComboImg.gameObject.SetActive(false);
        _rightComboImg.gameObject.SetActive(false);
        _leftComboObj.SetActive(false);
        _rightComboObj.SetActive(false);
    }

    //combo success
    public void ComboReset()
    {
        _timer = 0;
        _combo++;
    }

    //combo start
    public void ComboStart(Vector2 leftPos, Vector2 rightPos)
    {
        _rightPos = rightPos;
        _leftPos = leftPos;

        if (_isTimerStart == false)
        {
            _combo = 1;

            _isTimerStart = true;

            StartCoroutine(ProcessCoolTime(_limitTime));
        }
        else
        {
            _leftCoolTimeImg.fillAmount = 1.0f;
            _rightCoolTimeImg.fillAmount = 1.0f;
            ComboReset();
        }
        DrawCoolTimer(leftPos, rightPos);
        CalculateComboScore();
        //StartCoroutine(DrawScoreInfo());
    }

    //combo end
    public void ComboStop()
    {
        _isTimerStart = false;
        _leftCoolTimeObj.SetActive(false);
        _rightCoolTimeObj.SetActive(false);

        _comboScoreList.Add(_comboScore);
        _timer = 0;
        _combo = 0;
        _comboScore = 0;
    }

    public bool IsComboEnd()
    {
        return _isTimerStart;
    }


    public int ComboInfoReturn()
    {
        return _combo;
    }

    public void PauseSet()
    {

        _isPause = (_isPause == true)? false : true;
    }

    public void DrawCoolTimer(Vector2 leftPos, Vector2 rightPos)
    {
        _leftCoolTimeObj.transform.position = leftPos;
        _rightCoolTimeObj.transform.position = rightPos;

        _leftCoolTimeObj.SetActive(true);
        _rightCoolTimeObj.SetActive(true);

        if(_combo != 0)
        {
            StartCoroutine(DrawComboInfo(leftPos, rightPos));
        }
    }
   

    void CalculateComboScore()
    {
        List<int> baseScoreInfo = GameManager.Instance.InGameData.GetComboScoreInfos();
        for (int i = 0; i <= baseScoreInfo.Count; i++)
        {
            if (_combo == i)
            {
                _comboScore = _combo
                    * baseScoreInfo[i - 1];
            }
        }
        CalculateTotalScore();
    }

    void CalculateTotalScore()
    {
        int currentScoreSum = 0;

        foreach (int i in _comboScoreList)
        {
            currentScoreSum += i;
        }

        _totalScore = currentScoreSum + _comboScore;
    }

    void DrawScoreInfo(Vector2 leftPos, Vector2 rightPos)
    {
        _leftScoreInfoObj.transform.localPosition
            = new Vector2(_leftCoolTimeObj.transform.localPosition.x + 130, _leftCoolTimeObj.transform.localPosition.y);
        _rightScoreInfoObj.transform.localPosition
            = new Vector2(_rightCoolTimeObj.transform.localPosition.x + 130, _rightCoolTimeObj.transform.localPosition.y);

        SpriteFontSetting();

        _leftComboImg.color = (_combo == 1) ? GameConstants.TRANSPARENT : GameConstants.OPAQUE;
        _rightComboImg.color = (_combo == 1) ? GameConstants.TRANSPARENT : GameConstants.OPAQUE;

        

        for (int i = 0; i < _comboInfoImgs.Length; i++)
        {
            if((_combo -1) == i)
            {
                _leftComboImg.sprite = _comboInfoImgs[i].sprite;
                _rightComboImg.sprite = _comboInfoImgs[i].sprite;
            }
            _leftScoreInfoObj.SetActive(true);
            _rightScoreInfoObj.SetActive(true);
        }    
    }

    void SpriteFontSetting()
    {
        int baseScore = DataManager.Instance.GetBaseScoreInfo(GameManager.Instance.InGameData.ThemeDifficulty).basicScore;

        _leftScoreTxt.WordSpace = 25;
        _rightScoreTxt.WordSpace = 25;
        _leftScoreTxt.FontSize = 50;
        _rightScoreTxt.FontSize = 50;
        _leftScoreTxt.Text = baseScore.ToString();
        _rightScoreTxt.Text = baseScore.ToString();
    }

    #region COROUTINE
    IEnumerator ProcessCoolTime(float coolTime)
    {
        _leftCoolTimeImg.fillAmount = 1.0f;
        _rightCoolTimeImg.fillAmount = 1.0f;

        while (_leftCoolTimeImg.fillAmount > 0)
        {
            if(!_isPause)
            {
                _leftCoolTimeImg.fillAmount -= Time.smoothDeltaTime / coolTime;
                _rightCoolTimeImg.fillAmount -= Time.smoothDeltaTime / coolTime;

            }
            yield return null;
        }

        _leftCoolTimeObj.gameObject.SetActive(false);
        _rightCoolTimeObj.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator DrawComboInfo(Vector2 leftPos, Vector2 rightPos)
    {
        DrawScoreInfo(leftPos, rightPos);

        if (_combo <= 1)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CORRECT);
            yield return new WaitForSeconds(_comboPlayTime);
            _leftScoreInfoObj.SetActive(false);
            _rightScoreInfoObj.SetActive(false);
            yield break;
        }

        switch (_combo)
        {
            case 2:
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_combo1);
                break;
            case 3:
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_combo2);
                break;
            case 4:
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_combo3);
                break;
            case 5:
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_combo4);
                break;
        }

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.COMBO);

        _leftComboTxt.Text = _combo.ToString();
        _rightComboTxt.Text = _combo.ToString();

        _leftComboTxt.FontSize = 80;
        _rightComboTxt.FontSize = 80;

        _leftComboObj.transform.localPosition = new Vector2(_leftCoolTimeObj.transform.localPosition.x + 100, _leftCoolTimeObj.transform.localPosition.y + 70);
        _rightComboObj.transform.localPosition = new Vector2(_rightCoolTimeObj.transform.localPosition.x + 100, _rightCoolTimeObj.transform.localPosition.y + 70);

        _leftComboObj.SetActive(true);
        _rightComboObj.SetActive(true);

        _leftComboImg.gameObject.SetActive(true);
        _rightComboImg.gameObject.SetActive(true);

        yield return new WaitForSeconds(_comboPlayTime);

        _leftComboObj.SetActive(false);
        _rightComboObj.SetActive(false);

        _leftScoreInfoObj.SetActive(false);
        _rightScoreInfoObj.SetActive(false);
    }
    #endregion
}