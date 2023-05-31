using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_008_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_008_A_1;
    [SerializeField] PlayableDirector director_EL_008_A_5;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("안전벨트를 풀고 승객들을 안전하게 탈출 시키세요.  ");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("jumpseat에서 close 연출");
            await director_EL_008_A_1.PlayAsync();    

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("Jumpseat 상단에 있는 ELS 버튼을 눌러주세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("ELS 버튼 눌렀을 때 바닥에 불들어오는 연출 필요");
            // 불룸처리 연출넣기
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 2, 3, 4);
            NextMission();
        }).AddTo();

        OnBeginMission(5, true).Subscribe(async _ =>
        {
            Logger.Log("안전벨트를 푼 승객들이 일어선다. 연출하기");
            await director_EL_008_A_5.PlayAsync();
            
            NPCListModel.Instance.NPCList.ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 0);
            });

            NextMission();
        }).AddTo();


        OnBeginMission(6).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}