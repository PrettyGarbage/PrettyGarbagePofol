using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class FT_004_D : Mission
{

    #region Fields
    // 6번 승객
    [SerializeField] PlayableDirector director_004_D;
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
            Logger.Log("승객 좌석 벨트를 확인해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("승객 좌석 벨트가 채워지는 애니");
            await director_004_D.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}