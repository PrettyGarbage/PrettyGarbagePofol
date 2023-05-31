using Common;
using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_011_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_011_A_1;
    [SerializeField] PlayableDirector director_EL_011_A_npc3npc4;
    [SerializeField] PlayableDirector director_EL_011_A_npc10;
  
    #endregion
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당겨");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당기는 애니 연출");
            Logger.Log("슬라이드 팽창되는 애니");

            await director_EL_011_A_1.PlayAsync();
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("슬라이드 팽창 확인");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("탈출구 정상!! 짐버려!! 이쪽으로!!Good Exit! Leave Everything! Come This Way!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("10번 승객 캐리어를 가지고 L1 탈출구 방향으로 이동한다. ");
            
            NPCListModel.Instance.Gets(3, 4, 10).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 1);
            });
            
            await director_EL_011_A_npc3npc4.PlayAsync();
            await director_EL_011_A_npc10.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}