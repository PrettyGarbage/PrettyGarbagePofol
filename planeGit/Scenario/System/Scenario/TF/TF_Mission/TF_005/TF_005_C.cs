using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_005_C : Mission
{
    #region Fields

    [SerializeField] GameObject po2Bottle;
    [SerializeField] GameObject towel;
    [SerializeField] PlayableDirector director_005_C;

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
            Logger.Log("2번 승무원의 근처의 산소통을 화재진압지로부터 멀리 치워 주세요. // 산소통 마크 없음");
            po2Bottle.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            po2Bottle.gameObject.SetActive(false);
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("산소통을 치운는 연출");

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("젖은 타월을 선택하세요.");
            towel.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            towel.gameObject.SetActive(false);

            NextMission();
        }).AddTo();   
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("타월 전달 아이콘이 나타난다.");
            await director_005_C.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}