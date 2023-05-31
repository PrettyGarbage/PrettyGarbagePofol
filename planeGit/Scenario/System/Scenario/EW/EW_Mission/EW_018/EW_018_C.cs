using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_018_C : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_018_C_2;

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
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();

        }).AddTo();
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("안전벨트 착용하는 애니 보여주기");
            
            await director_EW_018_C_2.PlayAsync();
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }
    #endregion
}
