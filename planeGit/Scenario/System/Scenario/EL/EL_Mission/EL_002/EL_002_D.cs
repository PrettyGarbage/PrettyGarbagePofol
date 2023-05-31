using Common;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.Playables;

public class EL_002_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_002_D;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("등받이를 원위치 시키지  않은 승객을 찾아 등받이 원위치 시키도록 지시하세요.");

            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("손님 등받이 원위치 시켜주십시오.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            NPCListModel.Instance.Gets(7, 8).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 0);
            });
            
            await director_EL_002_D.PlayAsync();

            /*NPCListModel.Instance.Gets(7, 8).ForEach(npc =>
            {
                Logger.Log("7, 8번 승객 등받이 되돌리기");
                npc.Seat.Animator.SetBool(Constants.IsBack, false);
                npc.Animator.SetBool(Constants.IsManTalkSecond, false);
                npc.Animator.SetBool(Constants.IsWomanTalkSecond, false);
            });*/
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}