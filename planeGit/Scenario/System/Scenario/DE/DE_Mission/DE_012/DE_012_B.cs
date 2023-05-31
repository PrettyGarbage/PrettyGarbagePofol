using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_012_B : Mission
{
    #region Fields

    [SerializeField] GameObject FAKIcon;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 3);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("Overhead bin에서 FAK를 찾아 3번 승무원에게 전달해 주세요.");
            FAKIcon.gameObject.SetActive(true);

            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            FAKIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 전달 어떻게?

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}