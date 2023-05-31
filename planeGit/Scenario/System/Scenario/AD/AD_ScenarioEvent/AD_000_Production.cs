using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class AD_000_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_AD_000_Production;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("AD_000 시작");
        Logger.Log("연출 : 평화롭게 앉아 있는 승객들");
        Logger.Log("사운드 : 비행기소리");
        
        var audioSource = SoundManager.Instance.PlaySoundBGM("airplaneEngine");
        audioSource.Play();

        // 출장가서 timeline으로 설정하기 지상 승무원 서있는 Idle 상태
        /*var npc = NPCListModel.Instance.Get(20);
        npc.Animator.SetFloat(Constants.IdleState, 1);*/

        await director_AD_000_Production.PlayAsync();
   
        Logger.Log("시작 준비가 완료되셨나요 ?");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 5);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("AD_000 종료");
    }

    #endregion
}