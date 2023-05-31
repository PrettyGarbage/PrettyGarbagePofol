using Common;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.Playables;

public class EL_013_C : Mission
{
    #region Fields
    [SerializeField] PlayableDirector director_EL_013_C_1;
    [SerializeField] PlayableDirector director_EL_013_C_1_SlideEnd;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구정상!! 짐버려!! 이쪽으로!! Good Exit! Leave Everything! Come This Way!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("5번승객 내려가는 애니");

            await director_EL_013_C_1.PlayAsync();
            await director_EL_013_C_1_SlideEnd.PlayAsync();
            
            NextMission();
        }).AddTo();  
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}