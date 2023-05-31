using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class FT_000_Production : ScenarioEventProduction
{
    #region Fields

    //[SerializeField] PlayableDirector director_AD_000_Production;
    [SerializeField] PlayableDirector director_AD_000_BeltSetting;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("FT_000 시작");
        Logger.Log("연출 : 평화롭게 앉아 있는 승객들");
        Logger.Log("사운드 : 비행기소리");
        
        SoundManager.Instance.PlaySoundBGM("airplaneEngine");
        
        //await director_AD_000_Production.PlayAsync();
        await director_AD_000_BeltSetting.PlayAsync();
   
        Logger.Log("시작 준비가 완료되셨나요 ?");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 5);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("FT_000 종료");
    }

    #endregion
}