using Common;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.Playables;

public class AD_005_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_005_A;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("AD_005 시작");
            Logger.Log("지상직원에게 운송 관련 서류를 전달 받고, 지상 직원에게 객실 준비완료를 통보하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10));

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_005_A.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}