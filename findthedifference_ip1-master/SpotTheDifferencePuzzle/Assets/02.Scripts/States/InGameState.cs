using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LunarConsolePlugin;
using System.Text;

using Random = UnityEngine.Random;
using GoogleMobileAds.Api;

public class InGameState : State
{    
    const float HUD_TIME = 2.0f;
    static readonly Vector3 HUD_BEGIN_OFFSET = Vector3.down * 200.0f;
    
    const float CURTAIN_MOVE_TIME = 0.5f;

    const float BONUS_TIME_DELAY = 1.0f;

    const int INVALID_PUZZLE_TOKEN_INDEX = -1;

    StringBuilder _stringBuilder = new StringBuilder();

    [Header("Game")]
    [SerializeField] InGameTimer _timer;
    [SerializeField] GameObject _wrongMarkObj;
    [SerializeField] GameObject _starObj;
    [SerializeField] GameObject _starTwoObj;
    [SerializeField] int _maxCountinueCount = 1;
    [SerializeField] float _timerStartGaugeIncreaseTime = 0.5f;
    [SerializeField] TestOption testOption = TestOption.OPTIONB;

    [Header("Item")]
    [SerializeField] GameObject _shieldObj;
    [SerializeField] GameObject _scoreBonusObj;
    [SerializeField] Animator _shieldAnim;
    [SerializeField] Image _scoreBonusIcon;
    [SerializeField] Image _shieldIcon;
    [SerializeField] Image _timeBonusIcon;


    [Header("Round")]
    [SerializeField] float _roundClearDelay = 1.0f;
    

    [Header("Boards")]
    [SerializeField] Board _leftBoard;
    [SerializeField] Board _rightBoard;
    [SerializeField] Curtain _leftCurtain;
    [SerializeField] Curtain _rightCurtain;
    
    [Header("Items - Hint")]
    [SerializeField] Button _hintItemButton;
    [SerializeField] float _itemActivationDelay = 0.2f;
    [SerializeField] TMP_Text _itemAmountText;

    [Header("Items - Frozen")]
    [SerializeField] Button _frozenItemButton;
    [SerializeField] Animator _frozenAnimator;
    [SerializeField] TMP_Text _frozenItemAmountText;
    [SerializeField] SpriteFont _frozenTimerTxt;
    private float _frozenTime = GameConstants.INGAME_ITEM_FREEZE_TIME;

    [Header("UI - Default")]
    [SerializeField] GameObject _itemUseButton;
    [SerializeField] GameObject[] _splashObjs;
    [SerializeField] float _splashTime = 1.5f;
    [SerializeField] Image _warningFrame;
    [SerializeField] float _warningFrameBlinkInterval = 0.5f;
    [SerializeField] ComboTimer _comboTimer;
    [SerializeField] float _shakeSpeed;
    [SerializeField] int _shakeVal;

    [Header("UI - Normal Mode")]
    [SerializeField] GameObject _normalModeRoot;
    [SerializeField] TMP_Text _foundDifferenceText;
    [SerializeField] GameObject _itemRewardAdButton;
    [SerializeField] GameObject _correctIcon;
    [SerializeField] Animator _correctIconAnimator;


    [Header("UI - Bottom")]
    [SerializeField] TMP_Text _currentScoreBoardText;
    [SerializeField] GameObject[] _starAnimGroup;
    [SerializeField] GameObject[] _starGroup;
    [SerializeField] Image _scoreGauge;

    [Header("Stage Achievement")]
    [SerializeField] TMP_Text _starPointText;

    //
    PuzzleModeData _modeData;
    PuzzleImageData _puzzleData;
    List<PuzzleImagePart> _puzzleImageParts = new List<PuzzleImagePart>();
    
    int _maxDifference;
    int _foundDifference;

    // Hint
    int _hintPuzzleTokenIndex = INVALID_PUZZLE_TOKEN_INDEX;
    AudioSource _hintedAudioSource;
    int _hintItemAmount = 0;

    // Frozen Item
    int _frozenItemAmount = 0;
    bool _isFrozen = false;
    float _frozenStartTime = 0;

    //for item
    bool _isTimerPlusItem = false;
    bool _isShieldItem = false;
    bool _isScoreBonus = false;


    // Timer
    AudioSource _hurryUpSource;
    AudioSource _timeTickSource;
    Coroutine _timeWarningCoroutine;

    // Pause
    bool _pauseGame = false;
    Coroutine _pauseCoroutine;

    // for Stage Mode
    //ThemeInfo _themeInfo;
    StageInfo _stageInfo;
    string _themeLevel;

    // for Challenge Mode
    bool _isCoutinued;
    
    // for Analytics
    float _roundPlayTime = 0.0f;
    int _roundItemUsedCount = 0;
    float _totalPlayTime = 0.0f;
    int _totalItemUsedCount = 0;

    //for result
    private InGameData _inGameData;
    private int _missStack;
    private bool _isNoMiss; //미스하지 않고 클리어했는지를 판별하기 위함.

    //bgm
    private AudioDataKey _currentAudioKey;

