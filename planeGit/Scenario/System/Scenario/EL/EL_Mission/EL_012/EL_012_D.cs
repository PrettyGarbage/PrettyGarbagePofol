using Common;
using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_012_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_012_D_npc7;
    [SerializeField] PlayableDirector director_EL_012_D_npc7_SlideEnd;

    [SerializeField] PlayableDirector director_EL_012_D_npc8;
    [SerializeField] PlayableDirector director_EL_012_D_npc8_SlideEnd;

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
            Logger.Log("처음 탈출하는 승객에게 지시하세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());  
            
            Logger.Log("밑에서 내려가는 사람들을 도와주세요! Stay At The Bottom! Help People Off! ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("7, 8번 승객 탈출하는 애니");

            await director_EL_012_D_npc7.PlayAsync();
            await director_EL_012_D_npc7_SlideEnd.PlayAsync();

            await director_EL_012_D_npc8.PlayAsync();
            await director_EL_012_D_npc8_SlideEnd.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}