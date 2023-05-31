using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_007_B : Mission
{
    #region Fields
    [SerializeField] GameObject toiletLockIcon;
    [SerializeField] PlayableDirector director_007_B;

    #endregion
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("화장실을 점검후 폐쇄 해주세요");
            toiletLockIcon.gameObject.SetActive(true);
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            toiletLockIcon.gameObject.SetActive(false);
            await director_007_B.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("화장실을 점검후 폐쇄합니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}