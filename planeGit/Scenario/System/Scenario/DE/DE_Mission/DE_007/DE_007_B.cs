using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_007_B : Mission
{
    #region Fields

    [SerializeField] GameObject pssengerIcon;
    [SerializeField] PlayableDirector director_007_B;

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
            Logger.Log("화장실에서 나오는 승객을 빠르게 좌석에 앉게하세요. 승객을 선택하세요.");
            pssengerIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            pssengerIcon.gameObject.SetActive(false);
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("자리 이동하는 애니와 걷는 icon 보여주기");
            await director_007_B.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}