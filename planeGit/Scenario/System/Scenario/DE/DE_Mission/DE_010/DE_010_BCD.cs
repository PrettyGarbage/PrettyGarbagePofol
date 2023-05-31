using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_010_BCD : Mission
{
    #region Fields

    [SerializeField] GameObject carrierIcon;
    [SerializeField] PlayableDirector director_Carrier;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("이동에 방해가 되는 물건들을 정리해 주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();

            Logger.Log("떨어진 짐을 선택하세요.");
            carrierIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            carrierIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_Carrier.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}