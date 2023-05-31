using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_007_A : Mission
{
    #region Fields

    //[SerializeField] GameObject interPhone;
    //[SerializeField] PlayableDirector director_007_A_3;

    #endregion
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("모든 승무원에게 All Call(인터폰) 하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("cabin crew, door side stand by, Safety check ");
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[1], 10).AddTo();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}
