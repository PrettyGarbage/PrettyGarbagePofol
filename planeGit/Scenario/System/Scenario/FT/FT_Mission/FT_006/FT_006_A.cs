using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class FT_006_A : Mission
{
    #region Fields

    //[SerializeField] PlayableDirector director_006_A;

    #endregion
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("자리에 앉으세요");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("승객들 자리에 앉는 애니");
            // 연출 상 화장실에서 사람 나와서 벨트 채워야하나?
            //await director_006_A.PlayAsync();
            NextMission();
        }).AddTo(); 
     
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}
