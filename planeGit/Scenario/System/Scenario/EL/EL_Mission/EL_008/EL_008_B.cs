using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;
using Random = System.Random;

public class EL_008_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_008_B_2;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
           await WaitOtherCrewMission(2, 1);   //1번이 2번 미션할때까지 대기
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("승객들을 안전하게 탈출 시키세요. ");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(2,true).Subscribe(async _ =>
        {
            Logger.Log("jumpseat clsoe 연출");
            await director_EL_008_B_2.PlayAsync();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("벨트 풀어! 일어나! 나와! 짐버려! Release Seatbelts! Get Up! Get Out! Leave Everything!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NPCListModel.Instance.NPCList.ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 0);
            });
            
            NextMission();
        }).AddTo();   
       
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}