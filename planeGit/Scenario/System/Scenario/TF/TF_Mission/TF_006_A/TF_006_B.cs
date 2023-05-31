using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_006_B : Mission
{
    #region Fields

    [SerializeField] GameObject trashBox;
    [SerializeField] GameObject smokeDetector;
    [SerializeField] PlayableDirector director_006_B_1;
    [SerializeField] PlayableDirector director_006_B_2;

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
            Logger.Log("화재의 근원지를 다시 한번 확인하세요.");
            trashBox.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            trashBox.gameObject.SetActive(false);
            await director_006_B_1.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("화재가 완전히 진압되었습니다");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();   
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("화재를 완전 진압후 Smoke Detector를 리셋시 켜주세요.");
            Logger.Log(" smoke detector 선택  ");
            smokeDetector.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            smokeDetector.gameObject.SetActive(false);

            NextMission();
        }).AddTo();
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("smoke detector 리셋 되는 연출 필요");
            await director_006_B_2.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}