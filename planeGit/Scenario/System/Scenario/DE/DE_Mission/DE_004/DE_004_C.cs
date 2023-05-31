using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_004_C : Mission
{
    #region Fields

    [SerializeField] GameObject BeltIcon;
    [SerializeField] PlayableDirector director_004_C;

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
            Logger.Log("풀려있는 안전벨트 아이콘이 나타난다.");
            
            BeltIcon.gameObject.SetActive(true);
            Logger.Log("승객이 안전벨트를 매도록 안전벨트를 선택하세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            BeltIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();  
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("잠긴 안전벨트 아이콘이 나타난다.");
            
            await director_004_C.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}