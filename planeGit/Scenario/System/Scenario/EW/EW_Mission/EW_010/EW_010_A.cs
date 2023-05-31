using System.Linq;
using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_010_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_010_A_1;

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            // 핸드백 모델 지목하면 6번 승객이 overheadbin으로 올리기
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10, false).AddTo());
                
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_EW_010_A_1.PlayAsync();
            
            NextMission();
        }).AddTo(); 

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
