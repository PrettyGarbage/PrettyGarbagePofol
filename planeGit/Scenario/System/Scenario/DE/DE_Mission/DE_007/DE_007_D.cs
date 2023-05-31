using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_007_D : Mission
{
    #region Fields

    [SerializeField] GameObject maskIcon;
    [SerializeField] PlayableDirector director_007_D;

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
            Logger.Log("어린이를 동반한 승객에게 마스크를 착용시키세요. 마스크를 선택하세요.");
            maskIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            maskIcon.gameObject.SetActive(false);
            
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("산소 마스크 쓰는 애니와 해당 icon 보이기");
            await director_007_D.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}