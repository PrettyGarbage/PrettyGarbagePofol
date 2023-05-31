using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_003_D : Mission
{
    #region Fields
    [SerializeField] GameObject glovesIcon;
    [SerializeField] PlayableDirector director_003_D;
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 2);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            glovesIcon.gameObject.SetActive(true);
            Logger.Log("석면장갑을 찾아주세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            glovesIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("석면장갑 들기");
            await director_003_D.PlayAsync();
            NextMission();
        }).AddTo();


        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}