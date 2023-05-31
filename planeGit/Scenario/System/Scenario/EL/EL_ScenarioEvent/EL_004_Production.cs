using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EL_004_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_004_Production;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EL_004 시작");
        Logger.Log("Overrun 으로 인한 엔진의 큰 소음 ");
        
        director_EL_004_Production.Play();

        NPCListModel.Instance.Gets(1, 9).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 33);
        });

        NPCListModel.Instance.Gets(2, 10).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 31);
        }); 
        
        NPCListModel.Instance.Gets(3, 4, 7).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 31);
        });

        NPCListModel.Instance.Get(5).Animator.SetFloat(Constants.IdleState, 33);
        
        NPCListModel.Instance.Gets(6,8).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 32);
        });
      
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 3);
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 3);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EL_004 종료");
    }

    #endregion
}