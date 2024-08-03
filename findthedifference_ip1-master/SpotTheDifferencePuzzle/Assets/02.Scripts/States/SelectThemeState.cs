using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GoogleMobileAds.Api;



public class SelectThemeState : State
{
    [Header("Theme Cards")]
    [SerializeField] ThemeCard _themeButtonPrefab;
    [SerializeField] Transform _contentRoot;
    [SerializeField] ScrollRect _themeCardScrollRect;

    List<ThemeCard> _themeCards = new List<ThemeCard>();

    static StringBuilder _stringBuilder = new StringBuilder();

    [Header("SelectThemeState UI")]
    [SerializeField] Image _masterBg;
    [SerializeField] Button _difficultyBtn;
    [SerializeField] GameObject _normalDifficultObj;
    [SerializeField] GameObject _masterDifficultObj;
    [SerializeField] GameObject _levelSelectExpressObj;
    [SerializeField] GameObject _newAlramObj;

    [Header("Alram")]
    [SerializeField] GameObject _spinEventAlarmObject;
    [SerializeField] GameObject _collectionAlarmObject;

    private List<PopupInstantType> _playActionEventList = new List<PopupInstantType>();

    ThemeDifficulty _themeDifficulty;

    ThemeCard _prevSelectedCard;
    bool _isActionTextFlowEnd = false;

