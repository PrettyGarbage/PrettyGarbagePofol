using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_013_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_013_A_1;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("처음 탈출하는 승객에게 지시하세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 5).AddTo());
            Logger.Log("밑에서 내려가는 사람들을 도와주세요! Stay At The Bottom! Help People Off!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("3, 4번 승객이 슬라이드 아래에서 내려오는 승객을 도와주는 애니 : 현재는 리소스 변경을 빼놓은 상태");
            await  director_EL_013_A_1.PlayAsync();
            NextMission();
        }).AddTo();  
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}