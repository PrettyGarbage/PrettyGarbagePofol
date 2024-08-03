using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class StageCard : MonoBehaviour
{
    //[SerializeField] Image _image;

    [SerializeField] Image[] _stars;

    PuzzleImageData _imageData;
    ThemeInfo _themeInfo;

    [Header("StageInfo")]
    [SerializeField] string _StageName;
    [SerializeField] int _UserScore;
    [SerializeField] int _UserStarCoin;
    [SerializeField] float _shakeTime;
    [SerializeField] TMP_Text _stageText;
    [SerializeField] SpriteFont _stageCountText;
    [SerializeField] SpriteFont _lockStageCountText;

    [Header("BackGround")]
    [SerializeField] Color _unlockTextColor;
    [SerializeField] Color _lockTextColor;
    [SerializeField] Image _activeBackGround;
    [SerializeField] Image _lockBackground;


    //Member Variable
    ThemeDifficulty _themeDifficulty;
    StageInfo _stageInfo;
    UserStageInfo _userStageInfo;
    bool _isPreStageUnlock;
    int _stageIndex;
    bool _isShakeNow = false;

    InGameData _inGameData;

    private void Update()
    {
        if (_isShakeNow)
        {
            LeanTween.moveLocalX(gameObject, gameObject.transform.localPosition.x + 3, 0.1f).setEaseShake();
        }
    }

    public void Initialize(int stageIndex, StageInfo stageInfo)
    {
        _inGameData = GameManager.Instance.InGameData;

        _stageIndex = stageIndex;
        _themeDifficulty = _inGameData.ThemeDifficulty;

        string stageNoString = (stageInfo.stageNo < 10 ? CommonConstants.ZERO_STRING : string.Empty) + stageInfo.stageNo;
        _lockStageCountText.Text = stageNoString;
        _stageCountText.Text = stageNoString;

        _lockStageCountText.gameObject.SetActive(!IsUnLockCheck(stageIndex));

        _stageInfo = stageInfo;
    }

    public void Play()
    {
        if (!IsUnLockCheck(_stageIndex))
        {
            SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_lock_contents);
            StartCoroutine(ShakeMe());
            return;
        }
        SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_button_stage_card_flip);
        GameManager.Instance.InGameData.SetReadyData(_stageIndex);
        StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), false);

    }

    bool IsUnLockCheck(int stageIndex)
    {
        if (stageIndex == 0)
        {
            return true;
        }

        return _inGameData.IsPreStageClaimReward(stageIndex);
    }

    public void Refresh(int seq)
    {
        Color textColor = (IsUnLockCheck(seq)) ? _unlockTextColor : _lockTextColor;

        textColor.a = 1f;

        _lockBackground.gameObject.SetActive(!IsUnLockCheck(seq));

        StarPointUpdate(seq);

        GameManager.Instance.InGameData.IsNextStage(_stageIndex);

        _activeBackGround.gameObject.SetActive(_inGameData.IsNextStage(seq));

        _stageText.color = textColor; 
    }
    public void StarPointUpdate(int stageSeq)
    {
        UserStageInfo userStageInfo = GameManager.Instance.InGameData.GetUserStageInfo(stageSeq);

        int starCoin = (_themeDifficulty == ThemeDifficulty.THEME_BASIC) ? userStageInfo.stageDetailBasic.userStarCoin
            : userStageInfo.stageDetailMaster.userStarCoin;

        for(int i = 0; i < _stars.Length; i ++)
        {
            _stars[i].gameObject.SetActive(i < starCoin);
        }        
    }

    IEnumerator ShakeMe()
    {
        Vector3 originPos = gameObject.transform.localPosition;
        _isShakeNow = true;
        gameObject.GetComponent<Button>().interactable = false;

        yield return new WaitForSeconds(_shakeTime);
        _isShakeNow = false;
        yield return new WaitForSeconds(0.1f);

        gameObject.transform.localPosition = originPos;
        gameObject.GetComponent<Button>().interactable = true;
    }
}