using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_026_CD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("overwing window exit로 이동하세요.");
            // overwing window 모델 등록하고 포인팅사용하기
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();

        // 이동하는 연출필요 
    }

    #endregion
}