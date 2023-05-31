using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_006_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_006_A;

    #endregion
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("기장에게 인터폰을 통해 승객 탑승을 알리고 출입문을 닫을 준비 됨을 알리세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("승객 000 탑승했습니다. 출입문 닫을 준비 되었습니다.");
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[1], 10).AddTo();
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("L1 탑승구(출입문)를 닫으세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("L1 문 닫히는 애니");
            await director_006_A.PlayAsync();
            NextMission();
        }).AddTo(); 
     
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}
