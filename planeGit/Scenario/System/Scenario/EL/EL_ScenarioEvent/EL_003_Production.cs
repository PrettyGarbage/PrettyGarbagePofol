using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EL_003_Production : ScenarioEventProduction
{
    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_003 시작");
        Logger.Log("착륙할 것을 기다리며 벨트를 매고 자리에 앉아 있음");
        
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 3);
        
        // 1번 승무원의 jumpseat 
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 5);
        
        // jumpSeatF1.Animator.SetTrigger(Constants.jumpSeatOpen);
        // jumpSeatF1.Animator.SetBool(Constants.IsEquipFABelt, true);
        // await jumpSeatF1.Animator.WaitAnimationCompleteAsync(Constants.JumpSeat_Belt);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_003 종료");
    }

    #endregion
}