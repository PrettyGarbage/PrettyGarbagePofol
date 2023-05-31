using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_003_C : Mission
{
    #region Fields

    [SerializeField] GameObject halonPBEIcon;
    [SerializeField] PlayableDirector director_003_C;
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
            halonPBEIcon.gameObject.SetActive(true);
            Logger.Log("jumpseat 하단에서 Halon소화기와 PBE를 준비하여 주세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            halonPBEIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("player 승무원이 소화기와 PBE를 들기");
            await director_003_C.PlayAsync();
            NextMission();
        }).AddTo();


        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}