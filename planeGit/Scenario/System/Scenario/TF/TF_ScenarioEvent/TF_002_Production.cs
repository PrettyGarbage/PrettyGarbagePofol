using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class TF_002_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("TF_002 시작");
        
        Logger.Log("기내 화장실 화재가 발생하였습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);
        
        Logger.Log("Smoke detector 경보음_(청각효과) 필요함");

    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("TF_002 종료");
    }

    #endregion
}