using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EL_005_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_005_Production;

    #endregion
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        SoundManager.Instance.PlaySoundEffect("Airplane_ding_dong");
        
        Logger.Log("EW_005 시작");
        Logger.Log("항공기 굉음도로 긁히는 소리큰 소음승객 비명");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 3).AddTo();
        
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 5).AddTo();

        await director_EL_005_Production.PlayAsync();

        NPCListModel.Instance.NPCList.ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 6);
        });

        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 5).AddTo();
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[3], 5).AddTo();
        
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_005 종료");
    }

    #endregion
}