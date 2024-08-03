using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameover : NotBackablePopupState
{
    public const string PARAM_SCORE = "resultScore";
    public const string PARAM_BEST_SCORE_UPDTATED = "RenewalScore";
    public const string PARAM_REWARDED_COIN = "resultCoin";
    public const string PARAM_COIN_COUNTER = "InGameCoinCounter";

    public override string stateName { get { return GameConstants.STATENAME_POPUPGAMEOVER; } }

    [Header("Game Over")]
    [SerializeField] Button _watchButton;
    [SerializeField] Button _shareButton;
    [SerializeField] Button _rePlayButton;

    [SerializeField] TMP_Text _resultCoinText, _resultScoreText, _bestScoreText;
    [SerializeField] GameObject _coinEffect;

    [Header("Icon")]
    [SerializeField] Image _bestIcon;
    [SerializeField] float _bestIconTime = 0.5f;

    Vector3 _originBestIconScale;

    int _rewardedCoin;
    bool _showBestIcon;

    public override IEnumerator OnInitialize()
    {
        yield return base.OnInitialize();

        _originBestIconScale = _bestIcon.transform.localScale;

        _coinEffect.SetActive(false);

        _shareButton.onClick.AddListener(() => NetworkManager.Instance.AppShare());
        _rePlayButton.onClick.AddListener(OnReplay);
        _watchButton.onClick.AddListener(() => StartCoroutine(ProcessDoubleCoin()));
    }

    public override IEnumerator OnLoad(Dictionary<string, object> args = null)
    {
        yield return base.OnLoad(args);

        NetworkManager.Instance.LoadRewardVideoAd(GameConstants.ADUNIT_REWARD);

        if (GameManager.instance.replayInterstitialEnabled)
        {
            NetworkManager.Instance.LoadInterstitialAd(GameConstants.ADUNIT_INTERSTITIAL_GAMEEND_REPLAY);
        }

        ShowWatchButton(true);

        int score = GetParam(PARAM_SCORE, 0);
        int highscore = Mathf.Max(DataManager.Instance.UserData.Score, score);
        _rewardedCoin = GetParam(PARAM_REWARDED_COIN, 0);
        _showBestIcon = GetParam(PARAM_BEST_SCORE_UPDTATED, false);

        _resultScoreText.text = SystemUtil.GetCommaText(score);
        
        _bestIcon.gameObject.SetActive(false);

        _bestScoreText.text = SystemUtil.GetCommaText(highscore);
        _resultCoinText.text = SystemUtil.GetCommaText(_rewardedCoin);

        _watchButton.interactable = true;
        _rePlayButton.interactable = true;
    }

    public override IEnumerator OnEntered()
    {
        yield return base.OnEntered();

        if (_showBestIcon)
        {
            _bestIcon.gameObject.SetActive(true);

            _bestIcon.transform.localScale = _originBestIconScale * 2.0f;
            _bestIcon.color *= GameConstants.TRANSPARENT;

            _bestIcon.LeanCancel();
            _bestIcon.LeanAlpha(0.0f, 1.0f, _bestIconTime);
            _bestIcon.LeanScale(_originBestIconScale, _bestIconTime).
                setEaseOutBack();

            yield return new WaitForSeconds(_bestIconTime);

            SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_NEW_RECORD_STEMP);

            yield return new WaitForSeconds(0.2f);

            SoundManager.Instance.PlayUISoundInstance(GameConstants.UIFX_RECORD_RENEWAL);
        }
    }

    #region COROUTINES
    IEnumerator ProcessDoubleCoin()
    {
        ShowWatchButton(false);

        _shareButton.interactable = false;
        _rePlayButton.interactable = false;

        bool isClosedResult = false;
        bool closed = false;
        bool rewarded = false;

        Action<Reward> onRewarded = (reward) => rewarded = true;
        Action<bool> onCanceled = (close) => {
            closed = true;
            isClosedResult = close;
        };

        NetworkManager.Instance.ShowRewardVideoAd(
            GameConstants.ADUNIT_REWARD,
            onRewarded,
            onCanceled
        );

        while (!closed)
        {
            yield return new WaitForEndOfFrame();
        }

        //광고 실패(로드, 호출 등)
        if (!isClosedResult)
        {
            PopupInstanceMsg.ShowPopupInstanceMsg(GameConstants.AD_FAIL_MSG, 2f, InstanceMsgType.WARNING);
        }

        if (rewarded)
        {
            _coinEffect.SetActive(true);

            LeanTween.value(_rewardedCoin, _rewardedCoin * 2, 0.5f).
                setOnUpdate(val => _resultCoinText.text = SystemUtil.GetCommaText((int)val));

            DataManager.Instance.UserData.AddCoin(_rewardedCoin);    
        }

        _shareButton.interactable = true;
        _rePlayButton.interactable = true;
    }
    #endregion COROUTINES

    private void ShowWatchButton(bool isShow)
    {
        _watchButton.gameObject.SetActive(isShow);
        _watchButton.interactable = isShow;
    }

	private void OnReplay()
    {
        _watchButton.interactable = false;
        _rePlayButton.interactable = false;

		StartCoroutine(ProcessOnReplay());
    }

    #region COROUTINES
    IEnumerator ProcessOnReplay()
    {
        if (GameManager.instance.replayInterstitialEnabled)
        {
            bool closed = false;

            Action<bool> onClosed = state => {
                closed = true;
                if (state) {
                    GameManager.instance.ResetReplayInterstitialTime();
                }
            };

            NetworkManager.Instance.ShowInterstitialAd(
                GameConstants.ADUNIT_INTERSTITIAL_GAMEEND_REPLAY,
                onClosed
            );

            while (!closed)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        UICounter coinCounter = GetParam<UICounter>(PARAM_COIN_COUNTER, null);
        if (coinCounter != null)
        {
            coinCounter.Setup(DataManager.Instance.UserData.Coin);
            while (coinCounter.nowCounting)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(1.0f);
        }

        NetworkManager.Instance.UnLoadRewardVideoAd(GameConstants.ADUNIT_REWARD);
        StateManager.instance.OpenStateForced(GameConstants.STATENAME_INGAME);
    }
    #endregion COROUTINES
}
