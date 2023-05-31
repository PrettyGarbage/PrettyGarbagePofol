using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_005_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_005_D;

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
            Logger.Log("승객을 착석 시키고 모든 휴대용 수하물 정위치를 확인 후 모든 선반을 닫으세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 선받 닫히는 애니
            await director_005_D.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}