    //time
    float _nextTime;

    //board
    Quaternion _leftOriginBoardRotate;
    Quaternion _rightOriginBoardRotate;

    public override IEnumerator OnInitialize()
    {
        // Timer
        _timer.onTimeOver.AddListener(OnTimeOver);
        _timer.onTimeWarning.AddListener(OnTimeWarning);
        _timer.onTimeWarningReleased.AddListener(OnWarningReleased);

        // Boards
        yield return _leftBoard.Initialize(OnTouchCurrectPosition, OnTouchWrongPosition, OnStarTrail);
        yield return _rightBoard.Initialize(OnTouchCurrectPosition, OnTouchWrongPosition, OnStarTrail);  
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        //
        LunarConsole.RegisterAction("Auto Complete Round", OnAutoCompleteRound);
        LunarConsole.RegisterAction("Auto Failure Round", OnAutoFailureRound);

        //
        SoundManager.Instance.StopBgm(AudioDataKey.BGM_lobby);

        yield return base.OnPreOpen(args);

        _inGameData = GameManager.Instance.InGameData;
        _inGameData.GameContinueTime = 0;

        if (!SetPlayModeData(_inGameData.ThemeDifficulty))
        {
            yield break;
        }

        _isCoutinued = false;

        // Setup puzzle datas
        _puzzleData = DataManager.Instance.GetThemaPuzzleData(_inGameData.ThemeInfo.id, _inGameData.StageIndex);

        _timer.Initialize(_modeData.LimitTime, 0.0f);

        // Initialize values
        _pauseGame = false;
        _hintPuzzleTokenIndex = INVALID_PUZZLE_TOKEN_INDEX;

        // Initialize values for analytics
        _totalPlayTime = 0.0f;
        _totalItemUsedCount = 0;
        _missStack = 0;
        _isNoMiss = true;

        // Initialize stageinfo
        _stageInfo = _inGameData.StageInfo;

        _normalModeRoot.SetActive(true);
        
        _warningFrame.color *= GameConstants.TRANSPARENT;

        UpdateItemAmount();

        // Initialize play mode UI
        LoadThemeMode();

        _comboTimer.Initialize();

        //Stage Init시에 먼저 정보를 넣어주기 위해 OnEnter에서 Load로 옮김.
        yield return InitializeStage();

        SetPositionStarImage();

        CalculateStarGauge();

        //item use info init
        InitUseItemObj();

        testOption = TestOption.OPTIONB;

        _leftOriginBoardRotate = _leftBoard.transform.rotation;
        _rightOriginBoardRotate = _rightBoard.transform.rotation;

        _foundDifferenceText.text = _foundDifference +"/"+ _maxDifference;

        //save last theme difficulty
        DataManager.Instance.UserInfo.SaveUserThemeDifficulty(_inGameData.ThemeDifficulty);
        NetworkManager.Instance.RequestBanner(ADConstants.BANNER_INGAME, AdSize.Banner, AdPosition.BottomLeft);
    }

    public override IEnumerator OnOpening()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        Debug.Log("<color=#00FFFF>Start " + _modeData.PlayMode + " Mode</color>");

