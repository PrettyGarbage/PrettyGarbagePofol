using Common;
using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_012_C : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EL_012_C_2;
    [SerializeField] PlayableDirector director_EL_012_C_2_SlideEnd;

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
            Logger.Log("탈출구정상!! 짐버려!! 이쪽으로!! Good Exit! Leave Everything! Come This Way!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {                   
            Logger.Log("5, 9번 승객 탈출하는 애니");

            await director_EL_012_C_2.PlayAsync();
            await director_EL_012_C_2_SlideEnd.PlayAsync();
          
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}