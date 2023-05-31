using System;
using Common;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_021_A : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_021_A_3;
    
    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        var npc1 = NPCListModel.Instance.Get(1);
        var npc7 = NPCListModel.Instance.Get(7);
        var npc8 = NPCListModel.Instance.Get(8);
        var npc9 = NPCListModel.Instance.Get(9);

        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("21번 A 시작");

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("핸드셋 잡는 모션 해야함");
            
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[0], 10).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("기장님, 객실 뒷편에 물이 차고 있습니다.");

            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            await director_EW_021_A_3.PlayAsync();
            try
            {
                npc7.Animator.SetFloat(Constants.IdleState, 1);
                npc8.Animator.SetFloat(Constants.IdleState, 1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();  
        
    }

    #endregion
}