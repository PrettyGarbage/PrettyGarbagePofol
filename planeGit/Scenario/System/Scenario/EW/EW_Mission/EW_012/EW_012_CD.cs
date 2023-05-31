using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using UniRx;
using UnityEngine;

public class EW_012_CD : Mission
{
    
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
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[4], 10).AddTo());

            NextMission();
        }).AddTo();  
        
        OnBeginMission(5).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}