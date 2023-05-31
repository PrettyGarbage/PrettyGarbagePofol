using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_006_C : Mission
{
    #region Fields

    [SerializeField] GameObject coughIcon;
    [SerializeField] PlayableDirector director_006_C;

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
            Logger.Log("승객을 진정시키세요 기침하는 승객을 선택하세요.");
            coughIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            coughIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("진정된 승객이 아이콘이 나타난다.");
            await director_006_C.PlayAsync();
            NextMission();
        }).AddTo();   
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}