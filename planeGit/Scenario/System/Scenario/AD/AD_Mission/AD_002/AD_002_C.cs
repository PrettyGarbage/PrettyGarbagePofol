using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_002_C : Mission
{
    #region Fields
    [SerializeField] PlayableDirector director_002_C;    
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
            Logger.Log("네. 알겠습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();  
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("머리 위 선반(overheadbin)을 모두 오픈해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
           
            NextMission();
        }).AddTo();   
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("오버헤드빈이 열린다.");
            await director_002_C.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}