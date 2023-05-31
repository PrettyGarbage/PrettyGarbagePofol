using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EE_013 : Mission
{
    #region Fields

    //[SerializeField] PlayableDirector director_013;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Smoke detecotr: 위치확인, 이물질 유무를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            //await director_013.PlayAsync();
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}