using Common;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.Playables;

public class EL_013_D : Mission
{

    #region Fields

    [SerializeField] PlayableDirector director_EL_013_D_1;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("처음 탈출하는 승객에게 지시하세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            Logger.Log("밑에서 내려가는 사람들을 도와주세요! Stay At The Bottom! Help People Off!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("내려가는 사람 잡아주는 애니");
            await director_EL_013_D_1.PlayAsync();

            NextMission();
        }).AddTo();  
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}