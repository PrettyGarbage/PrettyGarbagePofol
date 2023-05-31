using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_006_BCD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1).AddTo();

            NextMission();
        }).AddTo();

        //핸드셋 울림 있어야함
        
        OnBeginMission(1).Subscribe(async _ =>
        {
           await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            /* 핸드셋 받는 것 포인팅으로 하기
            var result1 = await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo();*/
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 1).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();  
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}