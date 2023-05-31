using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_003_A : Mission
{
    #region Fields
    //[SerializeField] PlayableDirector director_AD_003_A;
    #endregion

    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("AD_003 시작");
            Logger.Log("승객 상황 : 기내 밖에서 탑승 대기 중");
            
            Logger.Log("탑승 뮤직을 ON 해주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            //await director_AD_003_A.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}