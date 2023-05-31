using Common;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_014_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_014_A_1;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("승객이 가까운 비상구를 지목하는 연출");
            
            await director_EW_014_A_1.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();  
        
    }

    #endregion
}