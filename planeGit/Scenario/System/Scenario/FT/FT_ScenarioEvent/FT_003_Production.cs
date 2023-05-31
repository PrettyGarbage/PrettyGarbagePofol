using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class FT_003_Production : ScenarioEventProduction
{
    [SerializeField] PlayableDirector director_003_Prop;
    [SerializeField] PlayableDirector director_003_Production;
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("FT_003 시작");

        Logger.Log("비행기 동체 흔들림");
        UniTask.Delay(5000);
        director_003_Prop.Play();
        NPCListModel.Instance.Gets(1, 2, 5).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 31));
        NPCListModel.Instance.Gets(3, 7, 9).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 34));
        NPCListModel.Instance.Gets(4, 6).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 33));
        NPCListModel.Instance.Get(8).Animator.SetFloat(Constants.IdleState, 35);
        NPCListModel.Instance.Get(10).Animator.SetFloat(Constants.IdleState, 32);
        await director_003_Production.PlayAsync();
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("FT_003 종료");
    }
    #endregion
}