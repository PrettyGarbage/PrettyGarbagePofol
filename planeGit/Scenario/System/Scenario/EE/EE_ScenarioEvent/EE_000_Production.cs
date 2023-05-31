using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_000_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] Interactable tablet;

    #endregion
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EE_000 시작");
        Logger.Log("시작 준비가 완료되셨나요?");
        
        AttachSystem.Instance.Attach(tablet, Valve.VR.InteractionSystem.Player.instance.rightHand);
        
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EE_000 종료");
    }

    #endregion
}