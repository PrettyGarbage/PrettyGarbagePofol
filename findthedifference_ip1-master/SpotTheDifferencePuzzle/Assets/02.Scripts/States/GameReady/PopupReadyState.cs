using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class PopupReadyState : State
{
    [Header("Popup - Ready")]
	[SerializeField] Image _thumbnail;
	[SerializeField] TMP_Text _stageLabel;
    [Header("themeDifficulty")]
    [SerializeField] GameObject _normalDifficultyObject;
    [SerializeField] GameObject _masterDifficultyObject;

    [Header("Item Info")]
    private ReadyItemCtrl[] _readyItemCtrls;
    [SerializeField]
    private ReadyItemCtrl _pfReadyItemCtrl;
    [SerializeField]
    private Transform _readyItemContents;
    [SerializeField]
    private ItemType[] _itemTypes;

    [Header("Mission")]
    [SerializeField]
    private MissionInfoCtrl _missionInfoCtrl;

    [Header("Play button")]
    [SerializeField]
    private Animator _playButtonAnimator;
    [SerializeField]
    private Button _playBtn;
  
    private PuzzleImageData _imageData;
    private InGameData _inGameData;
    private int _findDifferenceCount;

    private int _requestPlayInfoId = 0;
    
    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

        yield return GameManager.Instance.GetUserStageInfo();

        _inGameData = GameManager.Instance.InGameData;
       _imageData = DataManager.Instance.GetThemaPuzzleData(_inGameData.ThemeInfo.id, _inGameData.StageIndex);

        //스테이지 이름 및 난이도.
        _stageLabel.text = _inGameData.StageInfo.name;
        _normalDifficultyObject.SetActive(_inGameData.ThemeDifficulty == ThemeDifficulty.THEME_BASIC);
        _masterDifficultyObject.SetActive(_inGameData.ThemeDifficulty == ThemeDifficulty.THEME_MASTER);

        //미션.
        PuzzleModeData puzzleModeData = DataManager.Instance.GetPuzzleModeData(_inGameData.ThemeDifficulty);
        _findDifferenceCount = Random.Range(puzzleModeData.FindMinCount, puzzleModeData.FindMaxCount + 1);
        _missionInfoCtrl.SetMissionInfo(_findDifferenceCount, _inGameData.IsStageClaimReward(_inGameData.StageIndex));

        //썸네일
        _thumbnail.sprite = _imageData.ImageSprite;

        //User's Item 
        SetReadyItem();

        //play button
        _playButtonAnimator.ResetTrigger("show");

        _playBtn.interactable = true;

    }

    private void SetReadyItem()
    {
        if (_readyItemCtrls == null)
        {
            _readyItemCtrls = new ReadyItemCtrl[_itemTypes.Length];
            for (int i = 0; i < _readyItemCtrls.Length; i++)
            {
                ReadyItemCtrl readyItemCtrl = Instantiate<ReadyItemCtrl>(_pfReadyItemCtrl, _readyItemContents);
                readyItemCtrl.SetItem(_itemTypes[i]);
                _readyItemCtrls[i] = readyItemCtrl;
            }
        }
        else {
            for (int i = 0; i < _readyItemCtrls.Length; i++)
            {
                _readyItemCtrls[i].SetItem(_itemTypes[i]);
            }
        }
    }

    #region EVENT
    public void Play()
	{
        _playBtn.interactable = false;
        StateManager.Instance.ActiveBlockScreen(true);
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_start);
        RequestStagePlay();
    }

    public void GoToItemShop()
    {
        ItemShopState.Open(ItemType.HINT);
    }

    public void OnClose()
    {
        SelectStageState.Open(GameManager.Instance.InGameData.ThemeInfo, GameManager.Instance.InGameData.ThemeDifficulty);
    }

    public override void OnBack()
    {
        OnClose();
    }

    #endregion

    private void RequestStagePlay()
    {
        //사용 아이템 체크.
        List<string> itemIdList = new List<string>();
        for (int i = 0; i < _readyItemCtrls.Length; i++)
        {
            string itemId = _readyItemCtrls[i].ItemType.ToString();
            if (_readyItemCtrls[i].IsUseCheck)
            {
                itemIdList.Add(itemId);
                PlayerPrefs.SetString(itemId, GameConstants.GAME_READY_PASSIVE_ITEM_USE);
            }
            else
            {
                PlayerPrefs.SetString(itemId, GameConstants.GAME_READY_PASSIVE_ITEM_UNUSE);
            }
        }
        ReqeustStagePlay reqeustStagePlay = new ReqeustStagePlay();
        reqeustStagePlay.difficultyType = _inGameData.ThemeDifficulty;
        reqeustStagePlay.stageId = _inGameData.StageInfo.stageId;
        reqeustStagePlay.themeId = _inGameData.ThemeInfo.id;
        reqeustStagePlay.userId = DataManager.Instance.UserInfo.Id;
        reqeustStagePlay.itemIdList = itemIdList;

        ApiManager.Instance.RequestPlayInfo(reqeustStagePlay,(result)=>
        {
            if (result.isSuccess)
            {
                _playButtonAnimator.SetTrigger("show");
                SoundManager.Instance.PlayUISound(_playButtonAnimator.gameObject, AudioDataKey.ui_use_heart);

                LeanTween.delayedCall(1.3f, () =>
                {  
                    StateManager.Instance.ActiveBlockScreen(false);
                    _requestPlayInfoId = result.result;
                    GameManager.Instance.InGameData.SetRequestStageInfo(_requestPlayInfoId, itemIdList, _findDifferenceCount);
                    StateManager.Instance.OpenState<BaseStateData>(typeof(InGameState), true);
                });
            }
            else
            {
                _playBtn.interactable = true;
                StateManager.Instance.ActiveBlockScreen(false);
                if (result.errorInfo.errorCode == ApiErrorCode.NOT_ENOUGH_HEART)
                {
                    PopupPurchaseHeartState.Open(isPurchased => {
                        if (isPurchased)
                        {
                            RequestStagePlay();
                        }
                    });
                }
                else
                {
                    MessageBoxState.Open(result.errorInfo);                
                }
            }            
        });

    }

}