using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_010_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_010_A_1;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Jumpseat에 착석하시고 벨트및 shoulder harness 착용 및 30second Review 해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("jumpseat에 착석하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EL_010_A_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}