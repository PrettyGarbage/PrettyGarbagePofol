using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_013_All : Mission
{
    #region Fields

    [SerializeField] GameObject maskIcon;
    [SerializeField] PlayableDirector director_013;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("사용된 산소 마스크를 좌석 앞주머니에 넣어주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("산소 마스크를 선택해주세요.");
            maskIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            maskIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_013.PlayAsync();
         
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}