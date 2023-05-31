using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EL_007_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_007_Production;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        SoundManager.Instance.PlaySoundEffect("Airplane_ding_dong");

        Logger.Log("EL_007 시작");
        
        NPCListModel.Instance.Gets(1, 10).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 33);
        });

        NPCListModel.Instance.Gets(2, 3, 8).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 31);
        });

        NPCListModel.Instance.Gets(4, 7).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 32);
        });

        NPCListModel.Instance.Get(5).Animator.SetFloat(Constants.IdleState, 33);

        NPCListModel.Instance.Gets(6, 9).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 32);
        });
        
        await director_EL_007_Production.PlayAsync();

        Logger.Log("저는 기장입니다. 탈출하십시오! 탈출하십시오! ");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 3);
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EL_007 종료");
    }

    #endregion
}