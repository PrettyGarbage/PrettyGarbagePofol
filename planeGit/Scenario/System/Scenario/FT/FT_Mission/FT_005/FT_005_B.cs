using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class FT_005_B : Mission
{
    #region Fields

    // jumpseat 펼쳐지는 애니
    [SerializeField] PlayableDirector director_005_B;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("즉각적으로 승무원좌석에 착석하고 좌석벨트를 매세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_005_B.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}