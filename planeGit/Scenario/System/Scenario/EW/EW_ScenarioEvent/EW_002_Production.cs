using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Library.Manager;
using UnityEngine.Playables;

public class EW_002_Production : ScenarioEventProduction
{
    #region Fields
    
    [SerializeField]PlayableDirector director_EW_002_Production;
    #endregion
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_002 시작");
        
        var audioSource = SoundManager.Instance.PlaySoundEffect("boom");
        await director_EW_002_Production.PlayAsync();
        await audioSource.WaitAudioCompleteAsync();
        
        NPCListModel.Instance.Gets(1, 7, 10, 12).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 34));
        NPCListModel.Instance.Gets(2, 3, 8, 11).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 31));
        NPCListModel.Instance.Get(4).Animator.SetFloat(Constants.IdleState, 36);
        NPCListModel.Instance.Gets(5,6).ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 33));
        NPCListModel.Instance.Get(9).Animator.SetFloat(Constants.IdleState, 32);
        
        if (isObserver)
        {
            Logger.Log("버드 스트라이크 시작 신호 전송");

            //CrewTrainingResultPacket.SendPacket(new CrewTrainingResultPacket.PacketData());
        }
        
        Logger.Log("사운드 : 버드스트라이크 안내");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]);
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_002 종료");
    }

    #endregion
}