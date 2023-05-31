using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EL_000_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_000_Production;
    [SerializeField] PlayableDirector director_EL_000_BeltSetting;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EL_000 시작");
        Logger.Log("연출 : 평화롭게 앉아 있는 승객들");
        Logger.Log("사운드 : 비행기소리");
        
        SoundManager.Instance.PlaySoundBGM("airplaneEngine");
        
        director_EL_000_BeltSetting.Play();
        director_EL_000_Production.Play();
        
        NPCListModel.Instance.Get(1).Animator.SetFloat(Constants.IdleState, 0);
        NPCListModel.Instance.Get(2).Animator.SetFloat(Constants.IdleState, 11);
        
        NPCListModel.Instance.Gets(3, 4).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 10);
        });
        
        NPCListModel.Instance.Get(5).Animator.SetFloat(Constants.IdleState, 2);
        NPCListModel.Instance.Get(6).Animator.SetFloat(Constants.IdleState, 3);
        
        NPCListModel.Instance.Gets(7, 8).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 12);
        }); 
        
        NPCListModel.Instance.Gets(9, 10).ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 0);
        });
        
        Logger.Log("시작 준비가 완료되셨나요 ?");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 5);

    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EL_000 종료");
    }

    #endregion
}