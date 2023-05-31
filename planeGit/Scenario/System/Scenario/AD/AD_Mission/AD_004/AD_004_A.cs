using Common;
using UnityEngine;
using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine.Playables;

public class AD_004_A : Mission
{
    #region Fields

    // 몇몇 모델들 탑승중인 director
    [SerializeField] PlayableDirector director_004_A;    

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("AD_004 시작");
            Logger.Log("짐 보관 협조 방송을 하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);

            Logger.Log("손님 여러분, 가지고 계신 짐은 앞 좌석 밑이나 선반 속에 보관해주시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10));

            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_004_A.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}