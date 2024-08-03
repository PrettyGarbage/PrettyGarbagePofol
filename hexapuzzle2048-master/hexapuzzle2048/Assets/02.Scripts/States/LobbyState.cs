using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyState : State
{
    public override string stateName { get { return GameConstants.STATENAME_LOBBY; } }

    private Dictionary<string, object> popupSettingSrgs = new Dictionary<string, object>();

    [Header("Buttons")]
    [SerializeField] Button _settingButton;
    [SerializeField] Button _rankingButton;
    [SerializeField] Button _playButton;

    [Header("Text")]
    [SerializeField] TMP_Text _coinText;
    [SerializeField] TMP_Text _scoreText;

    [SerializeField] GameObject _coinEffect;
    private int _coin;
    private bool _isFirstLoad = true;
    
    public override IEnumerator OnInitialize()
    {
        yield return base.OnInitialize();

        _settingButton.onClick.AddListener(ShowPopupSetting);
        _rankingButton.onClick.AddListener(OnLeaderboard);
        _playButton.onClick.AddListener(OnPlay);

        _coinEffect.SetActive(false);
    }

    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
    {
        yield return base.OnLoad(args);

        NetworkManager.Instance.RequestBanner(
            GameConstants.ADUNIT_BANNER_BOTTOM,
            AdSize.Banner,
            AdPosition.Bottom
        );

        if(_isFirstLoad){
            NetworkManager.Instance.SignIn();
            _isFirstLoad = false;
        }

        _coin = DataManager.Instance.UserData.Coin;
        _coinText.text = SystemUtil.GetCommaText(_coin);
        _scoreText.text = SystemUtil.GetCommaText(DataManager.Instance.UserData.Score);

        SoundManager.Instance.PlayBgm(GameConstants.BGM_LOBBY);
    }

    public override IEnumerator OnEnter()
    {
        yield return base.OnEnter();

        AnalyticsManager.Instance.EventLog(GameConstants.EVENTLOG_LOBBY_SHOW);
    }

    public override IEnumerator OnEntered()
    {

        yield return base.OnEntered();

        if (!DataManager.Instance.UserData.IsGetDailyReward())
        {
            StateManager.instance.PushState(GameConstants.STATENAME_POPUPDAILYREWARD);
            while (!StateManager.instance.topState.stateName.Equals(stateName))
            {
                yield return CommonConstants.WaitLoopSeconds;
            }
            UpdateCoinText();
        }
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPopupSetting();
        }
    }

    public override IEnumerator OnUnload()
    {
        SoundManager.Instance.StopBgm(GameConstants.BGM_LOBBY);

        yield return base.OnUnload();
    }

    public void UpdateCoinText()
    {
        _coinEffect.SetActive(true);

        LeanTween.value(_coin, DataManager.Instance.UserData.Coin, 1f).
        setOnUpdate((float val) => {
            _coinText.text = SystemUtil.GetCommaText((int)val);
        });
        _coin = DataManager.Instance.UserData.Coin;
    }

    private void ShowPopupSetting()
    {
        StateManager.instance.PushState(GameConstants.STATENAME_POPUPSETTING);
    }

    public void OnLeaderboard()
    {
        NetworkManager.Instance.ShowLeaderBoard();
    }

    public void OnPlay()
    {
        PlayData playData = DataManager.Instance.GetPlayData();
#if UNITY_EDITOR
        //// Combo Test
        // playData = new PlayData(
        // 	989,
        // 	2700,
        // 	new int[,] {
        // 		{ 0, 0, 3, 1, 7, 6, 0 },
        // 		{ 0, 9, 8, 4, 2, 0, 0 },
        // 		{ 0, 9, 3, 8, 4, 2, 3 },
        // 		{ 1, 2, 6, 0, 0, 1, 1 },
        // 		{ 0, 2, 6, 7, 5, 3, 5 },
        // 		{ 0, 1, 7, 5, 3, 2, 0 },
        // 		{ 0, 0, 6, 2, 2, 1, 0 }
        // 	},
        // 	new LastBlocks(new int[] { 6, 1 }, 0),
        // 	new ItemUseCountInfo(),
        // 	0);

        // // ContinueTest
        // playData = new PlayData(
        // 	989,
        // 	2700,
        // 	new int[,] {
        // 		{  0,  0, 11,  1, 11,  6,  0 },
        // 		{  0,  9,  8,  4,  2, 10,  0 },
        // 		{  0,  9,  3, 10,  4,  2,  3 },
        // 		{  1, 11, 11,  0, 11,  1, 11 },
        // 		{  0,  2,  6, 10,  5,  3,  5 },
        // 		{  0,  1,  7,  5, 11,  2,  0 },
        //         {  0,  0, 11,  2,  2, 11,  0 }
        // 	},
        // 	new LastBlocks(new int[] { 1 }, 0),
        // 	new ItemUseCountInfo(),
        // 	0);
#endif
        Dictionary<string, object> args = new Dictionary<string, object>() {
            { InGameState.PARAM_PLAY_DATA, playData }
        };

        StateManager.instance.OpenState(GameConstants.STATENAME_INGAME, args);
    }
}