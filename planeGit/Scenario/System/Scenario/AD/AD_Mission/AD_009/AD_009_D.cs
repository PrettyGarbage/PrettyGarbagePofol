using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_009_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_009_D;

    #endregion

    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("모든 compartment 고정 상태를 확인해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_009_D.PlayAsync();
            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}