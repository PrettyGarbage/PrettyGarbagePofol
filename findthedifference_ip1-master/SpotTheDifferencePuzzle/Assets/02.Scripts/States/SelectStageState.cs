using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class SelectStageState : State
{  
    StringBuilder _stringBuilder = new StringBuilder();

    [Header("BG")]
    [SerializeField]
    private Image _bgImage;

	[Header("Stage Card")]
	[SerializeField] StageCard _stageCardPrefab;
	[SerializeField] ScrollRect _contentRect;
    [SerializeField] GameObject _themeTitleLevel;
    [SerializeField] LocalizationUIText _themeLevelTxt;
    [SerializeField] LocalizationUIText _themeTitleTxt;
    [SerializeField] RectTransform _contentsRoot;

    [Header("SelectStageState UI")]
    [SerializeField] Button _themeClearCheatBtn;
    [SerializeField] TMP_Text _starPointTxt;
    [SerializeField] Image _normalTitleImg;
    [SerializeField] Image _masterTitleImg;
    [SerializeField] float _slideMoveSpeed = 0.05f;

    List<StageCard> _cards = new List<StageCard>();
    private String _lastThemeId = string.Empty;
    
    ThemeInfo _themeInfo;

    ThemeDifficulty _themeDifficulty = ThemeDifficulty.THEME_BASIC;

    private InGameData _inGameData;

    public static void Open(ThemeInfo themeInfo, ThemeDifficulty themeDifficulty)
    {
        GameManager.Instance.InGameData.SetStageListData(themeInfo, themeDifficulty);
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectStageState), false);
    }

    public override IEnumerator OnInitialize()
	{
        yield return null;
    }

	public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

        yield return GameManager.Instance.GetUserStageInfo();
        
        _contentRect.normalizedPosition = Vector2.zero;

        _inGameData = GameManager.Instance.InGameData;
        _themeInfo = _inGameData.ThemeInfo;
        _themeDifficulty = _inGameData.ThemeDifficulty;;
        
        _bgImage.sprite = _themeInfo.ThemeMainImage;

        Refresh();

        SetTitleInfo(_themeDifficulty);

        _themeClearCheatBtn.gameObject.SetActive(BuildManager.Instance.IsDevBuild() &&
            Application.platform != RuntimePlatform.Android &&
            Application.platform != RuntimePlatform.IPhonePlayer);

        CurrentStageBack(true);
    }

    public override IEnumerator OnOpening()
    {
        yield return base.OnOpening();
    }

    public void ProcessBuildStageCards(string themeId)
    {
        //if (_lastThemeId.Equals(themeId)) return;

        List<StageInfo> stageInfos = DataManager.Instance.GetStageInfoList(themeId);

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < stageInfos.Count; i++)
        {
            StageCard card;
            if (_cards.Count < i + 1)
            {
                card = Instantiate(_stageCardPrefab);
                _cards.Add(card);
                card.transform.SetParent(_contentRect.content);
                card.transform.localScale = Vector3.one;
            }
            else
            {
                card = _cards[i];
            }

            card.gameObject.SetActive(true);
            card.Initialize(i, stageInfos[i]);
            card.Refresh(i);


        }

    }

    public override IEnumerator OnResume()
    {
        yield return null;
    }

    #region FUNCTION

    //타이틀 정보 세팅
    void SetTitleInfo(ThemeDifficulty themeDifficulty)
    {
        _normalTitleImg.gameObject.SetActive(themeDifficulty == ThemeDifficulty.THEME_BASIC);
        _masterTitleImg.gameObject.SetActive(themeDifficulty != ThemeDifficulty.THEME_BASIC);

        _themeTitleTxt.SetText(GameManager.Instance.GetThemeNameKey(_themeInfo.id));
    }

    //UI 정보 새로고침
    void Refresh()
    {
        _starPointTxt.text = _inGameData.GetStageTotalStarCoinAmount() + " / "
            + (GameConstants.REWARD_ATTAINABLE_STARPOINT_IN_STAGE * DataManager.Instance.GetStageInfoList(_inGameData.ThemeInfo.id).Count);

        ProcessBuildStageCards(_themeInfo.id);

        _lastThemeId = _themeInfo.id;
    }
    #endregion

    #region BUTTON EVENT

    public void  CurrentStageBack(bool isPreOpen = false)
    {
        int lastClearIdx = (_themeDifficulty == ThemeDifficulty.THEME_BASIC) ?
            GameManager.Instance.InGameData.UserThemeInfo.themeDetailBasic.stageClearCount
            : GameManager.Instance.InGameData.UserThemeInfo.themeDetailMaster.stageClearCount;

        lastClearIdx = Math.Max(lastClearIdx, GameManager.Instance.InGameData.StageIndex);

        if (_cards.Count <= lastClearIdx)
        {
            lastClearIdx = _cards.Count - 1;
        }

        float sliderPos = (lastClearIdx >= 5) ? 1.01f : -0.01f;

        if(isPreOpen)
        {
            sliderPos = (float)Math.Truncate(sliderPos);
        }

        LeanTween.value(_contentsRoot.gameObject, _contentRect.horizontalNormalizedPosition, sliderPos, _slideMoveSpeed)
            .setOnUpdate((float val) =>
            {
                _contentRect.horizontalNormalizedPosition = val;
            });
    }

    public void OnClose()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), false);
    }

    public override void OnBack()
    {
        OnClose();
    }


#endregion


#if UNITY_EDITOR
    public void OnCheatThemeClear()
    {
        StageCheat stageCheat = new StageCheat();

        StartCoroutine(ThemeClear(stageCheat));
    }

    IEnumerator ThemeClear(StageCheat stageCheat)
    {
        yield return stageCheat.StageResultSend();

        yield return GameManager.Instance.GetUserStageInfo();

        Refresh();
    }
#endif
}