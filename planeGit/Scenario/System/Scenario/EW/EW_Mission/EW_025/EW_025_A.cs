using Common;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EW_025_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director;
    [SerializeField] PlayableDirector director2;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        var npc4 = NPCListModel.Instance.Get(4);
        var npc5 = NPCListModel.Instance.Get(5);

        var npc7 = NPCListModel.Instance.Get(7);
        var npc8 = NPCListModel.Instance.Get(8);

        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("협력자 4명에게 Raft를  사용할 Door쪽으로 옮길 것을 지시하세요.");
            Logger.Log("NPC 2명 이상 지목하기");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 5).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("4, 5번 overheadbin에 raft 이동하기");

            Logger.Log("npc 4, 5번 이동");

            await director.PlayAsync();
            try
            {
                npc4.Animator.SetFloat(Constants.IdleState, 8);
                npc5.Animator.SetFloat(Constants.IdleState, 9);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            await director2.PlayAsync();
            try
            {
                npc7.Animator.SetFloat(Constants.IdleState, 8);
                npc8.Animator.SetFloat(Constants.IdleState, 9);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 20).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}