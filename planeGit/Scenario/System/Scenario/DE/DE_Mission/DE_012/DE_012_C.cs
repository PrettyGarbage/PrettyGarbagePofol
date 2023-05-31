using Common;
using UnityEngine;
using System;
using System.Drawing;
using UniRx;
using UnityEngine.Playables;

public class DE_012_C : Mission
{
    #region Fields
    [SerializeField] GameObject helpIcon;
    [SerializeField] PlayableDirector director_012_C;
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("6번 승객의 다리에 피가 흐르고 있다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("2번 승무원 FAK 가져오세요!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 2);
            
            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("FAK 키트로 다친 승객을 응급처치해 주세요.");
            helpIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            helpIcon.gameObject.SetActive(false);
            NextMission();
        }).AddTo();   
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            await director_012_C.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}