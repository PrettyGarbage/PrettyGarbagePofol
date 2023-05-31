using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_012_B : Mission
{
    #region Fields

    [SerializeField] NPCModel[] npcList;

    [SerializeField] PlayableDirector director_EW_012_B_2;

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
            // 사운드는 없음
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EW_012_B_2.PlayAsync();

            NPCListModel.Instance.NPCList.ForEach(npc =>
                {
                    npc.Animator.SetFloat(Constants.IdleState, 6);
                }
            );
            
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(5).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}