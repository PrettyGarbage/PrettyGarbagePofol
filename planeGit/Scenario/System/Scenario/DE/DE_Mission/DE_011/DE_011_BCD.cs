using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_011_BCD : Mission
{
    #region Fields

    [SerializeField] GameObject helpIcon;
    [SerializeField] PlayableDirector director_011;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("부상 승객을 응급조치 하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            
            Logger.Log("승객을 치료 해주세요.");
            helpIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            helpIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("승객이 치료되는 애니메이션 ");
            await director_011.PlayAsync();
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}