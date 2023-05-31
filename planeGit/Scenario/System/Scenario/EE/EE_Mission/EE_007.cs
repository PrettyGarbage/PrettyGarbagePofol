using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
public class EE_007 : Mission
{
    #region Fields

    //[SerializeField] PlayableDirector director_007;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("PBE : 정위치를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            //await director_007.PlayAsync();
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}