using Common;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_010_B : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_010_B_2;

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
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10, false).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {  
            Logger.Log("연출 : 3번 승객이 어린이 물건을 overheadbin으로 올리기");

            await director_EW_010_B_2.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            NextMission();
        });  
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        });
    }

    #endregion
}
