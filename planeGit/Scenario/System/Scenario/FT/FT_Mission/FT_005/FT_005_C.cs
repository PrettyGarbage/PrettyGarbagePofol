using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class FT_005_C : Mission
{
    #region Fields
    // 카트가 닫히는 애니
    // jumpseat 펼쳐지는 애니
    [SerializeField] PlayableDirector director_005_C_1;
    [SerializeField] PlayableDirector director_005_C_2;
    #endregion

    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("서비스중인 카트를 locking하고 가까운 좌석에 앉아 좌석벨트를 매세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 카트가 닫히는 애니
            await director_005_C_1.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("좌석에 앉아 좌석벨트를 매세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();   
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            // seat 벨트 매는 애니
            await director_005_C_2.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}