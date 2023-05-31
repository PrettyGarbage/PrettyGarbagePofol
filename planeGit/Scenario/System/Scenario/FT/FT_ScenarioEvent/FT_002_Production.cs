using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class FT_002_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("FT_002 시작");

        Logger.Log("비행중 난기류로 인하여 비행기가 흔들리고 있습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("FT_002 종료");
    }

    #endregion
}