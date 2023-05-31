using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UnityEngine;

public class EW_021_Production : ScenarioEventProduction
{

    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_021 시작");
        NPCListModel.Instance.NPCList.ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 0));
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_021 종료");
    }

    #endregion
}