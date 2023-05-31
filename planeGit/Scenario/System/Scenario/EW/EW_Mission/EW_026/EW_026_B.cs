using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_026_B : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("R1 탈출구로 이동하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 이동하는 연출필요 
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}