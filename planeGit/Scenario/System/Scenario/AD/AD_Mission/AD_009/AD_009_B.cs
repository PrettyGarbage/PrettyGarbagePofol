using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_009_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_009_B_1;
    [SerializeField] PlayableDirector director_009_B_2;
    
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
            Logger.Log("화장실에 있는 승객 발견하고, 자리에 앉도록 유도하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_009_B_1.PlayAsync();
            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("자리에 앉아 안전벨트를 매세요.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();  
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("안전벨트를 매는 애니");
            await director_009_B_2.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}