        yield return ProcessStartThemeMode();

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_title_readygo);

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.READY_ACTION_TEXT);

        yield return ShowSplash(SPLASH.READY);

        PopupEquipItemState.Open();

        _timer.IncreaseTime(_modeData.LimitTime);

        yield return ShowBoards();

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.GO_ACTION_TEXT);

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_title_go);
        yield return ShowSplash(SPLASH.GO);

        PopupEquipItemState.Close();

        yield return new WaitForSeconds(0.5f);

        SetUseItemObj();

        if (_inGameData.ItemIdList.Count > 0)
        {
            yield return SparkleItemExpress();
        }

        _timer.Resume();

        StateManager.Instance.ActiveBlockScreen(false);
    }

    public override IEnumerator OnClosing()
    {
        NetworkManager.Instance.RemoveBanner(ADConstants.BANNER_INGAME);
        _timer.StopTimer();
        if (_isFrozen)
        {
            yield return EndFrozen();
        }

        //나중에 조건식 바꾸자...
        if(_hintPuzzleTokenIndex != INVALID_PUZZLE_TOKEN_INDEX)
        {
            _hintedAudioSource.Stop();
            _leftBoard.StopHint(_hintPuzzleTokenIndex);
            _rightBoard.StopHint(_hintPuzzleTokenIndex);
        }

        yield return base.OnClosing();
    }

    public override void OnUpdate()
    {
        if (_autoActionCoroutine == null &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseGame();
        }

        _roundPlayTime += Time.deltaTime;

        if(_isFrozen)
        {
            if ((_frozenStartTime + _frozenTime) < _roundPlayTime)
            {
                StartCoroutine(EndFrozen());
                return;
            }

            if (_roundPlayTime > _frozenStartTime + _nextTime)
            {
                _nextTime += 1f;
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_timefreeze_hurryup);
            }

            float time = (_frozenStartTime + _frozenTime) - _roundPlayTime;

            _frozenTimerTxt.Text = ((int)time +1).ToString();
        }
    }

    public override IEnumerator OnClosed()
    {
        _warningFrame.LeanCancel();
        _warningFrame.color *= GameConstants.TRANSPARENT;

        SoundManager.Instance.PlayBgm(AudioDataKey.BGM_lobby);

        //
        LunarConsole.UnregisterAction(OnAutoCompleteRound);
        LunarConsole.UnregisterAction(OnAutoFailureRound);

        _pauseCoroutine = null;
        _timeWarningCoroutine = null;
        _autoActionCoroutine = null;

        yield return base.OnClosed();
    }

    public override void OnPause()
    {
        base.OnPause();
    }

    public override IEnumerator OnResume()
    {
        if (_pauseGame)
        {
            if (_pauseCoroutine != null)
            {
                StopCoroutine(_pauseCoroutine);
            }

            yield return ShowBoards();

            if (!_isFrozen)
            {
                _timer.Resume();
            }

            _pauseGame = false;
        }

    }

    public int FoundDifference
    {
        get
        {
            return _foundDifference;
        }

        set
        {
            _foundDifference = Mathf.Clamp(value, 0, _maxDifference);
        }
    }

    bool SetPlayModeData(ThemeDifficulty  playMode)
    {
        _modeData = DataManager.Instance.GetPuzzleModeData(playMode);
        if (_modeData != null)
        {
            return true;
        }

        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append("Not found ");
        stringBuilder.Append(playMode);
        stringBuilder.Append(" Mode Puzzle Mode Data");

        MessageBoxState.Open(
           LocalizationTextKey.COMMON_ALERT_TITLE,
            LocalizationTextKey.ERROR_DATA_ERROR,
            () => StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), true)
        );

        return false;
    }

    void LoadThemeMode()
    {
        _stageInfo = _inGameData.StageInfo;
        _themeLevel = (_modeData.PlayMode == ThemeDifficulty .THEME_BASIC) ? "Basic" : "Master";

        FoundDifference = 0;

    }

    void GameOverThemeMode()
    {
        if (_isCoutinued)
        {
            StateManager.Instance.OpenPopupState(typeof(ThemeGameOverState), new ThemeGameOverStateData(_maxDifference, _foundDifference));
        }
        else
        {
            StateManager.Instance.OpenPopupState(typeof(ThemeContinueState), new ThemeGameOverStateData(_maxDifference, _foundDifference));
        }
    }

    void UpdateItemAmount()
    {
        _itemAmountText.text = "0";
        _frozenItemAmountText.text = "0";

        List<UserItemInfo> userIteminfos = DataManager.Instance.UserInfo.ItemList;
        foreach (UserItemInfo info in userIteminfos)
        {
            if (info.itemId.Equals(ItemType.HINT.ToString()))
            {
                _hintItemAmount = info.count;
                _itemAmountText.text = _hintItemAmount.ToString();
            }
            if(info.itemId.Equals(ItemType.FREEZE_TIME.ToString()))
            {
                _frozenItemAmount = info.count;
                _frozenItemAmountText.text = info.count.ToString();
            }
        }

        bool showItemRewardAdButton = (_hintItemAmount <= 0);

        _itemRewardAdButton.SetActive(showItemRewardAdButton);

    }

    ThemeInfo GetApiThemeInfo(string themeName)
    {
        return DataManager.Instance.GetThemaInfoList().Find(i => i.name == themeName);
    }

    #region COROUTINES
    IEnumerator InitializeStage()
    {
        _puzzleImageParts.Clear();
        _puzzleImageParts.AddRange(
            Array.FindAll(
                _puzzleData.PuzzleImageParts,
                part => IsValidDifficulty(_modeData.PlayMode, part.ImageDifficulty)
            )
        );

        _maxDifference = Mathf.Min(
            Random.Range(_modeData.FindMinCount, _modeData.FindMaxCount + 1),
            _puzzleImageParts.Count
        );

        FoundDifference = 0;

        _puzzleImageParts.Shuffle();
        _puzzleImageParts.RemoveRange(_maxDifference, _puzzleImageParts.Count - _maxDifference);

        int separateIndex = Random.Range(0, _maxDifference);

        _hintItemButton.enabled = _hintItemAmount != 0;
        _frozenItemButton.enabled = _frozenItemAmount != 0;

        _leftCurtain.Show();
        _rightCurtain.Show();

        yield return _leftBoard.Setup(
            _puzzleData.ImageSprite,
            _puzzleImageParts.ToArray(),
            0,
            separateIndex
        );

        yield return _rightBoard.Setup(
            _puzzleData.ImageSprite,
            _puzzleImageParts.ToArray(),
            separateIndex,
            _puzzleImageParts.Count - separateIndex
        );

        _roundPlayTime = 0;
        _roundItemUsedCount = 0;

        Debug.Log("<color=#00FFFF> Start Round</color> " + _puzzleData.Id);
    }

    IEnumerator HideBoards()
    {
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_curtain_close);

        _leftCurtain.ShowFrom(CurtainDirection.RIGHT, 1f);
        _rightCurtain.ShowFrom(CurtainDirection.LEFT, 1f);

        NetworkManager.Instance.HideBanner(ADConstants.BANNER_INGAME);

        yield return new WaitForSeconds(CURTAIN_MOVE_TIME);
    }

    IEnumerator ShowBoards()
    {
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_curtain_open);

        _leftCurtain.HideTo(CurtainDirection.LEFT, CURTAIN_MOVE_TIME);
        _rightCurtain.HideTo(CurtainDirection.RIGHT, CURTAIN_MOVE_TIME);

        NetworkManager.Instance.ShowBanner(ADConstants.BANNER_INGAME);

        //yield return new WaitForSeconds(CURTAIN_MOVE_TIME);
        yield return null;
    }

    IEnumerator ShowSplash(SPLASH key)
    {

        _splashObjs[(int)key].SetActive(true);

        bool isEnd = false;

        while (!isEnd)
        {
            if(_splashObjs[(int)key].GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length
                < _splashObjs[(int)key].GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                isEnd = true;
            }

            yield return CommonConstants.WaitLoopSeconds;
        }

        _splashObjs[(int)key].SetActive(false);
    }

    IEnumerator ProcessStartThemeMode()
    {
        _currentAudioKey = (Random.Range(0f, 1f) < 0.5f) ?
            AudioDataKey.BGM_ingame_theme_mode1 : AudioDataKey.BGM_ingame_theme_mode2;

        SoundManager.Instance.PlayBgm(_currentAudioKey);


        yield return null;
    }

    IEnumerator ProcessRoundCompleted()
    {
        _totalPlayTime += _roundPlayTime;
        _totalItemUsedCount += _roundItemUsedCount;
        yield return ProcessCompleteStageRound();        
    }

    IEnumerator ProcessCompleteStageRound()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        foreach(GameObject go in _splashObjs)
        {
            go.SetActive(false);
        }

        if (_isFrozen)
        {
            yield return EndFrozen();
        }

        _timer.Pause();

        yield return new WaitForSeconds(_roundClearDelay);
        yield return HideBoards();
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_title_clear);

        _warningFrame.LeanCancel();
        _warningFrame.color *= GameConstants.TRANSPARENT;

        if (_isNoMiss)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.NOMISS_CLEAR);
        }
        else
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CLEAR);
        }

        ReportStageResult((isSuccess) =>
        {
            if (isSuccess)
            {
                StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupStageResult));
            }
            
            StateManager.Instance.ActiveBlockScreen(false);
        });

        
    }

    IEnumerator ProcessActivateHintDelayed(float soundDelay)
    {
        _leftBoard.ShowHint(_hintPuzzleTokenIndex);
        _rightBoard.ShowHint(_hintPuzzleTokenIndex);

        yield return new WaitForSeconds(soundDelay);

        _hintedAudioSource = SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.eff_hint_use);
    }
    IEnumerator ProcessTimeWarning()
    {
        _warningFrame.rectTransform.LeanCancel();
        _warningFrame.rectTransform.LeanAlpha(1.0f, _warningFrameBlinkInterval).
            setIgnoreTimeScale(true).
            setLoopPingPong();

        _hurryUpSource = SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_title_hurryup);
        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.HURRYUP);
        yield return ShowSplash(SPLASH.HURRYUP);

        while (_hurryUpSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        _hurryUpSource = null;

        _timeTickSource = SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.eff_time_warning_tick);

        _timeWarningCoroutine = null;
    }
    
    IEnumerator ProcessTimeOver()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        if (_hintedAudioSource != null)
        {
            _hintedAudioSource.Stop();
        }

        yield return HideBoards();

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_title_gameOver);

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.GAMEOVER);

        yield return ShowSplash(SPLASH.GAMEOVER);

        StateManager.Instance.ActiveBlockScreen(false);

        GameOverThemeMode();
    }

    public void ContinuePlay()
    {
        SoundManager.Instance.PlayBgm(_currentAudioKey);

        StartCoroutine(ProcessContinuePlay());
    }

    IEnumerator ProcessContinuePlay()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        _isCoutinued = true;

        yield return ShowBoards();

        float delay = _timer.IncreaseTime(_inGameData.GameContinueTime);

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CONTINUE_BONUSTIME);

        yield return ShowSplash(SPLASH.BONUSTIME);

        StateManager.Instance.ActiveBlockScreen(false);

        _timer.Resume();

        //yield return new WaitForSeconds(delay + BONUS_TIME_DELAY);
    }

    IEnumerator ProcessWrongAnimation()
    {
        _wrongMarkObj.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        _wrongMarkObj.SetActive(false);
    }

    IEnumerator ProcessWrongAnimTwo(Vector3 pos)
    {
        Transform parentsTransform = _wrongMarkObj.GetComponentInParent<Transform>();

        GameObject wrongMark = ObjectUtil.Instantiate(_wrongMarkObj.gameObject) as GameObject;

        wrongMark.transform.SetParent(parentsTransform);
        wrongMark.transform.localScale = Vector3.one;
        wrongMark.transform.position = pos;
        wrongMark.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        wrongMark.SetActive(false);
        Destroy(wrongMark);
    }
 
    IEnumerator ProcessCorrectTrail(Vector2 pos)
    {
        List<GameObject> starTrails = CreateCorrectTrail(pos);

        float trailSpeed = 0.3f;

        for (int i = 0; i < starTrails.Count; i++)
        {
            trailSpeed -= 0.05f;

            if (i == 0)
            {
                GameObject go = starTrails[i];
                go.SetActive(true);

                LeanTween.value(1f, 1.5f, 0.5f)
                    .setOnUpdate((float value) =>
                    {
                        go.transform.localScale = Vector3.one * value;
                    });

                yield return new WaitForSeconds(0.5f);

                LeanTween.move(starTrails[i], _correctIcon.transform.position, 0.3f)
                    .setEaseInExpo()
                    .setOnComplete(() =>
                    {
                        Destroy(go);
                        _correctIconAnimator.gameObject.SetActive(true);
                        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_mission_gain);
                        _correctIconAnimator.Play("start", -1, 0);
                    });

                continue;
            }

            yield return new WaitForSeconds(0.1f);

            GameObject objects = starTrails[i];

            starTrails[i].SetActive(true);
            LeanTween.move(objects, _correctIcon.transform.position, trailSpeed)
                .setEaseInExpo()
                .setOnComplete(() =>
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                    stringBuilder.Append(_foundDifference);
                    stringBuilder.Append("/");
                    stringBuilder.Append(_maxDifference);
                    _foundDifferenceText.text = stringBuilder.ToString();

                    Destroy(objects);
                });
        }

        yield return new WaitForSeconds(1.0f);
        _correctIconAnimator.gameObject.SetActive(false);
    }
    #endregion COROUTINES

    #region EVENTS
    public void OnPauseGame()
    {
        _pauseGame = true;

        _pauseCoroutine = StartCoroutine(HideBoards());

        _timer.Pause();

        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PauseState));
    }

    public void OnUseHintItem()
    {
        if (_hintPuzzleTokenIndex > INVALID_PUZZLE_TOKEN_INDEX)
        {
            return;
        }

        if (_hintItemAmount <= 0)
        {
            return;
        }

        DifferenceImageToken[] tokens = _leftBoard.tokens;
        if (!Array.TrueForAll(tokens, token => !token.isHinted))
        {
            return;
        }

        int tokenIndex = Array.FindIndex(tokens, token => {
            return (!token.isChecked && !token.isHinted);
        });

        if (tokenIndex < 0)
        {
            return;
        }

        _hintItemButton.enabled = false;

        ++_roundItemUsedCount;

        _hintPuzzleTokenIndex = tokenIndex;

        ApiManager.Instance.UseIngameItem(ItemType.HINT.ToString(), 0, apiresult => {
            if (apiresult.isSuccess)
            {
                if (apiresult.result.itemId.Equals(ItemType.HINT.ToString()))
                {
                    _hintItemAmount = apiresult.result.count;

                    _itemAmountText.text = _hintItemAmount.ToString();
                }

                StartCoroutine(ProcessActivateHintDelayed(_itemActivationDelay));

            }else
            {

            }
        });

        PlayHintRandomVoice();

    }

    public void OnUseFrozenItem()
    {
        if (_timer.isPaused || _frozenItemAmount <= 0)
        {
            return;
        }

        if (_frozenAnimator.gameObject.activeSelf)
        {
            return;
        }

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_timefreeze);

        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.USED_FREEZE_ITEM);

        _timer.Pause();
        _comboTimer.PauseSet();

        _frozenItemButton.enabled = false;

        float endTime = _roundPlayTime + _frozenTime;

        _frozenAnimator.gameObject.SetActive(true);

        _frozenTimerTxt.gameObject.SetActive(true);

        _frozenStartTime = _roundPlayTime;
        _nextTime = 1f;
        _isFrozen = true;
    }

    Coroutine _autoActionCoroutine;
    public void OnAutoCompleteRound()
    {
        if (_autoActionCoroutine == null)
        {
            _autoActionCoroutine = StartCoroutine(ProcessAutoCompleteRound());
        }
    }

    public void OnAutoFailureRound()
    {
        if (_autoActionCoroutine == null)
        {
            _autoActionCoroutine = StartCoroutine(ProcessAutoFailureRound());
        }
    }

    public void OnGiveUp()
    {
        StartCoroutine(ProcessGiveUp());
    }

    IEnumerator ProcessAutoCompleteRound()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        yield return new WaitForSeconds(1.0f);

        DifferenceImageToken[] tokens = _leftBoard.tokens;
        for (int i = 0, max = tokens.Length; i < max; ++i)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }

            if (!tokens[i].isChecked)
            {
                OnTouchCurrectPosition(i, true);
                yield return new WaitForSeconds(1.0f);
            }
        }

        StateManager.Instance.ActiveBlockScreen(false);

        _autoActionCoroutine = null;
    }

    IEnumerator ProcessAutoFailureRound()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        yield return new WaitForSeconds(1.0f);

        while (_timer.remainingTime > 0.0f)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }

            OnTouchWrongPosition(Vector3.zero);

            yield return new WaitForSeconds(1.0f);
        }

        StateManager.Instance.ActiveBlockScreen(false);

        _autoActionCoroutine = null;
    }

    IEnumerator ProcessGiveUp()
    {
        yield return new WaitForSeconds(1f);

        _timer.Pause();

        //yield return ShowSplash(SPLASH.GAMEOVER);

        _isCoutinued = true;

        OnTimeOver();
    }

    IEnumerator EndFrozen()
    {
        _frozenAnimator.SetBool("isOver", true);

        _isFrozen = false;

        _frozenItemButton.enabled = _frozenItemAmount != 0;

        _timer.Resume();
        _comboTimer.PauseSet();

        yield return new WaitForSeconds(_frozenAnimator.GetCurrentAnimatorStateInfo(0).length);

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_timefreezeBreak);

        ApiManager.Instance.UseIngameItem(ItemType.FREEZE_TIME.ToString(), 0, apiresult =>
        {
            if (apiresult.isSuccess)
            {
                if (apiresult.result.itemId.Equals(ItemType.FREEZE_TIME.ToString()))
                {
                    _frozenItemAmount = apiresult.result.count;
                    _frozenItemAmountText.text = _frozenItemAmount.ToString();
                }
            }
            else
            {

            }
        });

        _frozenAnimator.SetBool("isEnd", true);

        _frozenAnimator.gameObject.SetActive(false);
        _frozenTimerTxt.gameObject.SetActive(false);

        yield return null;
    }

    public void CalculateStarGauge()
    {
        List<int> baseScoreInfos = GameManager.Instance.InGameData.GetBaseScoreInfos();

        float gaugeRate = (float)_comboTimer._totalScore / (float)(baseScoreInfos[baseScoreInfos.Count -1]);

        _scoreGauge.fillAmount = gaugeRate;

        for(int i = 0; i < baseScoreInfos.Count; i++)
        {
            if(_comboTimer._totalScore >= baseScoreInfos[i])
            {
                if(!_starAnimGroup[i].gameObject.activeSelf)
                {
                    SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_starcoin_pickup);
                }
                _starAnimGroup[i].gameObject.SetActive(true);
            }
            else
            {
                _starAnimGroup[i].gameObject.SetActive(false);
            }

        }

        _starPointText.text = SystemUtil.GetCommaText(_comboTimer._totalScore);
    }

    public void SetPositionStarImage()
    {
        List<int> baseScoreInfos = GameManager.Instance.InGameData.GetBaseScoreInfos();

        float maxScoreValue = (float)(baseScoreInfos[baseScoreInfos.Count - 1]);

        float originStarXPos = _starGroup[0].transform.localPosition.x;

        for (int i = 0; i < baseScoreInfos.Count; i++)
        {
            float xPos = (baseScoreInfos[i] / maxScoreValue) * _scoreGauge.rectTransform.rect.width;

            _starGroup[i].GetComponent<RectTransform>().anchoredPosition 
                = new Vector2(xPos, _starGroup[i].transform.localPosition.y);
        }

    }

    public void SetTestMode()
    {
        testOption = (testOption == TestOption.OPTIONA) ? TestOption.OPTIONB : TestOption.OPTIONA;
    }

    #endregion EVENTS

    #region CALLBACKS
    void OnFindDifference(int imageTokenIndex, bool isAuto = false)
    {
        _timer.InputCorrectStack();

        if (_hintPuzzleTokenIndex == imageTokenIndex)
        {
            _hintPuzzleTokenIndex = INVALID_PUZZLE_TOKEN_INDEX;
            _hintedAudioSource.Stop();

            _hintItemButton.enabled = _hintItemAmount != 0;
        }

        DifferenceImageToken[] leftTokens = _leftBoard.tokens;
        DifferenceImageToken[] rightTokens = _rightBoard.tokens;

        _missStack = 0;

        _comboTimer.ComboStart(leftTokens[imageTokenIndex].transform.position, rightTokens[imageTokenIndex].transform.position);

        CalculateStarGauge();

        ++FoundDifference;

        if(isAuto)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(FoundDifference);
            stringBuilder.Append("/");
            stringBuilder.Append(_maxDifference);
            _foundDifferenceText.text = stringBuilder.ToString();
        }

        if (_foundDifference >= _maxDifference)
        {
            _comboTimer.ComboStop();
            SoundManager.Instance.StopBgm(_currentAudioKey);
            StartCoroutine(ProcessRoundCompleted());
        }
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_contain_O);
    }

    void OnTouchCurrectPosition(int tokenIndex)
    {
        OnFindDifference(tokenIndex);

        _leftBoard.Check(tokenIndex);
        _rightBoard.Check(tokenIndex);
    }

    void OnTouchCurrectPosition(int tokenIndex, bool isAuto=false)
    {
        OnFindDifference(tokenIndex, isAuto);

        _leftBoard.Check(tokenIndex);
        _rightBoard.Check(tokenIndex);
    }

    void OnTouchWrongPosition(Vector2 position)
    {
        if (FoundDifference >= _maxDifference)
        {
            return;
        }

        _wrongMarkObj.transform.position = position;

        _comboTimer.ComboStop();

        if(testOption == TestOption.OPTIONA)
            StartCoroutine(ProcessWrongAnimation());
        else
            StartCoroutine(ProcessWrongAnimTwo(position));

        //isShield Item 
        if (!_isShieldItem)
        {
            _missStack++;

            _isNoMiss = false;

            StartCoroutine(BoardShake());

            Debug.Log("현재 미스스택 : " + _missStack);

            SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_contain_X);

            _timer.DecreaseTime(_modeData.RoundTimePenalty);
        }
        else
        {
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.eff_shield_active);

            StartCoroutine(ProcessShieldAnimation());
        }

        MissRandomVoice();

    }

    void OnStarTrail(Vector2 pos)
    {
        StartCoroutine(ProcessCorrectTrail(pos));
    }

    void OnTimeWarning()
    {
        if (_timeWarningCoroutine != null)
        {
            StopCoroutine(_timeWarningCoroutine);
        }

        _timeWarningCoroutine = StartCoroutine(ProcessTimeWarning());
    }

    void OnWarningReleased()
    {
        if (_timeWarningCoroutine != null)
        {
            StopCoroutine(_timeWarningCoroutine);

            _timeWarningCoroutine = null;
        }

        if (_hurryUpSource != null)
        {
            _hurryUpSource.Stop();
            _hurryUpSource = null;
        }

        if (_timeTickSource != null)
        {
            _timeTickSource.Stop();
            _timeTickSource = null;
        }

        //
        _warningFrame.rectTransform.LeanCancel();
        _warningFrame.rectTransform.LeanAlpha(0.0f, _warningFrameBlinkInterval * _warningFrame.color.a).
            setIgnoreTimeScale(true);
    }
    
    void OnTimeOver()
    {
        OnWarningReleased();

        foreach (GameObject go in _splashObjs)
        {
            go.SetActive(false);
        }

        SoundManager.Instance.StopBgm(_currentAudioKey);

        StartCoroutine(ProcessTimeOver());

        if(_isFrozen)
        {
            StartCoroutine(EndFrozen());
        }
    }

    public List<GameObject> CreateCorrectTrail(Vector2 pos)
    {
        Transform parentsTransform = _starObj.GetComponentInParent<Transform>();

        float magnification = 1.5f;

        List<GameObject> trailArr = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            if (i % 2 == 0)
            {
                GameObject starTrail = ObjectUtil.Instantiate(_starObj.gameObject) as GameObject;
                starTrail.transform.SetParent(parentsTransform);
                starTrail.transform.localScale = Vector3.one;
                starTrail.transform.position = pos;
                magnification /= 2;
                trailArr.Add(starTrail);
            }
            else
            {
                GameObject starTrail = ObjectUtil.Instantiate(_starTwoObj.gameObject) as GameObject;
                starTrail.transform.SetParent(parentsTransform);
                starTrail.transform.localScale = Vector3.one;
                starTrail.transform.position = pos;

                trailArr.Add(starTrail);
            }
        }

        return trailArr;
    }

    void ReportStageResult(Action<bool> callback)
    {
        StagePlayResult stagePlayResult = new StagePlayResult();

        stagePlayResult.seq = _inGameData.RequestSeq;
        stagePlayResult.score = _comboTimer._totalScore;
        stagePlayResult.remainTimeSeconds = (int)_timer.remainingTime;
        stagePlayResult.isContinue = _isCoutinued;

        ApiManager.Instance.SendPlayResult(stagePlayResult, (result) =>
        {
            if (result.isSuccess)
            {
                GameManager.Instance.InGameData.SetStagePlayReward(result.result);
            }
            else
            {
                MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_FAIL_ALERT);
            }
            callback(result.isSuccess);
        });

    }
    #endregion CALLBACKS

    #region UTILITIES
    public static bool HasValidDifficulty(ThemeDifficulty  playMode, PuzzleImagePart[] parts)
    {
        for (int i = 0, max = parts.Length; i < max; ++i)
        {
            if (IsValidDifficulty(playMode, parts[i].ImageDifficulty))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsValidDifficulty(ThemeDifficulty  playMode, ImageDifficulty difficulty)
    {
        PuzzleModeData modeData = DataManager.Instance.GetPuzzleModeData(playMode);
        switch (modeData.FindMixRule)
        {
            case FindMixRule.easy_nomal:
                return (difficulty == ImageDifficulty.EASY ||
                        difficulty == ImageDifficulty.NORMAL);
            case FindMixRule.nomal_hard:
                return (difficulty == ImageDifficulty.NORMAL ||
                        difficulty == ImageDifficulty.HARD);
            case FindMixRule.easy_hard:
                return (difficulty == ImageDifficulty.EASY ||
                        difficulty == ImageDifficulty.HARD);
        }

        return true;
    }
    #endregion UTILITIES    

    #region FORUSINGITEM
    private void InitUseItemObj()
    {
        _scoreBonusIcon.gameObject.SetActive(false);
        _timeBonusIcon.gameObject.SetActive(false);
        _shieldIcon.gameObject.SetActive(false);

        _shieldObj.SetActive(false);
        _scoreBonusObj.SetActive(false);
    }

    private void SetUseItemObj()
    {
        _isTimerPlusItem = _inGameData.ItemIdList.Find(item => item.ToString() == ItemType.EXTRA_TIME.ToString()) != null;
        _isShieldItem = _inGameData.ItemIdList.Find(item => item.ToString() == ItemType.ONE_MISS_DEFENCE.ToString()) != null;
        _isScoreBonus = _inGameData.ItemIdList.Find(item => item.ToString() == ItemType.BONUS_SCORE.ToString()) != null;

        _scoreBonusIcon.gameObject.SetActive(_isScoreBonus);
        _timeBonusIcon.gameObject.SetActive(_isTimerPlusItem);
        _shieldIcon.gameObject.SetActive(_isShieldItem);

        _scoreBonusIcon.color = GameConstants.OPAQUE;
        _timeBonusIcon.color = GameConstants.OPAQUE;
        _shieldIcon.color = GameConstants.OPAQUE;
    }

    IEnumerator SparkleItemExpress()
    {
        if (_isScoreBonus)
        {
            LeanTween.alpha(_scoreBonusIcon.rectTransform, 0.1f, 0.2f)
                    .setRecursive(true)
                    .setLoopPingPong(3);
        }

        if (_isTimerPlusItem)
        {
            LeanTween.alpha(_timeBonusIcon.rectTransform, 0.1f, 0.2f)
                    .setRecursive(true)
                    .setLoopPingPong(3);
        }

        if (_isShieldItem)
        {
            LeanTween.alpha(_shieldIcon.rectTransform, 0.1f, 0.2f)
                    .setRecursive(true)
                    .setLoopPingPong(3);
        }

        yield return new WaitForSeconds(1f);

        yield return ActivateInOrderItem();
    }

    IEnumerator ActivateInOrderItem()
    {
        foreach (string str in _inGameData.ItemIdList)
        {
            if (str.Equals(ItemType.EXTRA_TIME.ToString()))
            {
                _timeBonusIcon.gameObject.SetActive(false);
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_timebonus_active);
                yield return new WaitForSeconds(0.1f);
                SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.EXTRATIME_ADDED);

                _timer.IncreaseExtraTime(GameConstants.INGAME_ADD_TIME_EXTRATIME);
            }
            else if (str.Equals(ItemType.ONE_MISS_DEFENCE.ToString()))
            {
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_shield_Equip);
                _shieldIcon.gameObject.SetActive(false);
                ShowInitShield(_isShieldItem);
            }
            else if (str.Equals(ItemType.BONUS_SCORE.ToString()))
            {
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_result_item_stamp);
                _scoreBonusIcon.gameObject.SetActive(false);
                _scoreBonusObj.SetActive(true);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator ProcessShieldAnimation()
    {
        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.USED_DEFENCE_ITEM);

        ShowShieldEffect();
        _isShieldItem = false;
        yield return new WaitForSeconds(0.7f);
        ShowInitShield(false);
    }

    private void ShowInitShield(bool isShow)
    {
        _shieldObj.SetActive(isShow);
        _shieldAnim.gameObject.SetActive(false);
    }

    private void ShowShieldEffect()
    {
        _shieldAnim.gameObject.SetActive(true);
    }
    #endregion

    #region FOR RANDOM VOICE
    private void PlayHintRandomVoice()
    {
        if (_hintItemAmount == 1)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.USED_LAST_HINT);
            return;
        }

        float randomRate = Random.Range(0, 1.00f);

        if (randomRate <= 0.01)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.USED_HINT_ITEM_1PER);
        }
        else
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.USED_HINT_ITEM);
        }
    }

    private void MissRandomVoice()
    {
        float maxTime = _modeData.LimitTime;

        if (_missStack == 5)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.MISS_FIVETIME);
        }
        else if (_missStack == (int)(maxTime / _modeData.RoundTimePenalty)-1)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.MISS_MAXTIME);
        }
        else if (_missStack == 7)
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.MISS_EIGHTTIME);
        }
        else
        {
            SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.MISS);
        }
    }
    #endregion


    #region Board Shake
    IEnumerator BoardShake()
    {
        Vibration.CreateOneShot(500);

        LeanTween.cancel(_leftBoard.gameObject);
        LeanTween.cancel(_rightBoard.gameObject);

        LeanTween.rotateZ(_leftBoard.gameObject, 1.05f, _shakeSpeed)
        .setLoopPingPong(_shakeVal);

        LeanTween.rotateZ(_rightBoard.gameObject, -1.05f, _shakeSpeed)
        .setLoopPingPong(_shakeVal);

        _leftBoard.transform.localRotation = _leftOriginBoardRotate;
        _rightBoard.transform.localRotation = _rightOriginBoardRotate;

        yield return null;
    }
    #endregion
}


