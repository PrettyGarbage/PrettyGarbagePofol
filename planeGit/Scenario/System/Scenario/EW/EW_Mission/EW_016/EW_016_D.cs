using Common;
using Library.Manager;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_016_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_016_D_2;

    #endregion
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("각 Life Raft 별로 장비 이동을 도와줄 협조자를 2명씩 2개조를 선정하고 임무를 부여하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            Logger.Log("창문탈출구의 overheadbin에 있는 Life Raft을 두분씩 협력해서 L1, R1탈출구로 이동시켜주시고 승무원들의 지시에 따라 이동시켜주십시요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();

        // 7,8번 승객이 Raft 설정을 하게 되면 overheadbin을 가리킴
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EW_016_D_2.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}