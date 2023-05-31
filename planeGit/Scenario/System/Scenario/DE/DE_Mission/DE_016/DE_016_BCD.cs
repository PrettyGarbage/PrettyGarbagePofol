using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_016_BCD : Mission
{
    #region Fields

    [SerializeField] GameObject seatBackIcon;
    [SerializeField] PlayableDirector director_016;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("등받이를 원위치 시키지 않은 승객을 찾아 등받이를 원위치 시키세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("등받이를 원위치 시키기 위해서 선택하세요");
            seatBackIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            seatBackIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("등받이 원위치 되는 애니");
            await director_016.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}