    public override IEnumerator OnInitialize()
    {
        yield return null;
    }
    
    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);

        _themeDifficulty = DataManager.Instance.UserInfo.SelectThemeDifficulty;
        SetDifficultyObject();

        StateManager.Instance.ShowUserCurrencyBar(true);
        SetAlarmObject();

        ThemeInfo prevThemeInfo = GameManager.Instance.InGameData.ThemeInfo;
        if (prevThemeInfo != null)
        {
            _prevSelectedCard = _themeCards.Find(t => t.GetThemeCardID().Equals(prevThemeInfo.id));
        }

        yield return GetUserThemeInfo();

        if (_themeCards.Count == 0)
        {
            yield return AssetBundleManager.Instance.LoadAssetBundlesCor(DataManager.Instance.AssetBundleData.themeAssetBundle, false, null,
                () =>{
                    DataManager.Instance.SetThemeDataInitailize();
                }
            );

            yield return ProcessBuildThemeCards();            
        }
        else
        {
            SetPlayActionEventList();
            if (_playActionEventList.Count == 0)
            {
                RefreshThemeCard();
            }
            else
            {
                RefreshStarPoint();
            }

        }
    }

    public override IEnumerator OnOpening()
    {
        SoundManager.Instance.PlayBgm(AudioDataKey.BGM_lobby);
        yield return null;
    }

    public override IEnumerator OnPostOpen()
    {
        if (DataManager.Instance.IsExistDailyReward())
        {
            PopupDailyRewardState.Open();
        }
        else
        {
            if (_playActionEventList.Count > 0)
            {
                StateManager.Instance.ActiveBlockScreen(true);
                yield return PlayActionText();
                StateManager.Instance.ActiveBlockScreen(false);

                RefreshThemeCard();
            }
        }

        EventPool.Send(EventNames.ON_UPDATE_USERINFO);
        DataManager.Instance.RemoveUnlockThemeId();
    }

    public override void OnPause()
    {
        base.OnPause();
    }

    public override IEnumerator OnResume()
    {
        _isActionTextFlowEnd = true;
        return base.OnResume();
    }

    #region EVENTS
    public void OnSpinEvent()
    {
        PopupSpinEventState.Open();
    }

    public void OnCollection()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(CollectionBookState), !DataManager.Instance.isLoadedCollectionBundle());
    }

    public void OnLevelSelect()
    {
        StartCoroutine(SwitchThemeDifficulty());
    }

    IEnumerator SwitchThemeDifficulty()
    {
        StateManager.Instance.ActiveBlockScreen(true);
        yield return StateManager.Instance.ShowFadeInOut(true);

        _levelSelectExpressObj.SetActive(false);
        _newAlramObj.SetActive(false);

        _themeDifficulty = _themeDifficulty == ThemeDifficulty.THEME_BASIC ? ThemeDifficulty.THEME_MASTER : ThemeDifficulty.THEME_BASIC;

        DataManager.Instance.UserInfo.SaveUserThemeDifficulty(_themeDifficulty);

        SetDifficultyObject();

        if (_themeDifficulty == ThemeDifficulty.THEME_BASIC)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.NORMALMODEBTN);
        }
        else
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.MASTERMODEBTN);
        }

        RefreshThemeCard();

        yield return StateManager.Instance.ShowFadeInOut(false);
        StateManager.Instance.ActiveBlockScreen(false);
    }

    private void SetDifficultyObject()
    {
        _masterBg.gameObject.SetActive(_themeDifficulty == ThemeDifficulty.THEME_MASTER);

        //토글로 바꾸자.. .나중에
        _normalDifficultObj.SetActive(_themeDifficulty == ThemeDifficulty.THEME_BASIC);
        _masterDifficultObj.SetActive(_themeDifficulty != ThemeDifficulty.THEME_BASIC);
    }

    public void OnShop()
    {
        ShopState.Open(ShopMainType.GOLD);
    }

    private void OnUpdateUserInfo(object[] args)
    {
        SetAlarmObject();
    }
    #endregion

    #region COROUTINES
    IEnumerator ProcessBuildThemeCards()
    {
        List<StageInfo> stageInfo;
        List<ThemeInfo> themes = DataManager.Instance.GetThemaInfoList();

        for (int i = 0, max = themes.Count; i < max; ++i)
        {
            ThemeInfo theme = themes[i];
            if (!string.IsNullOrEmpty(theme.name))
            {
                GameObject themeCardGO =
                    ObjectUtil.Instantiate(_themeButtonPrefab.gameObject) as GameObject;
                themeCardGO.transform.SetParent(_contentRoot);
                themeCardGO.transform.localScale = Vector3.one;

                stageInfo = DataManager.Instance.GetStageInfoList(themes[i].id);

                ThemeCard themeCard = themeCardGO.GetComponent<ThemeCard>();
                themeCard.Initialize(i, theme, _themeDifficulty);

                _themeCards.Add(themeCard);

                yield return new WaitForEndOfFrame();
            }
        }

        RefreshThemeCard();
    }

    private void SetPlayActionEventList()
    {
        _playActionEventList.Clear();

        if (GameManager.Instance.InGameData.IsUnLockTheme())
        {
            _playActionEventList.Add(PopupInstantType.THEME_UNLOCK);
        }

        if (GameManager.Instance.InGameData.IsUnLockTheme())
        {
            _playActionEventList.Add(PopupInstantType.THEME_OPEN_MASTER);
        }

        if (_prevSelectedCard != null && _prevSelectedCard.IsCompleteTheme())
        {
            _playActionEventList.Add(PopupInstantType.THEME_COMPLETE);
        }

        List<CollectionListInfo> unlockCollectionInfos = new List<CollectionListInfo>();
        unlockCollectionInfos = DataManager.Instance.GetUnlockCollectionListInfos();

        for (int i = 0; i < unlockCollectionInfos.Count; i++)
        {
            if (!unlockCollectionInfos[i].isOpen)
            {
                _playActionEventList.Add(PopupInstantType.COLLECTION_UNLOCK);
                CollectionListInfo cli = unlockCollectionInfos[i];
                cli.isOpen = true;
                unlockCollectionInfos[i] = cli;
            }
        }
    }


