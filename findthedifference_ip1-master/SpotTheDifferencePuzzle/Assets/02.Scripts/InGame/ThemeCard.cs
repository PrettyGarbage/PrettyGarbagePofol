using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ThemeCard : MonoBehaviour
{
	static StringBuilder _stringBuilder = new StringBuilder();

	[SerializeField] Image _themeMainImage;
	[SerializeField] Image _unLockMainImage;
    [SerializeField] LocalizationUIText _themeNameText;

	[SerializeField] float _completeMarkerAppearInterval = 0.5f;
	[SerializeField] float _completeMarkAppearScale = 1.5f;

    [Header("Background")]
    [SerializeField] Image _normalBackground;
    [SerializeField] Image _masterBackground;

    [Header("Theme Contents")]
    [SerializeField] TMP_Text _stageDescText;
	[SerializeField] TMP_Text _rateText;

	[SerializeField] Image _completeMark;
	[SerializeField] float _appearCompleteMarkTime = 0.3f;
	
	[SerializeField] Image _background;

    [Header("Lock Info")]
    [SerializeField] GameObject _unLockImageObj;
    [SerializeField] Material _grayScaleMaterial;
    [SerializeField] Animator _unlockAnimator;
    [SerializeField] GameObject _unLockImageObject;

    ThemeInfo _themeInfo;

    int _themeSeq;
	bool _isBasicCompleted = false;
	bool _isMasterCompleted = false;
    int _preThemeStarVal;
    ThemeDifficulty _themeDifficulty;
    bool _isShakeNow = false;
    bool _isShowUnlockMasterText = false;

    //for Unlock
    bool isNormalLock;
    bool isMasterLock;

    private UserThemeInfo UserThemeInfo { get { return GameManager.Instance.userThemeInfoList[_themeSeq]; } }

    #region
    private void Update()
    {
        if (_isShakeNow)
        {
            LeanTween.moveLocalX(gameObject, gameObject.transform.localPosition.x + 3, 0.1f).setEaseShake();
        }
    }
    #endregion

    public void Initialize(int seq, ThemeInfo themeInfo, ThemeDifficulty themeDifficulty)
	{
        _themeInfo = themeInfo;
        _themeSeq = seq;

		_themeMainImage.sprite = _themeInfo.ThemeMainImage;
		_themeNameText.SetText(GameManager.Instance.GetThemeNameKey(_themeInfo.id));
        _unLockMainImage.sprite = _themeInfo.ThemeMainImage;

        SetStageDesc(seq);

        SetActiveUI();
    }

	public void Refresh(int themeSeq, ThemeDifficulty themeDifficulty)
	{
        _themeSeq = themeSeq;
        _themeDifficulty = themeDifficulty;

        SetActiveUI();
    }

    int GetObtainStarCount()
	{
        if (UserThemeInfo==null)
        {
            return 0;
        }
        else
        {
            return (_themeDifficulty == ThemeDifficulty.THEME_BASIC) ?
            UserThemeInfo.themeDetailBasic.obtainStarCoin
            : UserThemeInfo.themeDetailMaster.obtainStarCoin;
        }
    }

    public string GetThemeCardID()
    {
        return _themeInfo.id;
    }

	public bool IsBasicCompleted()
	{
        if (UserThemeInfo == null)
        {
            return false;
        }

        return UserThemeInfo.themeDetailBasic.stageClearCount 
            >= DataManager.Instance.GetStageInfoList(_themeInfo.id).Count;
    }

    public void UnlockAnimationEvent()
    {
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_theme_card_flip);
        _unLockImageObj.SetActive(false);
    }

    public bool IsCompleteTheme()
    {
        if (UserThemeInfo == null || _completeMark.gameObject.activeSelf)
        {
            return false;
        }

        int attainableStar = GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_themeInfo.id).Count;
        return (_themeDifficulty == ThemeDifficulty.THEME_BASIC) ?
            UserThemeInfo.themeDetailBasic.obtainStarCoin >= attainableStar
            : UserThemeInfo.themeDetailMaster.obtainStarCoin >= attainableStar;
    }

    void IsLockTheme()
    {
        isMasterLock = UserThemeInfo.themeDetailBasic.stageClearCount < DataManager.Instance.GetStageInfoList(_themeInfo.id).Count;

        isNormalLock = ((UserThemeInfo.unlockYn== 0)) ? true : false;

        if (_themeDifficulty == ThemeDifficulty.THEME_BASIC )
        {
            _unLockImageObj.gameObject.SetActive(isNormalLock);
        }
        else
        {
            _unLockImageObj.gameObject.SetActive(isMasterLock);
        }

    }


    public bool IsNextThemeLockImgActive()
    {
        return _unLockImageObj.activeSelf;
    }


    private void SetActiveUI()
    {
        if (UserThemeInfo!=null)
        {
            IsLockTheme();
        }

        bool isComplete = GetObtainStarCount() >= (GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_themeInfo.id).Count);
        _completeMark.gameObject.SetActive(isComplete);


        gameObject.GetComponent<Image>().sprite = SetCardBackground().sprite;

        _unLockImageObject.SetActive(false);

        _rateText.text = GetObtainStarCount() + " / "
    + (GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_themeInfo.id).Count);

        //SetUnLockStatus();
    }

    public void SetOnlyStarPoint()
    {
        _rateText.text = GetObtainStarCount() + " / "
            + (GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_themeInfo.id).Count);
    }

    private Image SetCardBackground()
    {
        if (_themeDifficulty == ThemeDifficulty.THEME_MASTER)
        {
            if (!_unLockImageObj.activeSelf)
                return _masterBackground;
        }

        return _normalBackground;
    }

    

    //언락 상태일때 색상 조절 및 UI다르게 표현
    private void SetUnLockStatus()
    {
        Color lockBG = (_unLockImageObj.activeSelf)? new Color(103 / 255f, 103 / 255f, 103 / 255f) : new Color(60 / 255f, 36 / 255f, 27 / 255f);
        Color lockTitle = (_unLockImageObj.activeSelf) ? new Color(70 / 255f, 70 / 255f, 70 / 255f) : new Color(60 / 255f, 36 / 255f, 27 / 255f);
        Color lockDesc = (_unLockImageObj.activeSelf) ? new Color(135 / 255f, 135 / 255f, 135 / 255f) : new Color(1,1,1);
        _themeMainImage.material = (_unLockImageObj.activeSelf) ? _grayScaleMaterial : null;

        _background.color = lockBG;
        _themeNameText.gameObject.GetComponent<TextMeshProUGUI>().color = lockTitle;
        _stageDescText.color = lockDesc;

        _rateText.GetComponentInChildren<Image>().enabled = (!_unLockImageObj.activeSelf);
    }


    private void SetStageDesc(int seq)
    {
        int startNum = 0;
        int endNum = 0;

        for(int i = 0; i <= seq; i++)
        {
            startNum = endNum+1;
            endNum += DataManager.Instance.ThemeInfoList[i].stageCount;
        }

        _stringBuilder.Clear();
        _stringBuilder.Append(GameConstants.STAGE_DESC)
            .Append(startNum)
            .Append(" ~ ")
            .Append(endNum);

        _stageDescText.text = _stringBuilder.ToString();
    }


    #region EVENTS
    public void OnSelectTheme()
    {
        if (_unLockImageObj.activeSelf)
        {
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_lock_contents);

            ShakeMe();
            return;
        }

        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_theme_card_flip);

        GameManager.Instance.InGameData.SetSelectUserThemeInfo(UserThemeInfo);
        GameManager.Instance.InGameData.SetReadyData(0); //테마 이동시에 stageidx 초기화

        SelectStageState.Open(_themeInfo, _themeDifficulty);
    } 

    public void ShakeMe()
    {
        StartCoroutine(ShakeNow());
    }

    public void SetThemeStarPoint()
    {
        _rateText.text = GetObtainStarCount() + " / "
                + (GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_themeInfo.id).Count);
    }
    #endregion EVENTS

    #region COROUTINES

    IEnumerator ProcessAppearCompleteMark(Image mark, AudioDataKey audioDataKey)
    {
        yield return new WaitForSeconds(_completeMarkerAppearInterval);

        SoundManager.Instance.PlayUISoundInstance(audioDataKey);

        mark.gameObject.SetActive(true);

        mark.color *= GameConstants.TRANSPARENT;
        mark.rectTransform.LeanAlpha(1.0f, _appearCompleteMarkTime).
            setIgnoreTimeScale(true);

        Vector3 originScale = mark.rectTransform.localScale;
        mark.rectTransform.localScale *= _completeMarkAppearScale;
        mark.rectTransform.LeanScale(originScale, _appearCompleteMarkTime).
            setEaseOutBack().
            setIgnoreTimeScale(true);

        yield return new WaitForSeconds(_appearCompleteMarkTime);
    }

    IEnumerator ShakeNow()
    {
        Vector3 originPos = gameObject.transform.localPosition;
        _isShakeNow = true;
        gameObject.GetComponent<Button>().interactable = false;
        StateManager.Instance.ActiveBlockScreen(true);
        yield return new WaitForSeconds(0.1f);
        _isShakeNow = false;
        yield return new WaitForSeconds(0.1f);
        StateManager.Instance.ActiveBlockScreen(false);

        gameObject.transform.localPosition = originPos;
        gameObject.GetComponent<Button>().interactable = true;
    }

    public IEnumerator UnLockImageExpress()
    {
        StateManager.Instance.ActiveBlockScreen(true);
        SetThemeStarPoint();
        _unLockImageObject.SetActive(true);
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_unlock_contents);
        _unlockAnimator.Play("open");

        yield return new WaitForSeconds(0.2f);
        StateManager.Instance.ActiveBlockScreen(false);
    }

    public IEnumerator CompleteMarkExpress()
    {
        yield return new WaitForSeconds(0.2f);
        _completeMark.gameObject.SetActive(true);
        _completeMark.transform.localScale = Vector3.one * 2f;
        _completeMark.color = GameConstants.TRANSPARENT;
        LeanTween.value(0f, 1f, 1f).setOnUpdate((float v) =>
        {
            _completeMark.color = GameConstants.OPAQUE * v;
        });
        LeanTween.scale(_completeMark.gameObject, Vector3.one, 1f)
            .setEaseInElastic()
            .setOnComplete(()=>
            {
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_clear_Mstamp);
            });
    }
    #endregion COROUTINES
}