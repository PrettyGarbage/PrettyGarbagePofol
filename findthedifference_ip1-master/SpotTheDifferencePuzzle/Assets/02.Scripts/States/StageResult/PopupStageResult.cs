using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupStageResult : PopupState
{
    [Header("Resource Setting")]
    [SerializeField] private Animator[] _starAnimGroup;
    [SerializeField] private TMP_Text _basicScoreTxt;
    [SerializeField] private TMP_Text _itemPlusScoreTxt;
    [SerializeField] private TMP_Text _timeScoreTxt;
    [SerializeField] private TMP_Text _totalScoreTxt;

    [Header("Expression")]
    [SerializeField] private Image _bonusIcon;
    [SerializeField] private float _animationTime;

    [SerializeField] private Image _backGround;

    [SerializeField] Animator _clearBackgroundAnim;
    [SerializeField] Animator _clearAnim;

    private InGameData _inGameData;

    private int _totalScore;

    private bool _isOpen = false;

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        _inGameData = GameManager.Instance.InGameData;

        _backGround.GetComponent<Button>().enabled = false;

        for (int i = 0; i < _starAnimGroup.Length; i++)
        {
            _starAnimGroup[i].gameObject.SetActive(false);
        }

        _isOpen = false;

        ScoreValueSetting();
    }

    public override IEnumerator OnOpening()
    {
        _popup.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
        yield return ClearAniFlow();

        yield return base.OnOpening();
    }

    public override void OnPause()
    {
        base.OnPause();

        _clearBackgroundAnim.gameObject.SetActive(false);
    }

    IEnumerator ClearAniFlow()
    {
        _clearBackgroundAnim.gameObject.transform.localScale = Vector3.one;

        _clearAnim.gameObject.SetActive(true);

        _clearBackgroundAnim.gameObject.SetActive(true);

        while (!_clearAnim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.End"))
        {
            yield return null;
        }

        LeanTween.value(1.0f, 2.5f, 0.5f)
            .setOnUpdate((float value) =>
            {
                _clearAnim.gameObject.SetActive(false);
                _clearBackgroundAnim.gameObject.transform.localScale
                = new Vector3(value, value);
            });
    }

    public override IEnumerator OnPostOpen()
    {
        yield return base.OnPostOpen();

        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.eff_result_score_scene);

        yield return new WaitForSeconds(1f);

        yield return StageResultExpress();
    }

    public void ScoreValueSetting()
    {
        _totalScore = 0;
        _basicScoreTxt.text = "0";
        _timeScoreTxt.text = "0";
        _itemPlusScoreTxt.text = "0";
        _totalScoreTxt.text = "0";

        _bonusIcon.gameObject.SetActive(false);
    }

    public void OnNextResultFlow()
    {
        _isOpen = true;
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(ThemeRoundCompletedState));
        StateManager.Instance.CloseState(typeof(PopupStageResult));
    }

    public List<AudioDataKey> GetAudioSoundKeyList()
    {
        List<AudioDataKey> soundInfos = new List<AudioDataKey>();
        soundInfos.Add(AudioDataKey.ui_result_star_impact_1);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_2);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_3);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_4);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_5);

        return soundInfos;
    }

    #region NumberRoll
    IEnumerator NumberUpTextExpress(TMP_Text text,float curNum, float tarNum, AudioDataKey audio)
    {
        List<int> baseScores = GameManager.Instance.InGameData.GetBaseScoreInfos();

        List<AudioDataKey> soundsInfos = GetAudioSoundKeyList();

        SoundManager.Instance.PlayUISoundInstance(audio);

        LeanTween.value(curNum, tarNum, 1f)
            .setOnUpdate((float value) =>
            {
                text.text = (SystemUtil.GetCommaText((int)value)).ToString();
                _totalScoreTxt.text = (SystemUtil.GetCommaText((int)(_totalScore + value)));

                for (int i = 0; i < baseScores.Count; i++)
                {
                    if (!_starAnimGroup[i].gameObject.activeSelf && (_totalScore + (int)value) > baseScores[i])
                    {
                        _starAnimGroup[i].gameObject.SetActive(true);
                        SoundManager.Instance.PlayUISoundInstance(soundsInfos[i]);
                    }
                }
            })
            .setOnComplete(()=>
            {
                _totalScore += (int)tarNum;
            });
        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator StageResultExpress()
    {
        List<int> scores = new List<int>();
        scores.Add(_inGameData.StagePlayReward.basicScore);
        scores.Add(_inGameData.StagePlayReward.timeBonusScore);
        scores.Add(_inGameData.StagePlayReward.bonusScore);

        List<TMP_Text> texts = new List<TMP_Text>();
        texts.Add(_basicScoreTxt);
        texts.Add(_timeScoreTxt);
        texts.Add(_itemPlusScoreTxt);

        for(int i = 0; i < scores.Count; i ++)
        {
            if(scores[i] != 0)
            {
                if(texts[i].Equals(_itemPlusScoreTxt))
                {
                    SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_result_item_stamp);
                    yield return new WaitForSeconds(0.2f);
                    SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.BONUS_SCORE_ATTAIN);
                    _bonusIcon.gameObject.SetActive(true);
                    _bonusIcon.transform.localScale = Vector3.one * 2f;
                    LeanTween.scale(_bonusIcon.gameObject, Vector3.one * 1.5f, 1f)
                        .setEaseInElastic();
                }

                yield return NumberUpTextExpress(texts[i], 0, scores[i], AudioDataKey.ui_result_score_item_bonus_short);
            }
        }

        yield return FinalExpress();

        yield return new WaitForSeconds(5f);

        if (!_isOpen)
        {
            OnNextResultFlow();
        }
    }

    public IEnumerator FinalExpress()
    {
        yield return new WaitForSeconds(0.2f);
        _totalScoreTxt.gameObject.SetActive(true);
        _totalScoreTxt.transform.localScale = Vector3.one * 2f;

        LeanTween.scale(_totalScoreTxt.gameObject, Vector3.one * 1.5f, 1f)
            .setEaseInElastic()
            .setOnComplete(()=>
            {
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_result_score_complete);
                _backGround.GetComponent<Button>().enabled = true;
            });
    }
    #endregion
}