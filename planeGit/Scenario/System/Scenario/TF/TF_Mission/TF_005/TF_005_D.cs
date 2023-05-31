using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_005_D : Mission
{
    #region Fields
    [SerializeField] GameObject towel;
    [SerializeField] PlayableDirector director_005_D;
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
            Logger.Log("젖은 타월을 선택하세요.");
            towel.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            towel.gameObject.SetActive(false);

            NextMission();
        }).AddTo();   
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("5. 승객이 젖은 타월을 받아 코와 입을 막는 연출하기");
            await director_005_D.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}