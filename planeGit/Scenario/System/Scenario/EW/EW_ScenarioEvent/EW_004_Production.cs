using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Library.Manager;


public class EW_004_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_004 시작");
        
        Logger.Log("연출 : 비행기 엔진의 불안정한 소리");
        
        Logger.Log("자막 :동요하는 승객들을 안심시키세요");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo();
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        NPCListModel.Instance.NPCList.ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 0));
        Logger.Log("EW_004 종료");
    }

    #endregion
}