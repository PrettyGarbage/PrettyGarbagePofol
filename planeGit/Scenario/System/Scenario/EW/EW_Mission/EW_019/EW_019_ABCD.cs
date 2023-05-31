using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_019_ABCD : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_019_All;
    
    #endregion   
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("충격 방지 자세를 취해주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            /*NPCListModel.Instance.Gets(3, 6, 11, 12).ForEach(npc =>
            {
                Logger.Log("3, 6, 11, 12 충격방지 자세 ");
                npc.Animator.SetInteger(Constants.IdleState, 5);
            });
            
            NPCListModel.Instance.GetsExcept(3, 6, 11, 12).ForEach(npc =>
            {
                Logger.Log("3, 6, 11, 12 외 모델 충격방지 자세 ");
                npc.Animator.SetInteger(Constants.IdleState, 4);
            });*/
            await director_EW_019_All.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 5).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 5).AddTo()); 
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 5).AddTo());
            
            NextMission();
        }).AddTo();   
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}