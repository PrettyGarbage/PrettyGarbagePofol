using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_007_B : Mission
{
    [SerializeField] PlayableDirector director_007_B_goToilet;

    #region Fields

    [SerializeField] PlayableDirector director_007_B_1;
    //[SerializeField] PlayableDirector director_007_B_2;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_007_B_goToilet.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("R1 Door mode 변경 하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            await director_007_B_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("L1과 Cross Check 하세요 ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}