IEnumerator PlayActionText()
    {
        if (_playActionEventList.Count > 0)
        {
            MoveSlide();
            for (int i = 0; i < _playActionEventList.Count; i++)
            {
                Debug.Log("액션리스트 : " + _playActionEventList[i].ToString());
                string themeTitleName = LocalizationManager.Instance.
                    GetText(GameManager.Instance.GetThemeNameKey
                    (GameManager.Instance.InGameData.ThemeInfo.id));

                PopupInstantState.Open(_playActionEventList[i]);
                _isActionTextFlowEnd = false;

                while (_isActionTextFlowEnd == false || StateManager.Instance.nowTransitioning == true)
                {
                    yield return CommonConstants.WaitLoopSeconds;
                }
                yield return ProcessActionTextEvent(_playActionEventList[i]);
            }  
        }
    }

     private void MoveSlide()
    {
        if (GameManager.Instance.InGameData.ThemeInfo.id == null) return;

        ThemeCard card = _themeCards.Find(t => t.GetThemeCardID().Equals(GameManager.Instance.InGameData.ThemeInfo.id));

        float cardPos = card.transform.localPosition.x;
        float sliderLength = _contentRoot.gameObject.GetComponent<RectTransform>().rect.width;
        float sliderPos = (cardPos) / (sliderLength - (cardPos * 0.2f));

        sliderPos = (float)Math.Truncate((sliderPos * 10)) / 10;

        LeanTween.value(_contentRoot.gameObject, 0, sliderPos, 0.25f)
            .setOnUpdate((float val) => {
                _themeCardScrollRect.horizontalNormalizedPosition = val;
                });

    }

    IEnumerator ProcessActionTextEvent(PopupInstantType textEvent)
    {
        switch (textEvent)
        {
            case PopupInstantType.THEME_UNLOCK:
                ThemeCard unLockCard = _themeCards.Find(t => t.GetThemeCardID().Equals(DataManager.Instance.GetUnlockThemeId()));
                //ThemeCard unLockCard = _themeCards[2];
                yield return unLockCard.UnLockImageExpress();
                break;
            case PopupInstantType.THEME_OPEN_MASTER:
                yield return MasterModeOpenExpress();
                break;
            case PopupInstantType.THEME_COMPLETE:
                if (_prevSelectedCard != null)
                {
                    yield return _prevSelectedCard.CompleteMarkExpress();
                }
                break;
            case PopupInstantType.COLLECTION_UNLOCK:
                yield return UnlockCollectionExpress();
                break;
        }

        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator UnlockCollectionExpress()
    {
        _collectionAlarmObject.gameObject.SetActive(true);
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_notice_exc);

        _collectionAlarmObject.transform.localScale = Vector3.one * 2f;
        LeanTween.scale(_collectionAlarmObject.gameObject, Vector3.one, 1f)
            .setEaseInElastic();

        yield return new WaitForSeconds(1f);
    }

    IEnumerator MasterModeOpenExpress()
    {
        GameObject levelselctObj = _normalDifficultObj.GetComponentInParent<RectTransform>().gameObject;

        levelselctObj.transform.localScale = Vector3.one * 1.2f;
        LeanTween.scale(levelselctObj.gameObject, Vector3.one, 1f)
            .setEaseInElastic();

        _levelSelectExpressObj.SetActive(true);
        _newAlramObj.SetActive(true);
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_notice_exc);

        yield return new WaitForSeconds(0.5f);
    }
    #endregion COROUTINES

    #region FUNCTION
    private void RefreshThemeCard()
    {
        List<ThemeInfo> themes = DataManager.Instance.GetThemaInfoList();

        for (int i = 0; i < themes.Count; i++)
        {
            _themeCards[i].Refresh(i, _themeDifficulty);
        }
    }

    private void RefreshStarPoint()
    {
        List<ThemeInfo> themes = DataManager.Instance.GetThemaInfoList();

        for (int i = 0; i < themes.Count; i++)
        {
            _themeCards[i].SetOnlyStarPoint();
        }
    }

    private void SetAlarmObject()
    {
        _collectionAlarmObject.SetActive(DataManager.Instance.IsExistOpenEnableCollection());
        _spinEventAlarmObject.SetActive(DataManager.Instance.UserInfo.GetItemCount(ItemType.SPIN_TICKET)>0);
    }

    private void OnEnable()
    {
        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, OnUpdateUserInfo);
    }


    private void OnDisable()
    {
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, OnUpdateUserInfo);
    }

    public override void OnBack()
    {
        SettingsState.Open();
    }

    public IEnumerator GetUserThemeInfo()
    {
        bool isLoaded = false;

        ApiManager.Instance.GetUserThemeList((result) =>
        {
            GameManager.Instance.userThemeInfoList = result.result;

            isLoaded = true;
        });

        while (!isLoaded)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
    }
    #endregion


}
