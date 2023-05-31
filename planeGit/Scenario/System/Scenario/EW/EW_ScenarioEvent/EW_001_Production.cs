using System;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_001_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_001_Production;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_001 시작");
        Logger.Log("연출 : 평화롭게 앉아 있는 승객들");
        Logger.Log("사운드 : 비행기소리");

        SoundManager.Instance.PlaySoundBGM("airplaneEngine");

        NPCListModel.Instance.NPCList.ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 0);
        });
        
        await director_EW_001_Production.PlayAsync();
        
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]);
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]);

        /*BeltListModel.Instance.BeltList.ForEach(belt =>
        {
            belt.Animator.SetBool(Constants.IsEquipBelt, false);
            belt.Animator.Play("Off", 0, 1);
        });

        NPCListModel.Instance.GetsExcept(2, 3, 7, 8, 11).ForEach(async npc =>
        {
            npc.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);
            npc.Seat.Belt.Animator.Play("On", 0, 1);
        });

        Logger.Log("npc4 테이블 펼치기");
        var npc4 = NPCListModel.Instance.Get(4);
        npc4.Seat.Animator.SetBool(Constants.IsTable, true);
        npc4.Seat.Animator.Play("On", 0, 1);

        Logger.Log("npc5 발판 펼치기");
        var npc5 = NPCListModel.Instance.Get(5);
        npc5.Seat.Animator.SetBool(Constants.IsFoot, true);
        npc5.Seat.Animator.Play("On", 0, 1);

        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]);
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]);

        Logger.Log("버드스크라이크 충격에 R1, L2 캐비넷 열어 놓기 이후  EW_010 / EW_017에서 닫음");
        
        r1Cabinet.Animator.speed = 2f;
        l2Cabinet.Animator.speed = 2f;
        r1Cabinet.Animator.SetBool(Constants.OpenCabinetR, true);
        l2Cabinet.Animator.SetBool(Constants.OpenCabinetL, true);*/
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_001 종료");
    }

    #endregion
}