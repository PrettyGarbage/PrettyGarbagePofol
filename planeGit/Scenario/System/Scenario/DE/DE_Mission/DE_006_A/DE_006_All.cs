using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_006_All : Mission
{
    #region Fields
    [SerializeField] GameObject jumpSeat_Open;
    [SerializeField] GameObject mask;
    
    [SerializeField] PlayableDirector director_JumpSeatClose;
    [SerializeField] PlayableDirector director_Mask;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("가장 가까운 좌석 혹은 승무원 Jumpseat 착석하여 산소마스크를 착용하세요.");
            jumpSeat_Open.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            jumpSeat_Open.gameObject.SetActive(false);

            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_JumpSeatClose.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("산소마스크를 착용하세요.");
            mask.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            mask.gameObject.SetActive(false);
            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("산소 마스크 착용하는 애니?");
            await director_Mask.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}