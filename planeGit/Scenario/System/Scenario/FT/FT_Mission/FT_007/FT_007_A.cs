using UnityEngine;
using System;
using Common;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class FT_007_A : Mission
{

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            // 벨트 사인 사운드 정해지면 대체하기
            SoundManager.Instance.PlaySoundEffect("Airplane_Ding_Dong");
            
            Logger.Log("손님 여러분 저희 비행기는 심한 난기류를 만나 비행기가 많이 흔들렸으나 지금은 정상비행을 하고 있습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 15).AddTo());
            
            Logger.Log("손님 중에 몸이 불편하신 분이 계시면 저희 승무원에게 알려주시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();   
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            NPCListModel.Instance.NPCList.ForEach(npc => npc.Animator.SetFloat(Constants.IdleState, 0));
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}
